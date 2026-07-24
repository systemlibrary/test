using System.Collections.Concurrent;
using System.Text;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

internal class FileLogWriter : ILogWriter
{
    static Timer Interval = new Timer(SafeWriteQueue, null, IntervalTimeMs, Timeout.Infinite);
    static ConcurrentQueue<LogMessage> Queue = new();

    const int IntervalTimeMs = 6;

    const long MaxLogSizeBytes = 25 * 1024 * 1024;

    static object TimerLock = new object();
    static bool TimerStarted;

    static int TruncateCounter;

    public void Write(LogMessage message)
    {
        Queue.Enqueue(message);

        StartTimer();
    }

    static void StartTimer()
    {
        if (TimerStarted) return;

        lock (TimerLock)
        {
            if (TimerStarted) return;

            LogFileFullTruncate();

            Interval.Change(IntervalTimeMs, Timeout.Infinite);

            TimerStarted = true;
        }
    }

    static void LogFileFullTruncate()
    {
        TruncateCounter++;

        // Note: if log occurs every ms, 250 means roughly 2.5 seconds under heavy load, usually once a minute ish
        if (TruncateCounter > 250)
        {
            TruncateCounter = TruncateCounter - 250;
            try
            {
                FileInfo logFile = new(LogInstance.FilePath);

                if (logFile.Exists && logFile.Length >= MaxLogSizeBytes)
                {
                    File.WriteAllText(LogInstance.FilePath, string.Empty);
                }
            }
            catch
            {
                // swallow
            }
        }
    }

    static void SafeWriteQueue(object state)
    {
        StringBuilder batch = new(2048);

        try
        {
            var max = 5000;
            while (Queue.TryDequeue(out var message))
            {
                var text = LogFormatter.Format(message);

                batch.Append(text);
                batch.AppendLine();

                if(LogInstance.Format != LogFormat.NDJson)
                    batch.AppendLine();

                max--;
                if (max < 0)
                    break;
            }

            if (batch.Length > 0)
                File.AppendAllText(LogInstance.FilePath, batch.ToString(), Encoding.UTF8);

            batch.Clear();
        }
        catch
        {
            Thread.Sleep(IntervalTimeMs);

            try
            {
                if (batch.Length > 0)
                    File.AppendAllText(LogInstance.FilePath, batch.ToString(), Encoding.UTF8);
            }
            catch
            {
            }
        }
        finally
        {

            // Passing in "new object" internally to avoid restarting timer in app shutdown
            if (state == null)
            {
                if (!Queue.IsEmpty)
                {
                    Interval.Change(IntervalTimeMs, Timeout.Infinite);
                }
                else
                {
                    TimerStarted = false;
                }
            }
        }
    }
}
