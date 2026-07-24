using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Framework.Bootstrap;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(LogMessage message)
    {
        try
        {
            return ObjectFormatter.Format(message, FormatOptions);
        }
        catch(Exception ex)
        {
            if (LogInstance.Format == LogFormat.Text)
                return message.Level + " " + message.Message + ". Exception thrown during log formatting: " + ex.ToString();

            else if (LogInstance.Format == LogFormat.NDJson)
                return "{\"Level\":\"" + message.Level + "\",\"Message\":\"" + message.Level + " " + message.Message + ". Exception thrown during log formatting: " + ex.ToString().Replace(Environment.NewLine, "") + "\"}";

            else
                return "{\n\t\"Level\":\"" + message.Level + "\",\n\t\"Message\":\"" + message.Level + " " + message.Message + ". Exception thrown during log formatting: " + ex.ToString().Replace(Environment.NewLine, "") + "\"\n}";
        }
    }
}
