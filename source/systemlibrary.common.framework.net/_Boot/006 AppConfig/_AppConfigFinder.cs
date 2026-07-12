namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppConfigFinder
{
    internal static string[] FindConfigurationsInDirectory(string directoryPath, bool searchRecursively)
    {
        if (directoryPath.IsNot()) return [];

        if (!Directory.Exists(directoryPath)) return [];

        try
        {
            string[] files;

            if (!searchRecursively)
                files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
            else
                files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

            if (files == null || files.Length == 0) return [];

            return files
                .Where(x => x.EndsWithAny(StringComparison.OrdinalIgnoreCase, ".json", ".xml", ".config"))
                .Where(x => Path.GetFileName(x).Count(c => c == '.') <= 2)
                .ToArray();
        }
        catch(Exception ex)
        {
            // Framework.Debug(ex);

            return [];
        }
    }
}
