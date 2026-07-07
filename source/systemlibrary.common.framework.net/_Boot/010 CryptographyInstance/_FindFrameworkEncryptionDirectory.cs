namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static List<string> FindKeyDirectoriesInProbabilityOrder()
    {
        var directories = new List<string>();

        AddEncryptionKeyDirectory(directories);

        if (directories.Count == 0)
            AddParentKeyDirectories(directories);

        return directories;
    }

    static void AddEncryptionKeyDirectory(List<string> directories)
    {
        var encryptionKeyDirectory = CryptographyInstance.EncryptionKeyDirectory;

        if (encryptionKeyDirectory.IsNot()) return;

        if (encryptionKeyDirectory.IsFile())
            throw new Exception("[Bootstrap.Cryptography] EncryptionKeyDirectory points to a file, not a folder: " + encryptionKeyDirectory + ". It must be a directory containing an 'enc-xxx.key' file.");

        if (!Path.IsPathRooted(encryptionKeyDirectory))
        {
            encryptionKeyDirectory = Path.GetFullPath(
                encryptionKeyDirectory,
                AppRootInstance.ContentRootPath
            );

            //FrameworkLog.Warning("[Bootstrap.Cryptography] EncryptionKeyDirectory was relative, resolved to: " + encryptionKeyDirectory);
        }

        directories.Add(encryptionKeyDirectory);
    }

    static void AddParentKeyDirectories(List<string> directories)
    {
        var path = AppRootInstance.ContentRootPath;

        if (path.IsNot()) return;

        var parent = new DirectoryInfo(path).Parent;

        int num = 10;

        while (num > 0)
        {
            var isRootDir = parent?.Parent == null;

            if (isRootDir) break;

            directories.Add(parent.FullName);

            parent = parent.Parent;

            num--;
        }
    }

}