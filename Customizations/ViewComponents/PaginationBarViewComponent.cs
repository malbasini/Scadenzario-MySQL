using Microsoft.AspNetCore.Mvc;

namespace MyCourse.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        //public IViewComponentResult Invoke(CourseListViewModel model)
        public IViewComponentResult Invoke(IPaginationInfo model)
        {
            /*--Il parametro del metodo Invoke deve fornirci le seguenti
             informazioni. 
             
             //Il numero di pagina corrente
             //Il numero di risultati totali
             //Il numero di risultati per pagina
             //Search, OrderBy e Ascending
             
             
             Tuttavia questa soluzione non è soddisfacente,
             abbiamo detto che questo View Component dovrà essere riutilizzato
             in altri parti di codice, facendolo dipendere da CourseListViewModel
             questa soluzione non è praticabile
            */
            
            
            return View(model);
        }
    }
}