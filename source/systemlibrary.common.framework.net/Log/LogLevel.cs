namespace SystemLibrary.Common.Framework;

public enum LogLevel
{
    /// <summary>
    /// A 'placeholder' to ease and be deliberate in the appsettings that all log invocations are written
    /// </summary>
    All = -1,

    /// <summary>
    /// Default, sets the min log level to be the 'logging' in appsettings
    /// </summary>
    Unset = 0,

    /// <summary>
    /// Raw internals - every micro event meant for inspection only
    /// </summary>
    Trace = 1,

    /// <summary>
    /// Lifecycle and state changes as messages
    /// </summary>
    Information = 2,

    /// <summary>
    /// Soft issues which are recoverable, but somewhat unexpected
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Developer targeted probes 'why is this not behaving properly' messages
    /// </summary>
    Debug = 4,

    /// <summary>
    /// Exceptions that has to stop execution of code
    /// </summary>
    Error = 5,

    /// <summary>
    /// System failures
    /// </summary>
    Critical = 9,

    /// <summary>
    /// Ignored log level messages
    /// </summary>
    None = 9999,

    /// <summary>
    /// Ignored all thresholds and always sends the message to the ILogWriter
    /// </summary>
    Dump = 99999
}
