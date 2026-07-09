namespace SystemLibrary.Common.Framework.Boostrap;


partial class CryptographyInstanceBoot
{
    static string TryGetKeyFileName()
    {
        var keyFileFullPath = CryptographyInstance.EncryptionKeyFileFullPath;

        if (keyFileFullPath.IsNot()) return null;

        var key = Path.GetFileName(keyFileFullPath).Substring(4);

        //FrameworkLog.Debug("[Bootstrap.Encryption] Found key-file in 'EncryptionKeyDirectory' " + keyFileFullPath.MaxLength(6) + "... with key: " + key.MaxLength(3) + "…");

        return key;
    }
}
