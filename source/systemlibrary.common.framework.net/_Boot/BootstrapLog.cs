namespace SystemLibrary.Common.Framework.Bootstrap;

internal class BootstrapLog
{
    public static void Write(object o)
    {
        var m = "[BootLog] " + o + "\n";

        Console.Out.WriteLine(m);

        System.IO.File.AppendAllText(@"C:\logs\log.txt", m);
    }
}