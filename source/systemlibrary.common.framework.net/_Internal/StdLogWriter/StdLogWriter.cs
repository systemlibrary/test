namespace SystemLibrary.Common.Framework;

internal class StdLogWriter : ILogWriter
{
    public void Write(LogMessage message)
    {
        var text = LogFormatter.Format(message);

        Console.Out.WriteLine(text);


        //var topic = "[Common.Framework]";
        //var color = "\u001b[92m";
        //var hasTab = message.StartsWith("\t");

        //if (hasTab)
        //{
        //    topic = "";
        //    color = "\u001b[96m";
        //}

        //if (Log.SupportsAnsi)
        //{
        //    if (hasTab)
        //    {
        //        Console.WriteLine(color + message + "\u001b[0m");
        //    }
        //    else
        //    {
        //        Console.WriteLine(color + topic + "\u001b[0m " + message);
        //    }
        //}
        //else
        //{
        //    if (hasTab)
        //    {
        //        Console.WriteLine(message);
        //    }
        //    else
        //    {
        //        Console.WriteLine(topic + " " + message);
        //    }
        //}
    }
}
