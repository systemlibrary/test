namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyBoot
{
    static void TryAppendKeyFromKeyVault(List<KeyData> keys)
    {
        var value = AppConfigKeyVault.Get("slcf-encryption-key");

        if (value.Is())
            keys.Add(KeyData.Create("KEYVAULT", value, null));
    }
}
