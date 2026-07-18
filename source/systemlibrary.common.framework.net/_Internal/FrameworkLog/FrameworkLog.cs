using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SystemLibrary.Common.Framework;

internal static class FrameworkLog
{
    static ConcurrentQueue<(LogLevel, string)> Messages = new ConcurrentQueue<(LogLevel, string)>();

    static volatile bool IsFlushed = false;

    internal static Exception Critical(object message)
    {
        try
        {
            Console.Error.WriteLine(message);
        }
        catch
        {
            // swallow
        }

        try
        {
            Add(LogLevel.Critical, message?.ToString());
        }
        catch
        {
            // swallow
        }

        if (message is Exception ex)
            return ex;

        return new Exception(message?.ToString() ?? "");
    }

    internal static void Error(string message)
    {
        Add(LogLevel.Error, message);
    }

    internal static void Warning(string message)
    {
        Add(LogLevel.Warning, message);
    }

    // Requires minimum Debug log level and that the Debug flag for the framework node in appsettings is set to true
    internal static void Debug(object message, object arg1 = null, object arg2 = null, object arg3 = null)
    {
        if (!AppConfigVariables.Debug) return;

        var args = new[] { arg1, arg2, arg3 }.Where(x => x != null).ToArray();

        var msg = "[Framework.Debug=true] " +
            (args.Length > 0
            ? $"{message}: {string.Join(", ", args)}"
            : message.ToString());

        Add(LogLevel.Debug, msg);
    }

    internal static void Add(LogLevel level, string message)
    {
        if(!IsFlushed)
            Messages.Enqueue((level, message));
        else
            Write(level, message);
    }
   
    internal static void Flush()
    {
        if (IsFlushed) return;

        IsFlushed = true;

        foreach (var message in Messages)
        {
            // LogWriter.Write(message.Item1 + ": " + message.Item2);
        }

        Messages.Clear();
        Messages = null;
    }

    static void Write(LogLevel level, string message)
    {
        switch (level)
        {
            case LogLevel.Critical:
                Log.Critical(message);
                break;
            case LogLevel.Error:
                Log.Error(message);
                break;
            case LogLevel.Warning:
                Log.Warning(message);
                break;
            case LogLevel.Debug:
                Log.Debug(message);
                break;
            default:
                break;
        }
    }
}
