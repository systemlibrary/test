using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static partial class AppConfigBoot
{
    internal static void Strap() { }

    static AppConfigBoot()
    {
        var configurationFiles = FindConfigurationFiles();

        AppConfigInstance.Configurations = AppConfigLoader.Load(configurationFiles);
    }

    static string[] FindConfigurationFiles()
    {
        var rootPath = AppRootInstance.AppRootPath;

        var configsDirPath = Path.Combine(rootPath, "Configs");
        var configurationsDirPath = Path.Combine(rootPath, "Configurations");

        var configurationFilesFound = Methods.Parallel(
            () => AppConfigFinder.FindConfigurationsInDirectory(rootPath, false),
            () => AppConfigFinder.FindConfigurationsInDirectory(configsDirPath, true),
            () => AppConfigFinder.FindConfigurationsInDirectory(configurationsDirPath, true)
        );

        var all = configurationFilesFound.Flatten();

        if (all.Is())
        {
            all = all
                .Where(FilterValidConfigurationFileNames)
                .ToArray();

            for (int i = 0; i < all.Length; i++)
            {
                all[i] = all[i]._ToOsFriendlyPath();
            }
        }

        if (all.IsNot())
        {
            //FrameworkLog.Debug("appsettings.json not found at root, default appsettings IConfiguration is created from CLI and some environment variables");
            all = new[] { "appsettings.json" };
        }

        return all;
    }

    static bool FilterValidConfigurationFileNames(string file)
    {
        if (file.IsNot()) return false;

        file = file.ToLower();

        if (file.Contains(".runtimeconfig.", StringComparison.Ordinal) ||
            file.Contains("appmanifest.", StringComparison.Ordinal) ||
            file.Contains(".deps.json", StringComparison.Ordinal) ||
            file.Contains("microsoft.visualstudio", StringComparison.Ordinal) ||
            file.Contains("launchsettings.json", StringComparison.Ordinal) ||
            file.Contains("lint.json", StringComparison.Ordinal) ||
            file.Contains("tsconfig.json", StringComparison.Ordinal) ||
            file.Contains("bower.json", StringComparison.Ordinal) ||
            file.Contains("shrinkwrap.json", StringComparison.Ordinal) ||
            file.Contains("build.xml", StringComparison.Ordinal) ||
            file.Contains("editorconfig.json", StringComparison.Ordinal) ||
            file.Contains("app.config", StringComparison.Ordinal) ||
            file.Contains("web.config", StringComparison.Ordinal) ||
            file.Contains("babelrc.json", StringComparison.Ordinal) ||
            file.ContainsAny(StringComparison.Ordinal, "packages.json", "packages.xml", "package.json", "package-lock.json"))
            return false;

        return true;
    }
}
