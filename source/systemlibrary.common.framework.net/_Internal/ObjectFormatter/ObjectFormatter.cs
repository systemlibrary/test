using System.Text;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectFormatter
{
    internal static StringBuilder Format(object message, ObjectFormatterOptions options)
    {
        options = options ?? new ObjectFormatterOptions();

        var format = options.Format;

        if (format == ObjectFormatterFormat.Json)
        {
            return ObjectJsonFormatter.Format(message, options);
        }

        if (format == ObjectFormatterFormat.Plain)
        {
            return ObjectPlainTextFormatter.Format(message, options);
        }

        if (format == ObjectFormatterFormat.NDJson)
        {
            return ObjectNDJsonFormatter.Format(message, options);
        }

        throw new Exception("Format " + ((int)format) + " is not supported, must be 0-1");
    }
}
