namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class AppRootBoot
{
    internal static void Strap() { }

    static AppRootBoot()
    {
        AppRootInstance.RootPath = RootPath();

        AppRootInstance.WebRootPath = WebRootPath();

        AppRootInstance.ViewRootPath = AppRootInstance.RootPath;
    }

    static string RootPath()
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

    // NOTE: UseContentRoot() is an option in ASPNET, if invoked has to override this one somehow
    static string WebRootPath()
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

        var wwwroot = Path.Combine(AppRootInstance.RootPath, "wwwroot");

        if (Directory.Exists(wwwroot))
            return CleanRootPath(wwwroot);

        return AppRootInstance.RootPath;
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
