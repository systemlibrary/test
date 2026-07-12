namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppInstance
{
    internal static string Name;
    internal static bool Diagnostics;
    internal static bool Debug;
    internal static string License;
    internal static int Salt;
    internal static string HostName;

    internal static int Hosting;

    static AppInstance()
    {
        Boot.Strap();
    }
}
