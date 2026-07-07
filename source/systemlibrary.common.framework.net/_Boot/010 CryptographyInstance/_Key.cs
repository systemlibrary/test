namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    internal static string Key()
    {
        var key = TryGetKeyFromCLI();

        if (key.IsNot())
        {
            key = TryGetKeyFromEnvironmentVariable();
        }

        if (key.IsNot())
        {
            key = TryGetKeyFileName();
        }

        if(key.IsNot())
        {
            // TODO: Check if KeyVault is available and a keyvault is configured under 'app' in Framework settings
            // which when uses the default keyvault identity in appservice, if also available, and connects to get the key if it exists
            // a short timeout should be added, say top 15 seconds, assuming key vault maybe have 30-120s as that bloated if already in error
        }

        if (key.IsNot())
        {
            key = "ABCDEFGHIJKLMNOPQRST123456789011";

            //FrameworkLog.Debug("[Bootstrap.Encryption] Key is defaulting to: 'ABC...' as a 'Framework Enc Key File' was not found");
        }

        return (key + "#?(" + AppInstance.Name + "^");
    }
}
