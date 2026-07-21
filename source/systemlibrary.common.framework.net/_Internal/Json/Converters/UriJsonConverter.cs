using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class UriJsonConverter : JsonConverter<Uri>
{
    public override Uri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (s.IsNot())
                return null;

            if (Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var uri))
                return uri;
        }

        throw new JsonException("Invalid Uri");
    }

    public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}