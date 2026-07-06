using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework.Boostrap;

static class AppConfigLoader
{
    internal static IConfigurationRoot LoadAppSettings(string[] configurationFiles)
    {
        var builder = AppConfigBuilder.CreateDefaultBuilder();

        if (configurationFiles == null || configurationFiles.Length == 0)
            return AppConfigBuilder.AddEnvironmentVariables(builder).Build();

        var isDev = EnvironmentInstance.IsDev;

        if (isDev)
        {
            AppConfigBuilder.AppendKeyVault(builder, "appsettings");

            AppConfigBuilder.AppendJsonFile(builder, "appsettings");
        }
        else
        {
            AppConfigBuilder.AppendJsonFile(builder, "appsettings");

            AppConfigBuilder.AppendKeyVault(builder, "appsettings");
        }

        AppConfigUserSecrets.Add(builder);

        AppConfigBuilder.AddEnvironmentVariables(builder);

        return builder.Build();
    }

    internal static IDictionary<string, IConfigurationRoot> Load(string[] configurationFiles)
    {
        var configurations = new Dictionary<string, IConfigurationRoot>();

        if (configurationFiles == null || configurationFiles.Length == 0)
        {
            ///AddDefaultAppSettingsIfMissing(configurations);

            return configurations;
        }

        var environment = EnvironmentInstance.EnvironmentName;

        var configurationGroups = configurationFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !name.Contains('.'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (configurationFiles.Length > configurationGroups.Length)
        {
            //FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations and transformations");
        }
        else
        {
           // FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations");
        }

        IConfigurationRoot appSettings = null;

        string userSecretsId = null;

        foreach (var group in configurationGroups)
        {
            // TODO: Could optimize this slightly, to avoid "duplicate" read of appsettings.json and its env vars:
            // group == "appsettings" && ServiceLocator.HasService<IConfiguration>()
            // then IConfiguration has already read appsettings.json, environmentvaraibles and whatnot...
            // because we do call: hostBuilder.ConfigureAppConfiguration(...)
            if (group.Equals("appsettings", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    appSettings = ConfigurationBuild(configurationFiles, group, environment);

                    userSecretsId = appSettings["systemLibraryCommonFramework:config:userSecretsId"];

                    if (userSecretsId.Is())
                        appSettings = ConfigurationBuild(configurationFiles, group, environment, userSecretsId);
                }
                catch (Exception ex)
                {
                    //BootstrapLog.Dump("Error: loading and reading appsettings.json:" + group + ". " + ex.Message);
                    throw;
                }

                break;
            }
        }

        var actions = configurationGroups
            .Where(x => !x.Equals("appsettings", StringComparison.OrdinalIgnoreCase))
            .Select(name => new Func<(string Name, IConfigurationRoot Config)>(() =>
                (Name: name.ToLower(),
                 Config: ConfigurationBuild(configurationFiles, name, environment, userSecretsId))
            ))
            .ToArray();

        var results = Methods.Parallel(10000, actions);

        configurations = results
                .Where(x => x != default)
                .ToDictionary(x => x.Name, x => x.Config, StringComparer.OrdinalIgnoreCase);

        if (appSettings != null)
            configurations["appsettings"] = appSettings;

        //AddDefaultAppSettingsIfMissing(configurations);

        return configurations;
    }

    static IConfigurationRoot ConfigurationBuild(string[] configurationFiles, string group, string environment, string userSecretsId = null)
    {
        var builder = new ConfigurationBuilder();

        foreach (var configurationFile in configurationFiles)
        {
            AddConfigurationFile(builder, configurationFile, group, null);
        }

        if (environment.Is())
        {
            foreach (var configurationFile in configurationFiles)
            {
                AddConfigurationFile(builder, configurationFile, group, environment);
            }
        }

        if (group.Equals("appsettings", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddEnvironmentVariables(prefix: "ASPNETCORE_");

            builder.AddEnvironmentVariables(prefix: "DOTNET_");
        }

        builder.AddEnvironmentVariables(prefix: "APP_");

        if (userSecretsId.Is())
            builder.AddUserSecrets(userSecretsId);

        return builder.Build();
    }

    static void AddConfigurationFile(ConfigurationBuilder builder, string configurationFile, string fileName, string environment = null)
    {
        if (!configurationFile.Contains("/" + fileName + ".", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var checkTransformation = environment.Is();

        if (checkTransformation)
        {
            if (!configurationFile.Contains(fileName + "." + environment + ".", StringComparison.OrdinalIgnoreCase))
                return;
        }
        else
        {
            if (!configurationFile.EndsWith(fileName + ".json", StringComparison.OrdinalIgnoreCase) &&
                !configurationFile.EndsWith(fileName + ".xml", StringComparison.OrdinalIgnoreCase) &&
                !configurationFile.EndsWith(fileName + ".config", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        try
        {
            AddConfigurationFile(builder, configurationFile, fileName);
        }
        catch (Exception ex)
        {
            //BootstrapLog.Dump("Error: loading and reading " + configurationFile + ": " + ex.Message);
        }
    }

    static void AddConfigurationFile(ConfigurationBuilder builder, string configurationFile, string fileName)
    {
        if (configurationFile.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddJsonFile(configurationFile, true, false);
        }
        else if (configurationFile.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddXmlFile(configurationFile, true, false);
        }
        else if (configurationFile.EndsWith(".config", StringComparison.OrdinalIgnoreCase))
        {
            var data = File.ReadAllText(configurationFile);
            if (data.Length > 0)
            {
                if (data._IsXml())
                    builder.AddXmlFile(configurationFile, true, true);
                else if (data._IsJson())
                    builder.AddJsonFile(configurationFile, true, true);
            }
        }
    }

}
