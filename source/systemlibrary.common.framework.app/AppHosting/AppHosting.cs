namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Hosting server type used when starting the application via Application.Start or Application.Build.
/// </summary>
public enum AppHosting
{
    /// <summary>
    /// Kestrel web server
    /// </summary>
    Kestrel,

    /// <summary>
    /// IIS or IIS Express
    /// </summary>
    IIS,

    /// <summary>
    /// Unset web server, does not add any particular services nor middlewares for hosting
    /// </summary>
    Unset
}
    