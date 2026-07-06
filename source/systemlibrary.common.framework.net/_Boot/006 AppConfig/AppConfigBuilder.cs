using Microsoft.Extensions.Configuration;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal static class AppConfigBuilder
{
    internal static ConfigurationBuilder AddEnvironmentVariables(ConfigurationBuilder builder)
    {
        builder.AddEnvironmentVariables(prefix: "ASPNETCORE_");

        builder.AddEnvironmentVariables(prefix: "DOTNET_");

        builder.AddEnvironmentVariables(prefix: "APP_");

        return builder;
    }

    internal static ConfigurationBuilder CreateDefaultBuilder()
    {
        var args = Environment.GetCommandLineArgs();

        var builder = new ConfigurationBuilder();

        builder.AddCommandLine(args);

        return builder;
    }

    internal static ConfigurationBuilder AppendKeyVault(ConfigurationBuilder builder, string jsonFileName)
    {
        AppConfigKeyVault.Add(builder, jsonFileName);

        return builder;
    }

    internal static ConfigurationBuilder AppendJsonFile(ConfigurationBuilder builder, string jsonFileName)
    {
        var environmentName = EnvironmentInstance.EnvironmentName;

        var isDev = EnvironmentInstance.IsDev;

        //// TODO: does this appsettings.js2on find the appsettings.json every time, even locally in dev without copy to output (bin)?
        //// Else 'AppRootInstance.RootPath' + appsettings.json should be sufficient
        builder.AddJsonFile(jsonFileName + ".json", optional: true, reloadOnChange: false);

        if(environmentName.Is())
            builder.AddJsonFile(jsonFileName + "." + environmentName + ".json", optional: true, reloadOnChange: false);

        return builder;
    }
}
