namespace SystemLibrary.Common.Framework.Boostrap;

internal class LoggingSettings
{
    public LoggingLogLevel LogLevel { get; set; }

    public LoggingSettings()
    {
        LogLevel = new LoggingLogLevel();
    }
}

internal class LoggingLogLevel
{
    public string Default { get; set; } = "Warning";
}