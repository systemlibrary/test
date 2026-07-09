namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
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
