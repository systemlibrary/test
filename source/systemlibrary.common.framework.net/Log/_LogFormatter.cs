using System.Text;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static class LogFormatter
{
    static ObjectFormatterOptions Options;

    const int TextTimestampOffset = 11;
    const int TextTimestampLength = 35;

    const int JsonTimestampOffset = 22;
    const int JsonTimestampLength = 53;

    const int NDJsonTimestampOffset = 17;
    const int NDJsonTimestampLength = 48;

    static LogFormatter()
    {
        Options = new ObjectFormatterOptions();
        Options.ExcludeNullMembers = true;
        Options.MaxLevel = 5;

        Options.Format = LogInstance.Format.ToEnum<ObjectFormatterFormat>();
        //Framework.Debug("Unsupported format in systemLibraryCommonFramework:log:format, fallback to plain text");
    }

    public static StringBuilder Format(LogMessage message)
    {
        if (message.Level == LogLevel.Dump)
        {
            var msg = ObjectFormatter.Format(message, Options);

            if (LogInstance.Format == LogFormat.Text)
            {
                msg.Remove(TextTimestampOffset, TextTimestampLength);
            }
            else if(LogInstance.Format == LogFormat.Json)
            {
                msg.Remove(JsonTimestampOffset, JsonTimestampLength);
            }
            else if(LogInstance.Format == LogFormat.NDJson)
            {
                msg.Remove(NDJsonTimestampOffset, NDJsonTimestampLength);
            }
            return msg;
        }
        return ObjectFormatter.Format(message, Options);
    }
}
