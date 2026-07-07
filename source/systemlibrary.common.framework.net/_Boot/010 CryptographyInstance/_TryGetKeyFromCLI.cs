namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static string TryGetKeyFromCLI()
    {
        var key = EnvironmentInstance.Args?
            .FirstOrDefault(a => a.StartsWith("--slcf-encryption-key", StringComparison.OrdinalIgnoreCase))?
            .Split('=')?
            .ElementAtOrDefault(1);

        if (key.Is())
        {
            //FrameworkLog.Debug("[Bootstrap.Encryption] key is set as a command line arg: " + key.MaxLength(3) + "...");
            return key;
        }

        return null;
    }
}
