namespace SystemLibrary.Common.Framework.Boostrap;


internal static class CryptographyInstance
{
    internal static string EncryptionKeyDirectory;

    internal static KeyData[] Keys;

    static CryptographyInstance()
    {
        Boot.Strap();
    }
}
