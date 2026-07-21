using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class EncodingConverter : JsonConverter<Encoding>
{
    public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var encodingName = reader.GetString();

        if (encodingName.IsNot()) return Encoding.Default;

        return Encoding.GetEncoding(encodingName);
    }

    public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.WebName ?? Encoding.Default.WebName);
    }
}
