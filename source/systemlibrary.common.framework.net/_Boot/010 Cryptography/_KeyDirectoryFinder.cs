namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class KeyDirectoryFinder
{
    internal static List<KeyData> FindKeyFilesInDirectory(string directory)
    {
        if (directory.IsNot()) return null;

        try
        {
            var files = SearchDirectory(directory);

            if (files == null) return null;

            if (CryptographyInstance.KeyDirectory != null &&
                CryptographyInstance.KeyDirectory != directory)
            {
                FrameworkLog.Warning("Multiple key directories exist in the parent, found keys in " + directory + ", but also found in " + CryptographyInstance.KeyDirectory + ". Please have only key files in one parent folder or manually set the directory in appsettings");
            }
            else
            {
                CryptographyInstance.KeyDirectory = directory;
            }

            var filesSorted = SortKeyFiles(files);

            return ConvertFilesToKeyDataList(filesSorted);
        }
        catch (Exception ex)
        {
            //FrameworkLog.Warning("Could not read directory " + directory + ": " + ex.Message + " Continuing...");
        }

        return null;
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
                result.Add(KeyData.Create("RSA" + Path.GetExtension(file).Replace(".", "").ToUpper(), null, file));
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
