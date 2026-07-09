namespace SystemLibrary.Common.Framework.Boostrap;


internal static class CryptographyInstance
{
    internal static string EncryptionKeyDirectory;

    internal static byte[] Key;
    internal static string KeyStart;
    internal static byte[] KeyStartBytes;

    internal static string EncryptionKeyFileFullPath;
    internal static string EncryptionRsaPubFileFullPath;
    internal static string EncryptionRsaPrivFileFullPath;

    static CryptographyInstance()
    {
        Boot.Strap();
    }
}
