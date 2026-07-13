using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

internal class LogQueue
{
    public static ConcurrentQueue<LogMessage> Queue;

    internal const int MaxQueuePerInterval = 2400;

    const int Interval = 12; // ms

    public static volatile bool TimerStarted;

    public static Timer Timer;

    static object TimerLock;

    static LogMessage OverflowLogMessage;

    public static int QueueCounter;

    static LogQueue()
    {
        Queue = new();
        Timer = new Timer(LogFlusher.Flush, null, Interval / 3, Timeout.Infinite);
        TimerLock = new object();

        var overflowText = $"[Log] overflow, log queue discarded due to threshold of {MaxQueuePerInterval} messages reached within {Interval}ms.\n";

        OverflowLogMessage = new LogMessage(LogLevel.Critical, new object[] { overflowText });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    internal static void Add(LogLevel level, object[] obj)
    {
        var count = Interlocked.Increment(ref QueueCounter);

        if (count > MaxQueuePerInterval)
        {
            if (count == MaxQueuePerInterval + 1)
            {
                Queue.Enqueue(OverflowLogMessage);
                StartTimer();
            }

            if (level != LogLevel.Critical && level != LogLevel.Dump)
                return;
        }

        var message = new LogMessage(level, obj);

        Queue.Enqueue(message);

        StartTimer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    internal static void StartTimer()
    {
        if (TimerStarted) return;

        lock (TimerLock)
        {
            if (TimerStarted) return;

            TimerStarted = true;

            Timer.Change(Interval, Timeout.Infinite);
        }
    }

    internal static void Reset()
    {
        TimerStarted = false;
        Interlocked.Exchange(ref QueueCounter, 0);
    }
}
