using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static class LogFormatter
{
    static ObjectFormatterOptions FormatOptions;

    static LogFormatter()
    {
        FormatOptions = new ObjectFormatterOptions();
        FormatOptions.ExcludeNullMembers = true;
        FormatOptions.MaxLevel = 5;
        FormatOptions.Format = LogInstance.Format.ToEnum<ObjectFormatterFormat>();
    }

    public static string Format(LogMessage message)
    {
        return ObjectFormatter.Format(message, FormatOptions);
    }
}
