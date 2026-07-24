using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App;

internal class StdToLogILogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    static Dictionary<LogLevel, Microsoft.Extensions.Logging.LogLevel[]> Map = new()
    {
        { LogLevel.All, Enum.GetValues<Microsoft.Extensions.Logging.LogLevel>() },
        { LogLevel.Dump, Enum.GetValues<Microsoft.Extensions.Logging.LogLevel>() },
        { LogLevel.Trace, new[] { Microsoft.Extensions.Logging.LogLevel.Trace } },
        { LogLevel.Information, new[] { Microsoft.Extensions.Logging.LogLevel.Information } },
        { LogLevel.Warning, new[] { Microsoft.Extensions.Logging.LogLevel.Warning } },
        { LogLevel.Debug, new[] { Microsoft.Extensions.Logging.LogLevel.Debug } },
        { LogLevel.Error, new[] { Microsoft.Extensions.Logging.LogLevel.Error } },
        { LogLevel.Critical, new[] { Microsoft.Extensions.Logging.LogLevel.Critical } },
        { LogLevel.None, Array.Empty<Microsoft.Extensions.Logging.LogLevel>() }
    };

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        var enabledLevels = Map[LogInstance.MinLogLevel];

        return enabledLevels.Contains(logLevel);
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (exception == null)
        {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.Critical)
            {
                global::Log.Critical(logLevel + " " + eventId + ": " + state);
            }
            else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            {
                global::Log.Error(logLevel + " " + eventId + ": " + state);
            }
            else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Warning)
            {
                global::Log.Warning(logLevel + " " + eventId + ": " + state);
            }
            else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Debug)
            {
                global::Log.Debug(logLevel + " " + eventId + ": " + state);
            }
            else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Information)
            {
                global::Log.Information(logLevel + " " + eventId + ": " + state);
            }
            else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Trace)
            {
                global::Log.Trace(logLevel + " " + eventId + ": " + state);
            }
            return;
        }

        if (logLevel == Microsoft.Extensions.Logging.LogLevel.Critical)
            global::Log.Critical(exception);

        else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Error)
            global::Log.Error(exception);

        else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Debug)
            global::Log.Debug(exception);

        else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Warning)
            global::Log.Warning(exception);

        else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Information)
            global::Log.Information(exception);

        else if (logLevel == Microsoft.Extensions.Logging.LogLevel.Trace)
            global::Log.Trace(exception);

        else
            global::Log.Dump(exception);
    }
}
