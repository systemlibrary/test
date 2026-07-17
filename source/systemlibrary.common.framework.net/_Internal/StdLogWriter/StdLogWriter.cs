using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class StdLogWriter : ILogWriter
{
    public void Write(LogMessage message)
    {
        var text = LogFormatter.Format(message);

        if (Log.SupportsAnsi)
        {
            switch (message.Level)
            {
                case LogLevel.Debug:
                case LogLevel.Dump:
                    text.AppendAnsiColor(AnsiColor.Cyan);
                    break;

                case LogLevel.Information:
                    text.AppendAnsiColor(AnsiColor.Green);
                    break;

                case LogLevel.Warning:
                    text.AppendAnsiColor(AnsiColor.Yellow);
                    break;

                case LogLevel.Error:
                    text.AppendAnsiColor(AnsiColor.Red);
                    break;

                case LogLevel.Critical:
                    text.AppendAnsiColor(AnsiColor.DarkRed);
                    break;
            }
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
