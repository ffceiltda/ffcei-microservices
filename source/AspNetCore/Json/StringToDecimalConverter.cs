using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore.Json
{
    sealed class StringToDecimalConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(decimal);

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new StringToDecimalConverterInner();

        private sealed class StringToDecimalConverterInner : JsonConverter<decimal>
        {
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    var numberValue = reader.GetDecimal();

                    return numberValue;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var valueString = reader.GetString();

                    if (decimal.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                    {
                        return value;
                    }

                    throw new JsonException($"Unable to convert \"{valueString}\" to \"{typeof(decimal)}\".");

                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }
    }
}
