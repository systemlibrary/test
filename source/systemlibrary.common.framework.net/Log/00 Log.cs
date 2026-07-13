using System.Runtime.CompilerServices;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Boostrap;

/// <summary>
/// Static class for writing log entries at various severity levels and clearing the log file.
/// </summary>
public static class Log
{
    static bool WarningDumped = false;
    static object WarningDumpedLock = new object();

    /// <summary>
    /// Logs a critical-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Critical("Database connection lost");
    /// </code>
    /// </example>
    public static void Critical(params object[] obj)
    {
        Write(LogLevel.Critical, obj);
    }

    /// <summary>
    /// Logs an error-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Error("Something failed", exception);
    /// </code>
    /// </example>
    public static void Error(params object[] obj)
    {
        Write(LogLevel.Error, obj);
    }

    /// <summary>
    /// Logs a debug-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Debug("Value:", myObject);
    /// </code>
    /// </example>
    public static void Debug(params object[] obj)
    {
        Write(LogLevel.Debug, obj);
    }

    /// <summary>
    /// Logs a warning-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Warning("Retry attempt", retryCount);
    /// </code>
    /// </example>
    public static void Warning(params object[] obj)
    {
        Write(LogLevel.Warning, obj);
    }

    /// <summary>
    /// Logs a trace-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Trace("Entering method", nameof(MyMethod));
    /// </code>
    /// </example>
    public static void Trace(params object[] obj)
    {
        Write(LogLevel.Trace, obj);
    }

    /// <summary>
    /// Logs an information-level message.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Information("App started", version);
    /// </code>
    /// </example>
    public static void Information(params object[] obj)
    {
        Write(LogLevel.Information, obj);
    }

    /// <summary>
    /// Writes a single value directly to the log file regardless of log level — equivalent to console.log.
    /// </summary>
    /// <example>
    /// <code>
    /// Log.Dump(myObject);
    /// </code>
    /// </example>
    public static void Dump(params object[] obj)
    {
        Write(LogLevel.Dump, obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static void Write(LogLevel level, object[] obj)
    {
        if (level != LogLevel.Dump)
        {
            if (LogInstance.IsLogDisabled)
            {
                WriteLogIsDisabledMessage();
                return;
            }

            if (level < LogInstance.MinLogLevel) return;
        }

        if (MetricsInstance.Enable)
            Metric.Inc("Log", level.ToString());

        LogQueue.Add(level, obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static void WriteLogIsDisabledMessage()
    {
        if (!WarningDumped)
        {
            lock (WarningDumpedLock)
            {
                if (!WarningDumped)
                {
                    WarningDumped = true;
                    try
                    {
                        Dump("[Common.Framework] Logging is unset for the framework and the default logging is set to 'none', will not log anything from here on.");
                    }
                    catch
                    {
                        // swallow
                    }

                }
            }
        }
    }

    internal static bool SupportsAnsi = !Console.IsOutputRedirected;
}
