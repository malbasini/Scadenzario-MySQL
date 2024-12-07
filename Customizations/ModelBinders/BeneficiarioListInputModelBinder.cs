using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Scadenzario.Models.InputModels.Beneficiari;
using Scadenzario.Models.Options;

namespace Scadenzario.Customizations.ModelBinders
{
    public class BeneficiarioListInputModelBinder : IModelBinder
    {
        private readonly IOptionsMonitor<BeneficiariOptions> _beneficiarioOptions;
        public BeneficiarioListInputModelBinder(IOptionsMonitor<BeneficiariOptions> beneficiarioOptions)
        {
            _beneficiarioOptions = beneficiarioOptions;
        }
        /*--
         Il metodo "BindModelAsync" è definito nella classe "BeneficiarioListInputModelBinder" che implementa 
         l'interfaccia "IModelBinder". Questa classe è uno strumento di associazione di modelli personalizzato, 
         utilizzato da ASP.NET Core per associare i dati della richiesta HTTP ai parametri delle Action.
         In questo metodo, `bindingContext.ValueProvider` fornisce valori dalla richiesta HTTP. 
         I dati della richiesta vengono recuperati e trasformati in tipologie adeguate. Quindi, 
         con questi dati viene creata una nuova istanza di `ScadenzaListInputModel`. Questa nuova istanza viene 
         quindi impostata come risultato per il contesto di associazione per indicare che il modello è stato 
         associato correttamente. Infine, il metodo restituisce "Task.CompletedTask" perché è un metodo asincrono, 
         ma in questo caso non viene eseguito alcun lavoro asincrono effettivo.
         Infine, questo raccoglitore di modelli personalizzati viene utilizzato nella classe 
         "CourseListInputModel" utilizzando l'attributo "ModelBinder".
         Pertanto, quando ASP.NET Core rileva un parametro di tipo "ScadenzaListInputModel" in una Action, 
         utilizzerà "BeneficiarioListInputModelBinder" per associare i dati della richiesta HTTP a tale parametro.
         */
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Recuperiamo i valori grazie ai value provider
            string? search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string? orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
            bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);

            //Creiamo l'istanza del ScadenzaListInputModel
            BeneficiariOptions options = _beneficiarioOptions.CurrentValue;
            var inputModel = new BeneficiarioListInputModel(search, page, orderBy, ascending, (int)options.PerPage, options.Order);
            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result = ModelBindingResult.Success(inputModel);
            //Restituiamo un task completato
            return Task.CompletedTask;
        }
    }
}