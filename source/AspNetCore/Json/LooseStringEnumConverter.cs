using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore.Json
{
    public class LooseStringEnumConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == null)
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
        {
            public override TValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    var numberValue = reader.GetInt64();

                    return (TValue)(object)numberValue;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var valueString = reader.GetString();

                    if (Enum.TryParse(valueString, false, out TValue value) || Enum.TryParse(valueString, true, out value))
                    {
                        return value;
                    }

                    throw new JsonException($"Unable to convert \"{valueString}\" to Enum \"{typeof(TValue)}\".");
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options) => writer.WriteNumberValue((int)(object)value);
        }
#pragma warning restore CA1812
    }
}
