using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Use to convert a DateTime into a string date 'yyyy-MM-dd', ignoring time
/// </summary>
/// <example>
/// Example:
/// <code>
/// var options = new JsonSerializationOptions();
/// options.Converters.Add(new DateJsonConverter());
/// 
/// var user = new User();
/// user.Birth = DateTime.Christmas()
/// 
/// var json = user.Json(options)
/// // json.Birth is now a string "yyyy-12-24"
/// </code>
/// Example on a specific property:
/// <code>
/// [JsonConverter(typeof(DateJsonConverter))]
/// public DateTime Birth {get;set;}
/// </code>
/// </example>
public class DateJsonConverter : JsonConverter<DateTime>
{
    public DateJsonConverter()
    {
    }

    public static string SetType()
    {
        return "";
    }

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString("yyyy-MM-dd"));
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString().ToDateTime("yyyy-MM-dd");
    }
}
