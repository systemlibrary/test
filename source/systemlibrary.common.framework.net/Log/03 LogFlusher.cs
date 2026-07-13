using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal class LogFlusher
{
    internal static ILogWriter Writer;

    static LogFlusher()
    {
        Writer = ServiceProviderInstance.Current.GetService<ILogWriter>();

        if (Writer == null)
        {
            string message;
            if (LogInstance.LogForward == LogWriterType.File)
            {
                message = "[Common.Framework] File log writer";
                Writer = new FileLogWriter();

            }
            else
            {
                message = "[Common.Framework] Standard log writer";
                Writer = new StdLogWriter();
            }
            
            if(LogInstance.MinLogLevel <= LogLevel.Information)
            {
                var logMessage = new LogMessage(LogLevel.Information, message + " " + LogInstance.MinLogLevel.ToString());
                Writer.Write(logMessage);
            }
        }
        else
        {
            FrameworkLog.Debug("Custom ILogWriter found");
        }
    }

    static int Shutdown;

    internal static void ShutdownFlush()
    {
        if (Interlocked.Exchange(ref Shutdown, 1) == 1)
            return;

        Dequeue();

        Thread.Sleep(33);
    }

    internal static void Flush(object state = null)
    {
        try
        {
            Dequeue();

            LogQueue.Reset();

            if (!LogQueue.Queue.IsEmpty)
                LogQueue.StartTimer();
        }
        catch
        {
        }
    }

    static void Dequeue()
    {
        var queue = LogQueue.Queue;

        var processed = 0;

        while (queue.TryDequeue(out LogMessage message))
        {
            Writer.Write(message);

            processed++;

            if (processed >= LogQueue.MaxQueuePerInterval + 10)
                break;
        }
    }
}
