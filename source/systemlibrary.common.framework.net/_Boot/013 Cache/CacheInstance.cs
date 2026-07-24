namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class CacheInstance
{
    static CacheInstance()
    {
        Boot.Strap();
    }
}
