using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyBoot
{
    static void TryAppendKeysFromDictionary(List<KeyData> keys)
    {
        var directories = new List<string>();

        var keyDirectory = CryptographyInstance.EncryptionKeyDirectory;

        if (keyDirectory.Is())
        {
            directories.Add(keyDirectory);
        }
        else
        {
            directories.AddRange(GetParentDirectories());
        }

        var foundKeys = FindKeysInDirectories(directories);

        keys.AddRange(foundKeys);
    }

    static KeyData[] FindKeysInDirectories(List<string> directories)
    {
        var foundKeys = Methods.Parallel(directories, KeyDirectoryFinder.FindKeyFilesInDirectory);

        return foundKeys.Flatten();
    }

    static List<string> GetParentDirectories()
    {
        var path = AppRootInstance.ContentRootPath;

        var start = new DirectoryInfo(path);

        if (EnvironmentInstance.IsDev)
        {
            // Framework.Debug(...)
            start = start.Parent;
        }
        else
        {
            // Framework.Debug(...)
        }

        var directories = new List<string>();

        int num = 10;

        while (num > 0)
        {
            var isRootDir = start?.Parent == null;

            if (isRootDir) break;

            directories.Add(start.FullName);

            start = start.Parent;

            num--;
        }

        return directories;
    }
}
