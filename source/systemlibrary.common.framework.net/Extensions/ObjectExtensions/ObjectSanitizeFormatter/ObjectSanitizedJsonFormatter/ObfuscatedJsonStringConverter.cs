using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Extensions;

internal class ObjectSanitizedStringJsonConverter : JsonConverter<string>
{
    public ObjectSanitizedStringJsonConverter()
    {
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString();

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value != null) value = value.Obfuscate();

        writer.WriteStringValue(value);
    }
}