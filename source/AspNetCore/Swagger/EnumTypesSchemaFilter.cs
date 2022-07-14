using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace FFCEI.Microservices.AspNetCore.Swagger
{
    public class EnumTypesSchemaFilter : ISchemaFilter
    {
        private readonly XDocument? _xmlComments;

        public EnumTypesSchemaFilter(string xmlPath)
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

            if ((schema.Enum is not { Count: > 0 }) ||
                (context.Type is not { IsEnum: true }))
            {
                return;
            }

            schema.Description += "<p>Members:</p><ul>";

            var fullTypeName = context.Type.FullName;

            if (fullTypeName == null)
            {
                return;
            }

            Type? enumType = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                enumType = assembly.GetTypes().Where(t => (t.FullName != null) && t.FullName.Contains(fullTypeName, StringComparison.InvariantCulture)).FirstOrDefault();

                if (enumType != null)
                {
                    break;
                }
            }

            if (enumType == null)
            {
                return;
            }

            foreach (var enumMemberValue in schema.Enum.OfType<OpenApiInteger>().Select(v => v.Value))
            {
                var enumValue = Convert.ToInt32(enumMemberValue);
                var enumMemberName = Enum.GetName(enumType, enumValue);
                var enumMemberFullName = $"F:{fullTypeName}.{enumMemberName}";
                var enumMemberComments = _xmlComments.Descendants("member").FirstOrDefault(m => {
                    var attribute = m.Attribute("name");

                    if (attribute == null)
                    {
                        return false;
                    }

                    return attribute.Value.Equals(enumMemberFullName, StringComparison.OrdinalIgnoreCase);
                });

                var summary = enumMemberComments?.Descendants("summary").FirstOrDefault();

                if (summary == null)
                {
                    continue;
                }

                schema.Description += $"<li><i>{enumMemberValue}</i> - {summary.Value.Trim()}</li>";
            }

            schema.Description += "</ul>";
        }
    }
}
