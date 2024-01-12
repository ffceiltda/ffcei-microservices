using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace FFCEI.Microservices.AspNetCore;

public class FormDataJsonModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var fieldName = bindingContext.FieldName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(fieldName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(fieldName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        try
        {
            var result = JsonConvert.DeserializeObject(value, bindingContext.ModelType);

            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}
