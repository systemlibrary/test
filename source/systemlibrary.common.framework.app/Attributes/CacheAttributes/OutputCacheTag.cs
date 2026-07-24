namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Tag constants for use with the OutputCache attribute to conditionally skip caching.
/// </summary>
public static class OutputCacheTag
{
    /// <summary>
    /// Skips output cache for any authenticated user.<br/>
    /// Does not work on Razor Pages — controllers only.
    /// </summary>
    /// <remarks>
    /// TODO: To be tested: unsure if it caches MVC Views, cannot recall at time of writing
    /// </remarks>
    public const string SkipWhenAuthenticated = "skipWhenAuthenticated=true";

    /// <summary>
    /// Skips output cache for users in any admin role.<br/>
    /// Does not work on Razor Pages — controllers only.
    /// </summary>
    /// <remarks>
    /// TODO: To be tested: unsure if it caches MVC Views, cannot recall at time of writing
    /// </remarks>
    public const string SkipWhenAdmin = "skipWhenAdmin=true";
}
