using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyBoot
{
    static void TryAppendKeyFromCLI(List<KeyData> keys)
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
            keys.Add(KeyData.Create("CLI", key, null));
        }
    }
}
