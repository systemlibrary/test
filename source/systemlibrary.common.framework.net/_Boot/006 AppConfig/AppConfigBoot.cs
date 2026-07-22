using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static partial class AppConfigBoot
{
    internal static void Strap() { }

    static AppConfigBoot()
    {
        var configurationFiles = FindConfigurationFiles();

        AppConfigInstance.Configurations = AppConfigLoader.Load(configurationFiles);

        var builder = AppConfigBuilder.CreateDefaultBuilder();

        AppConfigBuilder.AppendJsonFile(builder, "appsettings", "json");

        AppConfigBuilder.AddEnvironmentVariables(builder);

        var temp = builder.Build();

        AppConfigInstance.KeyVaultUrl = temp["systemLibraryCommonFramework:config:keyVaultUrl"];
        //FrameworkLog.Debug($"'systemLibraryCommonFramework:config:keyVaultUrl' is configured: {keyVaultUrl.MaxLength(16)}.");

        AppConfigInstance.Debug = temp["systemLibraryCommonFramework:app:debug"]?.ToLower() == "true";
        //FrameworkLog.Debug($"'systemLibraryCommonFramework:app:debug' is true");
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
            file.Contains("mstest.framework.extensions", StringComparison.Ordinal) ||
            file.ContainsAny(StringComparison.Ordinal, "packages.json", "packages.xml", "package.json", "package-lock.json"))
            return false;

        return true;
    }
}
