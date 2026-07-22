using System.Runtime.CompilerServices;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectFormatter
{
    static bool InvalidFormatErrorLogged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string Format(object message, ObjectFormatterOptions options)
    {
        var format = options.Format;

        if (format == ObjectFormatterFormat.Json)
        {
            return ObjectJsonFormatter.Format(message, options);
        }

        if (format == ObjectFormatterFormat.Plain)
        {
            return ObjectPlainTextFormatter.Format(message, options).ToString();
        }

        if (format == ObjectFormatterFormat.NDJson)
        {
            return ObjectNDJsonFormatter.Format(message, options);
        }

        if (!InvalidFormatErrorLogged)
        {
            InvalidFormatErrorLogged = true;

            var supported = EnumExtensions<ObjectFormatterFormat>.GetKeys();

            FrameworkLog.Error("Format " + ((int)format) + " is not supported. Uses fallback. Supported ones are: " + string.Join(", ", supported));
        }

        return ObjectPlainTextFormatter.Format(message, options).ToString();
    }
}
