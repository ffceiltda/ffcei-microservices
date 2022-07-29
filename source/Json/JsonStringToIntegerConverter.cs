using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.Json;

internal sealed class JsonStringToIntegerConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(int);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new StringToLongConverterInner();

    private sealed class StringToLongConverterInner : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var numberValue = reader.GetInt32();

                return numberValue;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var valueString = reader.GetString();

                if (int.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }

                throw new JsonException($"Unable to convert \"{valueString}\" to \"{typeof(int)}\".");

            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
    }
}
