namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class AppRootInstance
{
    /// <summary>
    /// Project root - never inside /bin/. Traverses up from BaseDirectory if needed.
    /// </summary>
    internal static string RootPath;

    /// <summary>
    /// Root for public static files (css, js). Falls back to AppRootPath if not set by the host.
    /// </summary>
    internal static string WebRootPath;

    /// <summary>
    /// Root folder for cshtml views.
    /// </summary>
    internal static string ViewRootPath;

    static AppRootInstance()
    {
        Boot.Strap();
    }
}
