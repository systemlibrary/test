using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

public class LogJsonFormatter : ILogFormatter
{
    static ObjectFormatterOptions TextOptions;

    static LogJsonFormatter()
    {
        TextOptions = new ObjectFormatterOptions();
        TextOptions.ExcludeNullMembers = true;
        TextOptions.StartLevel = 0;
        TextOptions.MaxLevel = 5;
    }

    public static string Format(LogMessage message)
    {
    }

    internal static bool SupportsAnsi =
        !Console.IsOutputRedirected &&
        Environment.GetEnvironmentVariable("NO_COLOR") == null &&
        Environment.GetEnvironmentVariable("TERM") != "dumb";
}
