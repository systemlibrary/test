namespace SystemLibrary.Common.Framework.Boostrap;

internal static class LogInstance
{
    internal static string FullFilePath;
    internal static LogFormat Format;
    internal static LogForward LogForward;
    internal static StdForward StdForward;
    internal static LogLevel MinLogLevel;
    internal static bool IsLogDisabled;

    static LogInstance()
    {
        Boot.Strap();
    }
}
