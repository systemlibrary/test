namespace SystemLibrary.Common.Framework.Bootstrap;

partial class CryptographyBoot
{
    static void TryAppendDefaultKey(List<KeyData> keys)
    {
        if(keys.Where(x => !x.Source.StartsWith("RSA")).Count() == 0)
        {
            keys.Add(KeyData.Create("Default", "ABCDEFGHIJKLMNOPQRST123456789011", null));
            // FrameworkLog.Debug("[Bootstrap.Encryption] Encryption key source: ENVVAR (" + key.MaxLength(3) + "...)");
        }
    }
}
