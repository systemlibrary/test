namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppRootInstance
{
    /// <summary>
    /// Project root - never inside /bin/. Traverses up from BaseDirectory if needed.
    /// </summary>
    internal static string AppRootPath;

    /// <summary>
    /// Root for public static files (css, js). Falls back to AppRootPath if not set by the host.
    /// </summary>
    internal static string ContentRootPath;

    /// <summary>
    /// Root folder for cshtml views.
    /// </summary>
    internal static string AppViewRootPath;

    static AppRootInstance()
    {
        Boot.Strap();
    }
}
