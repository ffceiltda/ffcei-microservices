using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore.Json
{
    sealed class StringToLongConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(long);

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new StringToLongConverterInner();
        }

        private sealed class StringToLongConverterInner : JsonConverter<long>
        {
            public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    var numberValue = reader.GetInt64();

                    return numberValue;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var valueString = reader.GetString();

                    if (long.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                    {
                        return value;
                    }

                    throw new JsonException($"Unable to convert \"{valueString}\" to \"{typeof(long)}\".");

                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }
    }
}
