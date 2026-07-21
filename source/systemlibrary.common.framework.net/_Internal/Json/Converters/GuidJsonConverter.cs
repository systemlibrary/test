using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class GuidJsonConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return Guid.Empty;

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (s.IsNot())
                return Guid.Empty;

            if (Guid.TryParse(s, out var g))
                return g;
        }

        throw new JsonException("Invalid Guid");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}