using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

internal class StackTraceConverter : JsonConverter<StackTrace>
{
    public override StackTrace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return default;
    }

    public override void Write(
        Utf8JsonWriter writer,
        StackTrace value,
        JsonSerializerOptions options
        )
    {
        writer.WriteStringValue(value.ToFriendlyStackTrace());
    }
}
