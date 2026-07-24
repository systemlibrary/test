namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class CryptographyInstance
{
    internal static string KeyDirectory;

    internal static KeyData[] Keys;
    internal static KeyData[] DecryptKeys;
    internal static KeyData[] RsaKeys;
    internal static KeyData PrimaryKey;

    static CryptographyInstance()
    {
        Boot.Strap();
    }
}
