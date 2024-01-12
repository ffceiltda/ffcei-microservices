using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FFCEI.Microservices.AspNetCore.ModelBindProviders;

public class FormDataJsonModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Metadata.IsComplexType)
        {
            return null;
        }

        var propertyName = context.Metadata.PropertyName;

        if (propertyName == null)
        {
            return null;
        }

        var propertyInfo = context.Metadata.ContainerType?.GetProperty(propertyName);

        if (propertyInfo == null)
        {
            return null;
        }

        if (propertyInfo.PropertyType.IsAssignableFrom(typeof(IFormFile)))
        {
            return null;
        }

        if (propertyInfo.GetCustomAttributes(typeof(FromFormAttribute), false).Length == 0)
        {
            return null;
        }

        return new FormDataJsonModelBinder();
    }
}
