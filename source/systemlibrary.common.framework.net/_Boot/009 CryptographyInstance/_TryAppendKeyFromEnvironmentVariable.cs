namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyBoot
{
    static void TryAppendKeyFromEnvironmentVariable(List<KeyData> keys)
    {
        var key = EnvironmentInstance.GetEnvironmentVariable("SLCF_ENCRYPTION_KEY");

        if (key.Is())
        {
            keys.Add(KeyData.Create("EnvVar", key, null));
            // FrameworkLog.Debug("[Bootstrap.Encryption] Encryption key source: ENVVAR (" + key.MaxLength(3) + "...)");
        }
    }
    static string TryGetKeyFromEnvironmentVariable()
    {
        var key = EnvironmentInstance.GetEnvironmentVariable("SLCF_ENCRYPTION_KEY");

        if (key.Is())
        {
            // FrameworkLog.Debug("[Bootstrap.Encryption] Encryption key source: ENVVAR (" + key.MaxLength(3) + "...)");
        }

        return key;
    }
}
