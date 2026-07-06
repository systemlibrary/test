using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

internal static class AppConfigTemp
{
    /// <summary>
    /// 'AppSettings' stored as temp, never uses keyvault, as this is used locally to figure out KeyVaultUrl and UserSecretsId if configured
    /// </summary>
    internal static IConfigurationRoot Temp;

    static AppConfigTemp()
    {
        var builder = AppConfigBuilder.CreateDefaultBuilder();

        AppConfigBuilder.AppendJsonFile(builder, "appsettings");

        AppConfigBuilder.AddEnvironmentVariables(builder);

        Temp = builder.Build();
    }
}
