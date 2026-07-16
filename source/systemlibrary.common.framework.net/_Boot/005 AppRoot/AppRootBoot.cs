namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppRootBoot
{
    internal static void Strap() { }

    static AppRootBoot()
    {
        AppRootInstance.AppRootPath = AppRootPath();

        AppRootInstance.ContentRootPath = ContentRootPath();

        AppRootInstance.AppViewRootPath = AppRootInstance.AppRootPath;
    }

    static string AppRootPath()
    {
        var path = new DirectoryInfo(AppContext.BaseDirectory).FullName;

        var tmp = path;
        int max = 10;

        while (IsInBin(tmp))
        {
            tmp = new DirectoryInfo(tmp).Parent?.FullName;

            if (tmp.IsNot()) break;

            max--;

            if (max <= 0) break;
        }

        if (path == tmp || tmp.IsNot())
        {
            return CleanRootPath(path);
        }

        if (IsTestProjectByFolderNamingConvention(tmp))
        {
            if (File.Exists(Path.Combine(tmp, "appsettings.json")))
            {
                return CleanRootPath(tmp);
            }
            return CleanRootPath(path);
        }

        return CleanRootPath(tmp);
    }

    static string ContentRootPath()
    {
        try
        {
            var path = AppDomain.CurrentDomain?.GetData("ContentRootPath") + "";

            if(path.Is())
                return CleanRootPath(path);
        }
        catch
        {
            // swallow
        }

        return AppRootInstance.AppRootPath;
    }

    static bool IsInBin(string dir)
    {
        return dir._PathContains("/bin/") || dir._PathEndsWith("/bin");
    }

    static bool IsTestProjectByFolderNamingConvention(string dir)
    {
        return
            dir._PathEndsWith(".Tests") || dir._PathEndsWith(".Test") ||
            dir._PathEndsWith("/Tests") || dir._PathEndsWith("/Test");
    }

    static string CleanRootPath(string path)
    {
        return path.Replace("\\", "/").TrimEnd('/');
    }
}
