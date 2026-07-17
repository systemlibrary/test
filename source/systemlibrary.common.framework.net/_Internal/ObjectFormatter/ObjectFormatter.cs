using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectFormatter
{
    static bool InvalidFormatErrorLogged;

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

        if (!InvalidFormatErrorLogged)
        {
            InvalidFormatErrorLogged = true;
            var supported = EnumExtensions<ObjectFormatterFormat>.GetKeys();

            FrameworkLog.Error("Format " + ((int)format) + " is not supported. " + string.Join(", ", supported));
        }

        return ObjectPlainTextFormatter.Format(message, options);
    }
}
