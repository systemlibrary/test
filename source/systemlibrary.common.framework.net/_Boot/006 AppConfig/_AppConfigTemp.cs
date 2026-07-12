namespace SystemLibrary.Common.Framework;

internal static class AppConfigVariables
{
    internal static string KeyVaultUrl;
    internal static bool Debug;

    static AppConfigVariables()
    {
        var builder = AppConfigBuilder.CreateDefaultBuilder();

        AppConfigBuilder.AppendJsonFile(builder, "appsettings", "json");

        AppConfigBuilder.AddEnvironmentVariables(builder);

        var temp = builder.Build();

        KeyVaultUrl = temp["systemLibraryCommonFramework:config:keyVaultUrl"];
        //FrameworkLog.Debug($"'systemLibraryCommonFramework:config:keyVaultUrl' is configured: {keyVaultUrl.MaxLength(16)}.");

        Debug = temp["systemLibraryCommonFramework:app:debug"]?.ToLower() == "true";
        //FrameworkLog.Debug($"'systemLibraryCommonFramework:app:debug' is true");
    }
}
