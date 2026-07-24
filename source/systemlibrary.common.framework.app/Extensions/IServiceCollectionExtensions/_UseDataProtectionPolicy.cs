using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    public static IServiceCollection UseDataProtectionPolicy(this IServiceCollection services)
    {
        if (!FrameworkOptionsInstance.Current.UseDataProtectionPolicy) return services;

        var keyManagementOptionsType = typeof(IConfigureOptions<KeyManagementOptions>);

        var keyManagementOptions = services.FirstOrDefault(sd => sd.ServiceType == keyManagementOptionsType);

        if (keyManagementOptions != null)
        {
            Log.Warning("UseDataProtectionPolicy is set to True, but AddDataProtection() has already been registered, doing nothing...");

            return services;
        }

        var name = AppInstance.Name;

        FrameworkLog.Debug("[UseDataProtectionPolicy] Added and activated");

        return services.AddDataProtection()
            .SetApplicationName(name)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100))
            .Services;
    }
}