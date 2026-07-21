using System.Text;
using System.Text.Json;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectJsonFormatter
{
    static JsonSerializerOptions logOptions;

    static ObjectJsonFormatter()
    {
        logOptions = JsonSerializerInstance.GetJsonLogOptions();
    }

    internal static StringBuilder Format(object obj, ObjectFormatterOptions options)
    {
        return new StringBuilder(System.Text.Json.JsonSerializer.Serialize(obj, logOptions));
    }
}
