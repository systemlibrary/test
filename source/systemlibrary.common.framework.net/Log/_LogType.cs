namespace SystemLibrary.Common.Framework;

/// <summary>
/// Forward all log invocations to either STD or File Writer
/// </summary>
/// <remarks>
/// Registering your own ILogWriter always wins, then these settings dont matter
/// Not registering an ILogWriter, and you control wether or not to use the STD or File writer internally
/// </remarks>
internal enum LogType
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
