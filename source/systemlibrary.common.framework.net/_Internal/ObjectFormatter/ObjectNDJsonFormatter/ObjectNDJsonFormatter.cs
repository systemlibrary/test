using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectNDJsonFormatter
{
    static JsonSerializerOptions logOptions;

    static ObjectNDJsonFormatter()
    {
        logOptions = JsonSerializerInstance.GetNDJsonOptions();
    }

    // NOTE: Optimize, should support 'streams' instead of passing string into a stringbuilder somehow, to further adjust the stringbuilder and return the string, and with special "date converter"... to ignore time timestamp if 'Dump' is log level
    internal static StringBuilder Format(object obj, ObjectFormatterOptions options)
    {
        return new StringBuilder(System.Text.Json.JsonSerializer.Serialize(obj, logOptions));
    }
}
