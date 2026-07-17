using System.Text;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

public static class LogFormatter
{
    static ObjectFormatterOptions Options;

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
        return ObjectFormatter.Format(message, Options);
    }
}
