using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppConfigInstance
{
    internal static IConfigurationRoot AppSettings;

    internal static IDictionary<string, IConfigurationRoot> Configurations;

    static AppConfigInstance()
    {
        Boot.Strap();
    }
}
