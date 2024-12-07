
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Scadenze.Controllers
{
     [Authorize]
    public class HomeController : Controller
    {
        [ResponseCache(CacheProfileName="Home")]
        public IActionResult Index()
        {
            ViewData["Title"]="Home Page".ToUpper();
            return View();
        }  
        public IActionResult Privacy()
        {
            ViewData["Title"]="Home Page".ToUpper();
            return View();
        }  
    }
}
