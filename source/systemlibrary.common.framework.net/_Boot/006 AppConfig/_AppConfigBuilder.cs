using System.Reflection;

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

        builder.SetBasePath(AppRootInstance.AppRootPath);

        builder.AddCommandLine(args);

        return builder;
    }

    internal static ConfigurationBuilder AppendKeyVault(ConfigurationBuilder builder, string jsonFileName)
    {
        AppConfigKeyVault.Add(builder, jsonFileName);

        return builder;
    }

    internal static ConfigurationBuilder AppendXmlFile(ConfigurationBuilder builder, string xmlFileName, string extension)
    {
        var environmentName = EnvironmentInstance.EnvironmentName;

        var isDev = EnvironmentInstance.IsDev;

        //// TODO: does this appsettings.js2on find the appsettings.json every time, even locally in dev without copy to output (bin)?
        //// Else 'AppRootInstance.RootPath' + appsettings.json should be sufficient
        builder.AddXmlFile(xmlFileName + extension, optional: true, reloadOnChange: false);

        if (environmentName.Is())
            builder.AddXmlFile(xmlFileName + "." + environmentName + extension, optional: true, reloadOnChange: false);

        return builder;
    }

    internal static ConfigurationBuilder AppendJsonFile(ConfigurationBuilder builder, string jsonFileName, string extension)
    {
        var environmentName = EnvironmentInstance.EnvironmentName;

        var isDev = EnvironmentInstance.IsDev;

        //// TODO: does this appsettings.js2on find the appsettings.json every time, even locally in dev without copy to output (bin)?
        //// Else 'AppRootInstance.RootPath' + appsettings.json should be sufficient
        builder.AddJsonFile(jsonFileName + extension, optional: true, reloadOnChange: false);

        if (environmentName.Is())
            builder.AddJsonFile(jsonFileName + "." + environmentName + extension, optional: true, reloadOnChange: false);

        return builder;
    }

    internal static ConfigurationBuilder AddUserSecrets(ConfigurationBuilder builder)
    {
        builder.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);

        return builder;
    }
}
