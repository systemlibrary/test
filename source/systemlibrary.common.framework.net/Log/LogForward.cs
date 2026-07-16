namespace SystemLibrary.Common.Framework;

/// <summary>
/// Forward all log invocations to either STD or File Writer
/// </summary>
/// <remarks>
/// Registering your own ILogWriter always wins, then these settings dont matter
/// Not registering an ILogWriter, and you control wether or not to use the STD or File writer internally
/// </remarks>
public enum LogType
{
    /// <summary>
    /// Sends all log invocations to stderr and stdout
    /// </summary>
    /// <remarks>
    /// INjecting your own ILogWriter always takes precedence.
    /// </remarks>
    Std,
    /// <summary>
    /// Sends all log invocations to ILogWriter
    /// </summary>
    /// <remarks>
    /// Injecting your own ILogWriter always takes precedence.
    /// </remarks>
    File
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