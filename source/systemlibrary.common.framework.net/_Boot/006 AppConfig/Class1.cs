//using System.Text;

//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;

//using Microsoft.Extensions.Configuration;

//using SystemLibrary.Common.Framework.Boostrap;

//internal static class AppSettingsLoader
//{
//    {
//        if (string.IsNullOrEmpty(keyVaultUrl))
//        {
//            //FrameworkLog.Debug("KeyVaultUrl in appsettings is null/empty");
//            return;
//        }

//        try
//        {
//            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

//            var secret = client.GetSecret("appsettings")?.Value?.Value;

//            if (secret == null)
//            {
//                Log.Warning("KeyVault missing 'appsettings' at " + keyVaultUrl);
//                return;
//            }

//            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(secret));

//            var kvConfig = new ConfigurationBuilder()
//                .AddJsonStream(stream)
//                .Build();
//            var properties = kvConfig.AsEnumerable();

//            builder.AddInMemoryCollection(properties);
//        }
//        catch(Exception ex)
//        {
//            Log.Warning("Exception thrown when reading keyvault: " + ex.Message.MaxLength(255) + " Trying to continue, as runtime might just be a 'tool' or similar like CLI EF migrations...");
//        }
//    }
//}
