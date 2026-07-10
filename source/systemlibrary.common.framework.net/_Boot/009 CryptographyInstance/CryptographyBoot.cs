namespace SystemLibrary.Common.Framework.Boostrap;

internal static partial class CryptographyBoot
{
    internal static void Strap() { }

    static CryptographyBoot()
    {
        CryptographyInstance.EncryptionKeyDirectory = FrameworkSettingsInstance.Current.Cryptography.EncryptionKeyDirectory;

        CryptographyInstance.Keys = GetKeys();
    }
}
