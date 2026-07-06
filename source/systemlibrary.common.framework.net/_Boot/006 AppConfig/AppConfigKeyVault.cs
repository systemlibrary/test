using System.Text;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using Microsoft.Extensions.Configuration;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Boostrap;

internal static class AppConfigKeyVault
{
    static string KeyVaultUrl;
    static SecretClient Client;

    static AppConfigKeyVault()
    {
        KeyVaultUrl = AppConfigTemp.Temp["systemLibraryCommonFramework:config:keyVaultUrl"];

        if (KeyVaultUrl.IsNot())
        {
            //  FrameworkLog.Debug("'systemLibraryCommonFramework:config:keyVaultUrl' is not configured.");
        }
        else
        {
            //FrameworkLog.Debug($"'systemLibraryCommonFramework:config:keyVaultUrl' is configured: {keyVaultUrl.MaxLength(16)}.");
        }

        var options = new SecretClientOptions();

        var isDev = EnvironmentInstance.IsDev;

        if (isDev)
        {
            options.Retry.MaxRetries = 0;
            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);
        }

        else
        {
            options.Retry.Delay = TimeSpan.FromMilliseconds(1000);
            options.Retry.MaxRetries = 2;
            options.Retry.MaxDelay = TimeSpan.FromSeconds(15);
            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);
        }

        Client = new SecretClient(new Uri(KeyVaultUrl), new DefaultAzureCredential(), options);
    }

    internal static void Add(ConfigurationBuilder builder, string fileName)
    {
        if (KeyVaultUrl.IsNot()) return;

        var fetched = Fetch(fileName);

        if (fetched.Configs != null && fetched.Configs.Any())
            builder.AddInMemoryCollection(fetched.Configs);

        if (fetched.Transformations != null && fetched.Transformations.Any())
            builder.AddInMemoryCollection(fetched.Transformations);
    }

    internal static (IEnumerable<KeyValuePair<string, string>> Configs, IEnumerable<KeyValuePair<string, string>> Transformations) Fetch(string fileName)
    {
        var environmentName = EnvironmentInstance.EnvironmentName;

        IEnumerable<KeyValuePair<string, string>> configs = null;
        IEnumerable<KeyValuePair<string, string>> transformations = null;

        try
        {
            // TODO: Use "Methods.Parallell" to grab two and two secrets in an efficient way
            var configFromVault = Client.GetSecret(fileName + ".json")?.Value?.Value;

            var transformationsFromVault = Client.GetSecret(fileName + "." + environmentName + ".json")?.Value?.Value;

            if (configFromVault != null)
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(configFromVault));

                configs = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build()
                    .AsEnumerable()
                    .ToArray();
            }

            if (transformationsFromVault != null)
            {
                using var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(transformationsFromVault));

                transformations = new ConfigurationBuilder()
                    .AddJsonStream(stream2)
                    .Build()
                    .AsEnumerable()
                    .ToArray();
            }
        }
        catch (Exception ex)
        {
            // Log.Warning("Exception thrown when reading keyvault: " + ex.Message.MaxLength(255) + " Trying to continue, as runtime might just be a 'tool' or similar like CLI EF migrations...");
        }

        return (configs, transformations);
    }
}
