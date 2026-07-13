using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectFormatter
{
    internal static StringBuilder Format(object message, ObjectFormatterFormat format, ObjectFormatterOptions options)
    {
        options = options ?? new ObjectFormatterOptions();

        if(format == ObjectFormatterFormat.Json)
        {
            return ObjectJsonFormatter.Format(message, options);
        }

        if(format == ObjectFormatterFormat.Plain)
        {
            return ObjectPlainTextFormatter.Format(message, options);
        }

        throw new Exception("Format " + ((int)format) + " is not supported, must be 0-1");
    }
}
