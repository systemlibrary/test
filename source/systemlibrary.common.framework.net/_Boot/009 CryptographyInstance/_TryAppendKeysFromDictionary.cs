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
        var foundKeys = Methods.Parallel(directories, FindKeyFilesInDirectory);

        return foundKeys.Flatten();
    }

    static List<KeyData> FindKeyFilesInDirectory(string directory)
    {
        if (directory.IsNot()) return null;

        try
        {
            var files = SearchDirectory(directory);

            if (files == null) return null;

            var filesSorted = SortKeyFiles(files);

            return ConvertFilesToKeyDataList(filesSorted);
        }
        catch (Exception ex)
        {
            //FrameworkLog.Warning("Could not read directory " + directory + ": " + ex.Message + " Continuing...");
        }

        return null;
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

    static string[] SearchDirectory(string directory)
    {
        return Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)?
                        .Where(f =>
                        {
                            return (f.EndsWith(".key", StringComparison.OrdinalIgnoreCase) && f.Contains("enc-", StringComparison.OrdinalIgnoreCase)) ||
                                f.EndsWith(".pub", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".priv", StringComparison.OrdinalIgnoreCase);
                        })?
                    .ToArray();
    }

    static string[] SortKeyFiles(string[] keyFiles)
    {
        return keyFiles
            .OrderByDescending(GetKeyFileSortTime)
            .Select(x => x._ToOsFriendlyPath())
            .ToArray();
    }

    static List<KeyData> ConvertFilesToKeyDataList(string[] files)
    {
        var result = new List<KeyData>();

        foreach (var file in files)
        {
            if (file.Contains("enc-", StringComparison.OrdinalIgnoreCase) && file.EndsWith(".key", StringComparison.OrdinalIgnoreCase))
                result.Add(KeyData.Create("KEYFILE", Path.GetFileName(file), null));
            else
            {
                // Adds RSAPUB and RSAPRIV
                result.Add(KeyData.Create("RSA" + Path.GetExtension(file).ToUpper(), null, file));
            }
        }
        return result;
    }

    static DateTime GetKeyFileSortTime(string file)
    {
        try
        {
            var creationTime = File.GetCreationTime(file);

            if (creationTime.Year > 2000)
                return creationTime;

            var lastWriteTime = File.GetLastWriteTime(file);

            return lastWriteTime > creationTime
                ? lastWriteTime
                : creationTime;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}
