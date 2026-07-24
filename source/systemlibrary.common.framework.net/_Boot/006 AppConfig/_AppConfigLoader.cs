using Microsoft.Extensions.Configuration;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.Bootstrap;

static class AppConfigLoader
{
    internal static IDictionary<string, IConfigurationRoot> Load(string[] configurationFiles)
    {
        var configurationGroups = GetConfigurationGroups(configurationFiles);

        if (configurationFiles.Length > configurationGroups.Count)
        {
            //FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations and transformations");
        }
        else
        {
            // FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is {EnvironmentInstance.EnvironmentType}, loading configurations");
        }

        if (!configurationGroups.Any(x => x.Name == "appsettings"))
        {
            var appsettingsGroup = new AppConfigGroup
            {
                Name = "appsettings",
                Extension = "json"
            };
            configurationGroups.Add(appsettingsGroup);
        }

        var actions = configurationGroups
            .Where(x => x?.Name.Is() == true)
            .Select(ToLoadAction)
            .ToArray();

        var results = Methods.Parallel(45000, actions);
        
        var configurations = results
            .Where(x => x.Config != default)
            .ToDictionary(x => x.group.Name, x => x.Config, StringComparer.OrdinalIgnoreCase);

        return configurations;
    }

    static Func<(AppConfigGroup group, IConfigurationRoot Config)> ToLoadAction(AppConfigGroup configurationGroup)
    {
        return () => (
            group: configurationGroup,
            Config: LoadConfig(configurationGroup)
        );
    }

    internal static IConfigurationRoot LoadConfig(AppConfigGroup group)
    {
        try
        {
            var builder = AppConfigBuilder.CreateDefaultBuilder();

            var isDev = EnvironmentInstance.IsDev;

            var fileName = group.Name;

            var extension = group.Extension;

            if (isDev)
            {
                if (extension.Contains("xml", StringComparison.OrdinalIgnoreCase))
                    AppConfigBuilder.AppendXmlFile(builder, fileName, extension);
                else
                {
                    AppConfigBuilder.AppendKeyVault(builder, fileName, extension);

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

                    AppConfigBuilder.AppendKeyVault(builder, fileName, extension);
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

    static List<AppConfigGroup> GetConfigurationGroups(string[] configurationFiles)
    {
        return configurationFiles
            .Select(file => new AppConfigGroup
            {
                FullPath = file,
                Name = Path.GetFileNameWithoutExtension(file),
                Extension = Path.GetExtension(file)
            })
            .Where(x => !x.Name.Contains('.'))
            .DistinctBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
