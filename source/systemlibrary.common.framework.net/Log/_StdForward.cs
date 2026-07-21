namespace SystemLibrary.Common.Framework;

internal enum StdForward
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