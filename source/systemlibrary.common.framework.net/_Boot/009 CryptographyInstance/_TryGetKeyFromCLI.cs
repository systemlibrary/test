using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static void TryAppendKeyDataFromCLI(List<KeyData> keys)
    {
        var args = EnvironmentInstance.Args;

        if (args.IsNot()) return;

        var prefix = "--slcf-encryption-key";

        string key = null;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg.StartsWith(prefix + "=", StringComparison.OrdinalIgnoreCase))
            {
                key = arg.Substring(prefix.Length + 1).Trim();
                break;
            }

            if (arg.StartsWith(prefix + " ", StringComparison.OrdinalIgnoreCase))
            {
                key = arg.Substring(prefix.Length + 1).Trim();
                break;
            }
        }

        if (key.Is())
        {
            keys.Add(KeyData.Create("CLI", key));
        }
    }


    static string TryGetKeyFromCLI()
    {
        var args = EnvironmentInstance.Args;

        if (args.IsNot()) return null;

        var prefix = "--slcf-encryption-key";

        string key = null;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg.StartsWith(prefix + "=", StringComparison.OrdinalIgnoreCase))
            {
                key = arg.Substring(prefix.Length + 1).Trim();
                break;
            }

            if (arg.StartsWith(prefix + " ", StringComparison.OrdinalIgnoreCase))
            {
                key = arg.Substring(prefix.Length + 1).Trim();
                break;
            }
        }

        if(key.Is())
        {
            // FrameworkLog.Debug("[Bootstrap.Encryption] Encryption key source: CLI (" + key.MaxLength(3) + "...)");
        }
        return key;
    }
}
