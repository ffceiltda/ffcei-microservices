using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.Json
{
    public static class JsonSerializerOptionsExtensionMethods
    {
        public static JsonSerializerOptions ConfigureJsonSerializerOptions(this JsonSerializerOptions options, bool writeIndented, bool ignoreNullsOrSerialization)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            options.WriteIndented = writeIndented;
            options.DefaultIgnoreCondition = ignoreNullsOrSerialization ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never;
            options.PropertyNameCaseInsensitive = true;

            options.Converters.Add(new JsonTrimmingConverter());
            options.Converters.Add(new JsonLooseStringEnumConverter());
            options.Converters.Add(new JsonStringToDecimalConverter());
            options.Converters.Add(new JsonStringToLongConverter());
            options.Converters.Add(new JsonStringToIntegerConverter());

            return options;
        }

        public static JsonSerializerOptions WebApiOptions { get; private set; } = (new JsonSerializerOptions()).ConfigureJsonSerializerOptions(true, true);
    }
}
