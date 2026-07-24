namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class ClientInstance
{
    static ClientInstance()
    {
        Boot.Strap();
    }
}
