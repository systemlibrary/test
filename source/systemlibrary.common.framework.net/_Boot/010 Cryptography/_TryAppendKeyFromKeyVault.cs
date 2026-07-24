namespace SystemLibrary.Common.Framework.Bootstrap;

partial class CryptographyBoot
{
    static void TryAppendKeyFromKeyVault(List<KeyData> keys)
    {
        var value = AppConfigKeyVault.Get("slcf-encryption-key");

        if (value.Is())
            keys.Add(KeyData.Create("KEYVAULT", value,null));
    }
}
