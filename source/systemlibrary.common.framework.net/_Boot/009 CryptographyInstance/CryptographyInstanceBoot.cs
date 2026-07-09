namespace SystemLibrary.Common.Framework.Boostrap;

internal static partial class CryptographyInstanceBoot
{
    internal static void Strap() { }

    static CryptographyInstanceBoot()
    {
        CryptographyInstance.EncryptionKeyDirectory = FrameworkSettingsInstance.Current.Cryptography.EncryptionKeyDirectory;
        
        var keyFiles = GetKeyFiles();

        // TODO: Support two key files and two/three/four pub/priv files at a time, so one can change from old to new without worry
        // Simply order by date created, modified, take top 2-3 newest, so one has to cleanup...
        CryptographyInstance.EncryptionKeyFileFullPath = keyFiles.key;
        CryptographyInstance.EncryptionRsaPubFileFullPath = keyFiles.pub;
        CryptographyInstance.EncryptionRsaPrivFileFullPath = keyFiles.priv;

        var key = Key();

        var keyHashed = key.ToSha256Hash();

        var bytes = Convert.FromHexString(keyHashed);

        CryptographyInstance.Key = bytes;

        CryptographyInstance.KeyStart = key.MaxLength(3);

        CryptographyInstance.KeyStartBytes = key.MaxLength(3).GetBytes();
    }
}
