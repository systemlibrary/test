using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework.Boostrap;

static class AppConfigLoader
{
    internal static IDictionary<string, IConfigurationRoot> Load(string[] configurationFiles)
    {
        var configurationGroups = GetConfigurationGroups(configurationFiles);

        if (configurationFiles.Length > configurationGroups.Length)
        {
            //FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations and transformations");
        }
        else
        {
            // FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations");
        }

        var actions = configurationGroups
            .Where(x => x.Is())
            .Select(ToLoadAction)
            .ToArray();

        var results = Methods.Parallel(45000, actions);

        var configurations = results
            .Where(x => x.Config != default)
            .ToDictionary(x => x.Name, x => x.Config, StringComparer.OrdinalIgnoreCase);

        if(!configurations.ContainsKey("appsettings"))
        {
            //FrameworkDebug.Log("appsettings not found, nor a default one was created. Youve created json configurations, but main 'appsettings.json' is not created.");
        }

        return configurations;
    }

    static Func<(string Name, IConfigurationRoot Config)> ToLoadAction(string configurationFile)
    {
        return () => (
            Name: configurationFile.ToLower(),
            Config: LoadConfig(configurationFile)
        );
    }

    internal static IConfigurationRoot LoadConfig(string configFileName)
    {
        try
        {
            var builder = AppConfigBuilder.CreateDefaultBuilder();

            var isDev = EnvironmentInstance.IsDev;

            var fileName = Path.GetFileName(configFileName);

            var extension = Path.GetExtension(configFileName);

            if (isDev)
            {
                if (extension.Contains("xml", StringComparison.OrdinalIgnoreCase))
                    AppConfigBuilder.AppendXmlFile(builder, fileName, extension);
                else
                {
                    AppConfigBuilder.AppendKeyVault(builder, configFileName);

                    AppConfigBuilder.AppendJsonFile(builder, fileName, extension);
                }
            }
            else
            {
                if (extension.Contains("xml", StringComparison.OrdinalIgnoreCase))
                    AppConfigBuilder.AppendXmlFile(builder, fileName, extension);
                else
                {
                    AppConfigBuilder.AppendJsonFile(builder, fileName, extension);

                    AppConfigBuilder.AppendKeyVault(builder, configFileName);
                }
            }

            AppConfigBuilder.AddUserSecrets(builder);

            AppConfigBuilder.AddEnvironmentVariables(builder);

            return builder.Build();
        }
        catch (Exception ex)
        {
            // Log.Error(ex);
            return default;
        }
    }

    static string[] GetConfigurationGroups(string[] configurationFiles)
    {
        return configurationFiles
          .Select(Path.GetFileNameWithoutExtension)
          .Where(name => !name.Contains('.'))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .ToArray();
    }

}
