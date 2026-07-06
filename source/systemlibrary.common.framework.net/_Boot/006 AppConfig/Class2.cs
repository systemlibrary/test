using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

internal static class AppConfigUserSecrets
{
    internal static ConfigurationBuilder Add(ConfigurationBuilder builder)
    {
        builder.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);

        return builder;
    }
}
