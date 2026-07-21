using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// JsonConverter for DateTime with a custom format, for use with <c>Json()</c> extension method options.
/// </summary>
/// <remarks>
/// Exposed for cases where the built-in format support is insufficient.
/// </remarks>
/// <example>
/// <code>
/// var options = new JsonSerializerOptions();
/// options.Converters.Add(new DateTimeJsonConverter("yyyy/MM/dd HH:mm"));
/// var result = json.Json&lt;MyType&gt;(options);
/// </code>
/// </example>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    string Format;

    public DateTimeJsonConverter(string format = null)
    {
        Format = format;
    }

    /// <summary>Serializes a DateTime using the configured format.</summary>
    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(Format));
    }

    /// <summary>Deserializes a DateTime string using the configured format.</summary>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTime(Format);
    }
}
