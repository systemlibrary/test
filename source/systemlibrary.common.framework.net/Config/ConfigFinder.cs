using Microsoft.Extensions.Configuration;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal static class ConfigFinder
{
    internal static IConfiguration FindByType(Type type)
    {
        var configurationName = type.Name.ToLower();

        if (AppConfigInstance.Configurations.ContainsKey(configurationName))
            return AppConfigInstance.Configurations[configurationName];
        
        return AppConfigInstance.Configurations["appsettings"];
    }
}
