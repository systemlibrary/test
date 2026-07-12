namespace SystemLibrary.Common.Framework.Boostrap;

internal static class MetricsInstance
{
    internal static bool Enable;
    internal static string secretKeyForUI;
    internal static string Url;
    internal static string Path;
    internal static string PathUI;

    static MetricsInstance()
    {
        Boot.Strap();
    }
}
