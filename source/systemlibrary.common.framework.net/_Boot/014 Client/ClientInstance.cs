namespace SystemLibrary.Common.Framework.Boostrap;

internal static class ClientInstance
{
    static ClientInstance()
    {
        Boot.Strap();
    }
}
