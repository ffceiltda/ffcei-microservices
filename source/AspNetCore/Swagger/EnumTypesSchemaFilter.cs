using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace FFCEI.Microservices.AspNetCore.Swagger;

#pragma warning disable CA1812
internal sealed class EnumTypesSchemaFilter : ISchemaFilter
#pragma warning restore CA1812
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
        if (_xmlComments is null)
        {
            return;
        }

        if (schema is null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        if (context is null)
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

        if (fullTypeName is null)
        {
            return;
        }

        Type? enumType = null;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            enumType = assembly.GetTypes().Where(t => (t.FullName is not null) && t.FullName.Contains(fullTypeName, StringComparison.InvariantCulture)).FirstOrDefault();

            if (enumType is not null)
            {
                break;
            }
        }

        if (enumType is null)
        {
            return;
        }

        foreach (var enumMemberValue in schema.Enum.OfType<OpenApiInteger>().Select(v => v.Value))
        {
            var enumValue = Convert.ToInt32(enumMemberValue);
            var enumMemberName = Enum.GetName(enumType, enumValue);
            var enumMemberFullName = $"F:{fullTypeName}.{enumMemberName}";
            var enumMemberComments = _xmlComments.Descendants("member").FirstOrDefault(m =>
            {
                var attribute = m.Attribute("name");

                if (attribute is null)
                {
                    return false;
                }

                return attribute.Value.Equals(enumMemberFullName, StringComparison.OrdinalIgnoreCase);
            });

            var summary = enumMemberComments?.Descendants("summary").FirstOrDefault();

            if (summary is null)
            {
                continue;
            }

            schema.Description += $"<li><i>{enumMemberValue}</i> - {summary.Value.Trim()}</li>";
        }

        schema.Description += "</ul>";
    }
}
