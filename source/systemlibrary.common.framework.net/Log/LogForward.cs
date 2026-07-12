namespace SystemLibrary.Common.Framework;

public enum LogForward
{
    /// <summary>
    /// Send all log invocations to stderr and stdout
    /// </summary>
    LogToStd,
    /// <summary>
    ///  Send all log invocations to ILogWriter
    /// </summary>
    /// <remarks>
    /// If not implemented a custom, an internal FileWriter is used to dump logs and a message is logged as a warning that you havent implemented one yet or registered it at least as a service in DI
    /// </remarks>
    LogToLogWriter
}

public enum StdForward
{
    /// <summary>
    /// None: unset, std goes to std streams 'stderr' and 'stdout' as normal
    /// </summary>
    None,

    /// <summary>
    /// All writes to stderr and stdout is forwarded to Log class which you control where lands
    /// </summary>
    StdToLog
}