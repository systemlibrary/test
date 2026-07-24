namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class MetricsInstance
{
    internal static bool Enabled;
    internal static string Url;
    internal static string Path;
    internal static string PathUI;

    static MetricsInstance()
    {
        Boot.Strap();
    }
}
