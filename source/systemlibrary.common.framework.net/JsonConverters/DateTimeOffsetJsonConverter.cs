using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// JsonConverter for DateTimeOffset with a custom format, for use with <c>Json()</c> extension method options.
/// </summary>
/// <remarks>
/// Exposed for cases where the built-in format support is insufficient.
/// </remarks>
/// <example>
/// <code>
/// var options = new JsonSerializerOptions();
/// options.Converters.Add(new DateTimeOffsetJsonConverter("yyyy/MM/dd HH:mm"));
/// var result = json.Json&lt;MyType&gt;(options);
/// </code>
/// </example>
public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    string Format;

    public DateTimeOffsetJsonConverter(string format)
    {
        Format = format;
    }

    /// <summary>Serializes a DateTimeOffset using the configured format.</summary>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(Format));
    }

    /// <summary>Deserializes a DateTimeOffset string using the configured format.</summary>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTimeOffset(Format);
    }
}
