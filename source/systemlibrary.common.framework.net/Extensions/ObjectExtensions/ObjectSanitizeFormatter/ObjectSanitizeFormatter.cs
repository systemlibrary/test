
using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectSanitizedFormatter
{
    internal static StringBuilder Format(object message, ObjectSanitizedFormat format, ObjectSanitizedFormatOptions options)
    {
        options = options ?? new ObjectSanitizedFormatOptions();

        if(format == ObjectSanitizedFormat.Json)
        {
            return ObjectSanitizedJsonFormatter.Format(message, options);
        }

        if(format == ObjectSanitizedFormat.Plain)
        {
            return ObjectSanitizedPlainFormatter.Format(message, options);
        }

        throw new Exception("Format " + ((int)format) + " is not supported, must be 0-1");
    }
}
