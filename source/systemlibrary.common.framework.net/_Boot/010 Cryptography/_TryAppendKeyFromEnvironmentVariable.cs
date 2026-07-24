namespace SystemLibrary.Common.Framework.Bootstrap;

partial class CryptographyBoot
{
    static void TryAppendKeyFromEnvironmentVariable(List<KeyData> keys)
    {
        var key = EnvironmentVariable.Get("SLCF_ENCRYPTION_KEY");

        if (key.Is())
        {
            keys.Add(KeyData.Create("EnvVar", key, null));
            // FrameworkLog.Debug("[Bootstrap.Encryption] Encryption key source: ENVVAR (" + key.MaxLength(3) + "...)");
        }
    }
}
