namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static (string key, string pub, string priv) GetKeyFiles()
    {
        var directories = FindKeyDirectoriesInProbabilityOrder();

        // No user directory and no keys found; we automatically set it to ^1 it so EncryptAsymmetric can create them
        if (CryptographyInstance.EncryptionKeyDirectory.IsNot() && directories.Count > 0)
        {
            CryptographyInstance.EncryptionKeyDirectory = directories[^1];
        }
        else
        {
            // NOTE: fail lazily on EncryptAsymmetric invocation if App is hosted from OS root
        }

        string key = null, pub = null, priv = null;

        foreach (var directory in directories)
        {
            var res = FindKeyFilesInDirectory(directory);

            if (res.key.Is() || res.pub.Is() || res.priv.Is())
            {
                return res;
            }
        }

        return (key, pub, priv);
    }
}