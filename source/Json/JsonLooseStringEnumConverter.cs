using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.Json;

internal sealed class JsonLooseStringEnumConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (typeToConvert is null)
        {
            throw new ArgumentNullException(nameof(typeToConvert));
        }

        return typeToConvert.IsEnum;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Activator.CreateInstance(typeof(DictionaryValueConverterInner<>).MakeGenericType(new Type[] { typeToConvert })) as JsonConverter;
    }

#pragma warning disable CA1812
    private sealed class DictionaryValueConverterInner<TValue> : JsonConverter<TValue> where TValue : struct, Enum
#pragma warning restore CA1812
    {
        public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var numberValue = reader.GetInt64();
                var numberString = numberValue.ToString(CultureInfo.InvariantCulture);

                if (Enum.TryParse(numberString, false, out TValue value) || Enum.TryParse(numberString, true, out value))
                {
                    return value;
                }

                try
                {
                    return (TValue)(object)numberValue;
                }
                catch (InvalidCastException)
                {
                    throw new JsonException($"Unable to convert \"{numberValue}\" to Enum \"{typeof(TValue)}\".");
                }
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var valueString = reader.GetString();

                if (Enum.TryParse(valueString, false, out TValue value) || Enum.TryParse(valueString, true, out value))
                {
                    return value;
                }

                try
                {
                    if (valueString is not null)
                    {
                        return (TValue)(object)valueString;
                    }
                }
                catch (InvalidCastException)
                {
                    throw new JsonException($"Unable to convert \"{valueString}\" to Enum \"{typeof(TValue)}\".");
                }
            }

            throw new JsonException($"Unable to convert to Enum \"{typeof(TValue)}\".");
        }

        public override void Write(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options) =>
            writer.WriteNumberValue((long)Convert.ChangeType(Convert.ChangeType(value, value.GetTypeCode(), CultureInfo.InvariantCulture), typeof(long), CultureInfo.InvariantCulture));
    }
}
