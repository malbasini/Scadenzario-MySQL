using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scadenzario.Customizations.Authorization;
using Scadenzario.Models.Enums;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Controllers
{
    [Authorize]
    public class ScadenzeController : Controller
    {
        private readonly IScadenzeService _service;
        private readonly IRicevuteService _ricevute;
        public ScadenzeController(ICachedScadenzeService service, IRicevuteService ricevute)
        {
            _service = service;
            _ricevute = ricevute;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(ScadenzaListInputModel input)
        {
            ViewData["Title"] = "Lista Scadenze";
            ListViewModel<ScadenzaViewModel> scadenze = await _service.GetScadenzeAsync(input);

            ScadenzaListViewModel viewModel = new ScadenzaListViewModel
            {
                Scadenze = scadenze,
                Input = input
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Dettaglio Scadenza";
            ScadenzaDetailViewModel viewModel = await _service.GetScadenzaAsync(id);
            viewModel.Ricevute = _ricevute.GetRicevute(id);
            return View(viewModel);
        }
        [HttpGet]
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuova Scadenza";
            ScadenzaCreateInputModel inputModel = new ScadenzaCreateInputModel();
            inputModel.DataScadenza = DateTime.Now;
            inputModel.Beneficiari = _service.GetBeneficiari();
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [ValidateReCaptcha]
        [HttpPost]
        public async Task<IActionResult> Create(ScadenzaCreateInputModel inputModel)
        {
            inputModel.Beneficiari = _service.GetBeneficiari();
            if(ModelState.IsValid)
            {
                await _service.CreateScadenzaAsync(inputModel);
                TempData["ConfirmationMessage"] = "Ok! la tua scadenza è stata creata, ora perché non inserisci anche gli altri dati?";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(allErrors);
                ViewData["Title"] = "Nuova Scadenza";
                return View(inputModel); 
            }
              
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TempData["IDScadenza"]=id; 
            ViewData["Title"] = "Aggiorna Scadenza";
            ScadenzaEditInputModel inputModel = new ScadenzaEditInputModel();
            inputModel = await _service.GetScadenzaForEditingAsync(id);
            inputModel.Denominazione=_service.GetBeneficiarioById(inputModel.IdBeneficiario);
            inputModel.Beneficiari = _service.GetBeneficiari();
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpPost]
        public async Task<IActionResult> Edit(ScadenzaEditInputModel inputModel)
        {
            inputModel.Denominazione=_service.GetBeneficiarioById(inputModel.IdBeneficiario);
            if (ModelState.IsValid)
            {
                if(inputModel.DataPagamento.HasValue)
                    inputModel.GiorniRitardo=_service.DateDiff(inputModel.DataScadenza,inputModel.DataPagamento.Value);
                else
                    inputModel.GiorniRitardo=_service.DateDiff(inputModel.DataScadenza,DateTime.Now.Date);   
                await _service.EditScadenzaAsync(inputModel);
                TempData["Message"] = "Aggiornamento effettuato correttamente".ToUpper();
                return RedirectToAction(nameof(Index),"Scadenze");
            }
            else
            {
                ViewData["Title"] = "Aggiorna Scadenza".ToUpper();
                inputModel.Beneficiari = _service.GetBeneficiari();
                return View(inputModel);
            }

        }
        [AuthorizeRole(Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ScadenzaDeleteInputModel inputModel = new ScadenzaDeleteInputModel();
            inputModel.IdScadenza=id;
            if(ModelState.IsValid)
            {
                await _service.DeleteScadenzaAsync(inputModel);
                TempData["Message"] = "Cancellazione effettuata correttamente";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Title"] = "Elimina scadenza";
                return View(inputModel); 
            }
              
        }
    }
}