using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCEI.Microservices.AspNetCore.Json
{
    sealed class JsonTrimmingConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString()?.Trim();

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStringValue(value?.Trim());
        }
    }
}
