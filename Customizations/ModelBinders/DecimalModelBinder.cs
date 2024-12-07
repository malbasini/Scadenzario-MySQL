using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Scadenze.Customizations.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal decimalValue)) 
            {
                bindingContext.Result = ModelBindingResult.Success(decimalValue);
            }
            return Task.CompletedTask;
        }
    }
}