using System;
using System.Threading.Tasks;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Customizations.Authorization;
using Scadenzario.Models.Enums;
using Scadenzario.Models.InputModels;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.Services.Applications.Beneficiari;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Beneficiari;
using Scadenze.Models.ViewModels;

namespace Scadenzario.Controllers
{
    [Authorize] 
    public class BeneficiariController : Controller
    {
        private readonly IBeneficiariService service;
        
        public BeneficiariController(ICachedBeneficiarioService service)
        {
            this.service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(BeneficiarioListInputModel input)
        {
            ViewData["Title"] = "Lista Beneficiari";
            ListViewModel<BeneficiarioViewModel> beneficiari = await service.GetBeneficiariAsync(input);

            BeneficiarioListViewModel viewModel = new BeneficiarioListViewModel
            {
                Beneficiari = beneficiari,
                Input = input
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Dettaglio Beneficiario";
            BeneficiarioDetailViewModel viewModel = await service.GetBeneficiarioAsync(id);
            return View(viewModel);
        }
        [HttpGet]
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo beneficiario";
            BeneficiarioCreateInputModel inputModel = new();
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [ValidateReCaptcha]
        [HttpPost]
        public async Task<IActionResult> Create(BeneficiarioCreateInputModel inputModel)
        {
            if(ModelState.IsValid)
            {
                await service.CreateBeneficiarioAsync(inputModel);
                TempData["ConfirmationMessage"] = "Ok! Il tuo beneficiario è stato creato, ora perché non inserisci anche gli altri dati?";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Title"] = "Nuovo beneficiario";
                return View(inputModel); 
            }
              
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Aggiorna Beneficiario";
            var inputModel = await service.GetBeneficiarioForEditingAsync(id);
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpPost]
        public async Task<IActionResult> Edit(BeneficiarioEditInputModel inputModel)
        {
            if(ModelState.IsValid)
            {
                    if (await service.IsBeneficiarioAvailableAsync(inputModel.Denominazione, inputModel.IdBeneficiario))
                    {
                        BeneficiarioDetailViewModel viewModel = await service.EditBeneficiarioAsync(inputModel);
                        TempData["Message"] = "Aggiornamento effettuato correttamente";
                        return RedirectToAction(nameof(Detail),new {id=inputModel.IdBeneficiario});
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(BeneficiarioEditInputModel.Denominazione), "Questo beneficiario già esiste");
                        ViewData["Title"] = "Aggiorna beneficiario";
                        return View(inputModel); 
                    }
            }
            else
            {
                ViewData["Title"] = "Aggiorna beneficiario";
                return View(inputModel); 
            }
        }
        [AuthorizeRole(Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            BeneficiarioDeleteInputModel inputModel = new BeneficiarioDeleteInputModel();
            inputModel.IdBeneficiario=id;
            
            if(ModelState.IsValid)
            {
                await service.DeleteBeneficiarioAsync(inputModel);
                TempData["Message"] = "Cancellazione effettuata correttamente";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Title"] = "Elimina beneficiario";
                return View(inputModel); 
            }
              
        }
        public async Task<IActionResult> IsBeneficiaryAvailable(string denominazione, int id=0)
        {
            bool result = await service.IsBeneficiarioAvailableAsync(denominazione, id);
            return Json(result);
        }
    }
}