using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace FFCEI.Microservices.AspNetCore.Swagger
{
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
            if (_xmlComments == null)
            {
                return;
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (Attribute.GetCustomAttribute(context.Type, typeof(SwaggerMessageAttribute), false) != null)
            {
                schema.Title = context.Type.Name;
            }
        }
    }
}
