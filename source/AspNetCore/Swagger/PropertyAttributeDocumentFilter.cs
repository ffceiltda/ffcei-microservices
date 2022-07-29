using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore.Swagger;

#pragma warning disable CA1812
internal sealed class PropertyAttributeDocumentFilter : IDocumentFilter
#pragma warning restore CA1812
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc is null)
        {
            throw new ArgumentNullException(nameof(swaggerDoc));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach (var operation in path.Operations.Values.Where(op => op.Parameters is not null && op.Parameters.Any()))
            {
                foreach (var parameter in operation.Parameters)
                {
                    var schemaReferenceId = parameter.Schema.Reference?.Id;

                    if (string.IsNullOrEmpty(schemaReferenceId))
                    {
                        continue;
                    }

                    var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                    if ((schema.Enum is null) || (schema.Enum.Count == 0))
                    {
                        continue;
                    }

                    parameter.Description = "<p>Variants:</p>";

                    int cutStart = schema.Description.IndexOf("<ul>", StringComparison.InvariantCulture);
                    int cutEnd = schema.Description.IndexOf("</ul>", StringComparison.InvariantCulture) + 5;

                    parameter.Description += schema.Description[cutStart..cutEnd];
                }
            }

            foreach (var operation in path.Operations.Values.Where(x => x.RequestBody is not null))
            {
                foreach (var content in operation.RequestBody.Content)
                {
                    var schemaReferenceId = content.Value.Schema.Reference?.Id;

                    if (string.IsNullOrEmpty(schemaReferenceId))
                    {
                        continue;
                    }

                    var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                    if (string.IsNullOrEmpty(schema.Title))
                    {
                        continue;
                    }

                    var lastOpenParenthesis = schema.Title.LastIndexOf("(", StringComparison.InvariantCulture);
                    var lastCloseParenthesis = schema.Title.LastIndexOf(")", StringComparison.InvariantCulture);

                    if (lastOpenParenthesis == -1 || lastCloseParenthesis == -1)
                    {
                        continue;
                    }

                    var title = schema.Title.Substring(0, lastOpenParenthesis - 1).TrimEnd();

                    int length = lastCloseParenthesis - lastOpenParenthesis - 1;

                    if (length < 1)
                    {
                        schema.Title = title;

                        continue;
                    }

                    var schemaType = schema.Title.Substring(lastOpenParenthesis + 1, lastCloseParenthesis - lastOpenParenthesis - 1);

                    schema.Title = title;

                    if (schemaType is null)
                    {
                        continue;
                    }

                    var type = Type.GetType(schemaType);

                    if (type is null)
                    {
                        continue;
                    }

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.Never
                    };

                    var attribute = Attribute.GetCustomAttribute(type, typeof(SwaggerRequestAttribute), true);

                    if (attribute is not null)
                    {
                        content.Value.Examples = new ConcurrentDictionary<string, OpenApiExample>();

                        dynamic miniObject = new ExpandoObject();

                        var fullObject = Activator.CreateInstance(type);
                        var requiredProperties = type.GetProperties().Where(prop => prop.IsDefined(typeof(SwaggerRequiredPropertyAttribute), true));
                        var expandoDictionary = miniObject as IDictionary<string, object>;

#pragma warning disable CA1508 // Avoid dead conditional code
                        if (expandoDictionary is null)
                        {
                            continue;
                        }
#pragma warning restore CA1508 // Avoid dead conditional code

                        foreach (var requiredProperty in requiredProperties)
                        {
                            expandoDictionary.Add(requiredProperty.Name, requiredProperty.PropertyType.GetDefaultValue());
                        }

                        /*
                        TODO: fixup response generator to include all fields, and handle array types
                        
                        content.Value.Examples.Add("Complete Request Schema", new OpenApiExample { Value = new OpenApiString(JsonSerializer.Serialize(fullObject, options)) });

                        if (expandoDictionary.Any())
                        {
                            content.Value.Examples.Add("Minimal Required Schema", new OpenApiExample { Value = new OpenApiString(JsonSerializer.Serialize(miniObject, options)) });
                        }
                        */
                    }
                }
            }
        }
    }
}
