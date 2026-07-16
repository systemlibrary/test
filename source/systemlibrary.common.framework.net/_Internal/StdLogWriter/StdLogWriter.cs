namespace SystemLibrary.Common.Framework;

internal class StdLogWriter : ILogWriter
{
    public void Write(LogMessage message)
    {
        var text = LogFormatter.Format(message).Append(Environment.NewLine);

        var startsWithTab = text?.Length > 1 && text[0] == '\t';

        if (Log.SupportsAnsi)
        {
            var color = "\u001b[92m";

            if (startsWithTab)
            {
                color = "\u001b[96m";

                Console.WriteLine(color + text + "\u001b[0m");
            }
            else
            {
                Console.WriteLine(color + "\u001b[0m" + text);
            }
        }
        else
        {
            if (startsWithTab)
            {
                Console.WriteLine(text);
            }
            else
            {
                var topic = "[" + message.Level + "] ";

                Console.WriteLine(text);
            }
        }
    }
}
