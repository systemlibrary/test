namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Policy name constants for use with the OutputCache attribute's PolicyName property.
/// </summary>
public static class OutputCachePolicy
{
    /// <summary>
    /// Splits cache into two buckets — authenticated and unauthenticated users.
    /// </summary>
    public const string CacheAuthenticated = "slcf__CacheAuthenticatedPolicy";

    /// <summary>
    /// Varies cache by all roles on the current user, concatenated as a single cache key.
    /// </summary>
    public const string CacheRoles = "slcf__CacheRolesPolicy";

    /// <summary>
    /// Varies cache by common identity claims: sub, phone, email, id.
    /// </summary>
    /// <remarks>
    /// Only use if you trust the IDP to return unique claims per user and they must be lower cased
    /// </remarks>
    public const string CacheUser = "slcf__CacheUserPolicy";

    /// <summary>
    /// Placeholder constant — not a real registered policy.<br/>
    /// Documents that the standard OutputCache attribute supports framework-specific tags via OutputCacheTag.
    /// </summary>
    public const string OutputCacheTags = "slcf__NoPolicyRegistered";
}
