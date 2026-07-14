using System.Text;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

public static class LogFormatter
{
    static ILogFormatter Formatter;

    static LogFormatter()
    {
        if (LogInstance.Format == LogFormat.Text)
            Formatter = new LogTextFormatter();
        else if (LogInstance.Format == LogFormat.Json)
            Formatter = new LogJsonFormatter();
    }

    public static string Format(LogMessage message)
    {
        Formatter.
    }

    static string FormatText(LogMessage message)
    {
        var builder = new StringBuilder();

        builder.Append(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        builder.Append(" ");

        if (SupportsAnsi)
        {
            var prefix = message.Level switch
            {
                LogLevel.Critical => "\u001b[38;5;196m[",
                LogLevel.Error => "\u001b[38;5;160m[",
                LogLevel.Warning => "\u001b[96m[",
                LogLevel.Debug => "\u001b[93m[",
                LogLevel.Trace => "\u001b[90m[",
                _ => "\u001b[92m["
            };

            builder.Append(prefix);
        }
        else
            builder.Append("[");

        builder.Append(message.Level.ToString().ToUpperInvariant());

        if (SupportsAnsi)
            builder.Append("]\u001b[0m ");
        else
            builder.Append("] ");

        if (message.Messages != null)
        {
            builder.Append(ObjectFormatter.Format(message.Messages, ObjectFormatterFormat.Plain, TextOptions).ToString());
        }
        else
        {
            builder.Append("(null)");
        }

        builder.AppendLine();

        if (message.CorrelationId != null)
        {
            builder.Append("CorrelationId: ");
            builder.AppendLine(message.CorrelationId);
        }

        if (message.IsAuthenticated.HasValue)
        {
            builder.Append("Authenticated: ");
            builder.AppendLine(message.IsAuthenticated.Value.ToString());
        }

        if (message.Url != null)
        {
            builder.Append("Url: ");
            builder.AppendLine(message.Url);
        }

        if (message.BrowserName != null)
        {
            builder.Append("Browser: ");
            builder.AppendLine(message.BrowserName);
        }

        if (message.StackTrace != null)
        {
            builder.Append("StackTrace:");
            builder.AppendLine(message.StackTrace.ToFriendlyStackTrace());
        }

        return builder.ToString();
    }

    static string FormatJson(LogMessage message)
    {
        return "";
    }

    internal static bool SupportsAnsi =
        !Console.IsOutputRedirected &&
        Environment.GetEnvironmentVariable("NO_COLOR") == null &&
        Environment.GetEnvironmentVariable("TERM") != "dumb";
}
