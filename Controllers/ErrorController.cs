
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.Exceptions.Application;
using Scadenze.Models.Exceptions.Application;


namespace Scadenzario.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            switch(feature.Error)
            {
                case BeneficiarioNotFoundException exc:
                    ViewData["Title"] = "Beneficiario non trovato";
                    Response.StatusCode = 404;
                    return View();

                case ScadenzaNotFoundException exc:
                    ViewData["Title"] = "Scadenza non trovata";
                    Response.StatusCode = 404;
                    return View();

                case UserUnknownException exc:
                    ViewData["Title"] = "Utente sconosciuto";
                    Response.StatusCode = 400;
                    return View();

                case SendException exc:
                    ViewData["Title"] = "Non è stato possibile inviare il messaggio, riprova più tardi";
                    Response.StatusCode = 500;
                    return View();

                default:
                    ViewData["Title"] = "Errore";
                    return View();
            }
        }
    }
}