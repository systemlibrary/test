namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static string TryGetKeyFromEnvironmentVariable()
    {
        var key = EnvironmentInstance.GetEnvironmentVariable("SLCF_ENCRYPTION_KEY") ??
                EnvironmentInstance.GetEnvironmentVariable("slcf_encryption_key");

        if (key.Is())
        {
            //FrameworkLog.Debug("[Bootstrap.Encryption] key is based on env-var: " + key.MaxLength(3) + "...");
        }

        return key;
    }
}
