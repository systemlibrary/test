using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class StdLogWriter : ILogWriter
{
    public void Write(LogMessage message)
    {
        var text = LogFormatter.Format(message);

        if (Log.SupportsAnsi)
        {
            var sb = new StringBuilder(text);
            switch (message.Level)
            {
                case LogLevel.Debug:
                case LogLevel.Dump:
                    sb.AppendAnsiColor(AnsiColor.Cyan);
                    break;

                case LogLevel.Information:
                    sb.AppendAnsiColor(AnsiColor.Green);
                    break;

                case LogLevel.Warning:
                    sb.AppendAnsiColor(AnsiColor.Yellow);
                    break;

                case LogLevel.Error:
                    sb.AppendAnsiColor(AnsiColor.Red);
                    break;

                case LogLevel.Critical:
                    sb.AppendAnsiColor(AnsiColor.DarkRed);
                    break;
            }
            text = sb.ToString();
        }

        if (message.Level > LogLevel.Warning)
        {
            Console.Error.WriteLine(text);
        }
        else
        {
            Console.Out.WriteLine(text);
        }
    }
}
