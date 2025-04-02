using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Scadenze.Customizations.ModelBinders
{
    /// <summary>
    /// A custom model binder for binding decimal values from the request.
    /// </summary>
    public class DecimalModelBinder : IModelBinder
    {
        /// <summary>
        /// Attempts to bind a model from the request to a decimal value.
        /// </summary>
        /// <param name="bindingContext">The context for model binding.</param>
        /// <returns>A completed task.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Retrieve the value from the value provider using the model name.
            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            
            // Try to parse the value as a decimal using the current culture's currency format.
            if (decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal decimalValue))
            {
                // If parsing is successful, set the binding result to success with the parsed decimal value.
                bindingContext.Result = ModelBindingResult.Success(decimalValue);
            }
            
            // Return a completed task.
            return Task.CompletedTask;
        }
    }
}