namespace SystemLibrary.Common.Framework.Boostrap;

internal static class CacheInstance
{
    static CacheInstance()
    {
        Boot.Strap();
    }
}
