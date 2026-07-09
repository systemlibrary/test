namespace SystemLibrary.Common.Framework.Boostrap;

partial class CryptographyInstanceBoot
{
    static (string key, string pub, string priv) FindKeyFilesInDirectory(string directory)
    {
        try
        {
            var files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Where(f =>
                {
                    return f.EndsWith(".key", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".pub", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".priv", StringComparison.OrdinalIgnoreCase);
                })
            .ToArray();

            if (files == null || files.Length == 0) return (null, null, null);

            var keyFile = FindKeyFile(files);

            var rsaKeyFile = FindRsaKeyFiles(files);

            return (keyFile, rsaKeyFile.pub, rsaKeyFile.priv);
        }
        catch (Exception ex)
        {
            //FrameworkLog.Warning("Could not read directory " + directory + ": " + ex.Message + " Continuing...");
            return (null, null, null);
        }
    }

    static (string pub, string priv) FindRsaKeyFiles(string[] files)
    {
        string pub = null, priv = null;

        foreach (var file in files)
        {
            var e = Path.GetExtension(file);

            if (e.EndsWith("pub")) pub = file;

            else if (e.EndsWith("priv")) priv = file;
        }

        return (pub, priv);
    }

    static string FindKeyFile(string[] files)
    {
        var keyFiles = files.Where(
            file => file.EndsWith(".key", StringComparison.OrdinalIgnoreCase) &&
            file.Contains("enc-", StringComparison.OrdinalIgnoreCase)
        ).ToArray();

        if (keyFiles.Length == 0) return null;

        // Preserve and use the oldest key file always, so one can easily Decrypt old values to get the text, then delete old key file to encrypt again with latest if needed
        if (keyFiles.Length > 1)
        {
            keyFiles = keyFiles
                .OrderBy(file => file.Length)
                .ThenBy(file =>
                {
                    var creationTime = File.GetCreationTime(file);
                    return creationTime == DateTime.MinValue
                        ? File.GetLastWriteTime(file)
                        : creationTime;
                })
                .ToArray();
        }

        foreach (var fullKeyFilePath in keyFiles)
        {
            if (fullKeyFilePath.Length <= 20) continue;

            return fullKeyFilePath._ToOsFriendlyPath();
        }

        return null;
    }
}