using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace FFCEI.Microservices.AspNetCore.Swagger;

#pragma warning disable CA1812
internal sealed class MessageAttributeSchemaFilter : ISchemaFilter
#pragma warning restore CA1812
{
    private readonly XDocument? _xmlComments;

    public MessageAttributeSchemaFilter(string xmlPath)
    {
        if (File.Exists(xmlPath))
        {
            _xmlComments = XDocument.Load(xmlPath);
        }
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (_xmlComments is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(schema, nameof(schema));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var attribute = Attribute.GetCustomAttribute(context.Type, typeof(SwaggerRequestAttribute), true);

        if (attribute is not null)
        {
            schema.Title = $"{context.Type.Name} ({context.Type.AssemblyQualifiedName})";
        }
    }
}
