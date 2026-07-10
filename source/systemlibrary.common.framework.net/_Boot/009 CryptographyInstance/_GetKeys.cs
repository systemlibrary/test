namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyBoot
{
    internal static KeyData[] GetKeys()
    {
        var list = new List<KeyData>();

        TryAppendKeyFromCLI(list);

        TryAppendKeyFromEnvironmentVariable(list);

        TryAppendKeyFromKeyVault(list);

        TryAppendKeysFromDictionary(list);

        TryAppendDefaultKey(list);

        // FrameworkLog.Debug("[Bootstrap.Encryption] Primary key source: {list[0].Source} (" + list[0].KeyStart + "...)");
        for (int i = 1; i < list.Count; i++)
        {
            // FrameworkLog.Debug("[Bootstrap.Encryption] Fallback key source: {keyData.Source} (" + keyData.KeyStart + "...)");
        }

        return list.ToArray();
    }
}
