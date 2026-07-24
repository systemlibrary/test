using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseForwardedHeaders(this IServiceCollection services)
    {
        if (!FrameworkOptionsInstance.Current.UseForwardedHeaders) return services;

        return services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

            options.KnownProxies.Clear();

            if (EnvironmentConfig.IsDevelopment)
            {
                options.KnownIPNetworks.Clear();
            }
            else
            {
                var forwardedIPNetworks = FrameworkSettingsInstance.Current.Network.ForwardedIPNetworks;

                if (forwardedIPNetworks.IsNot())
                    throw new Exception(
                       "UseForwardedHeaders: is true in a non-development environment, while systemLibraryCommonFramework:app:forwardedNetworks is not set. " +
                       "Use '*' for all networks or a comma-separated list of CIDRs. Cannot continue. Specify the setting in appsettings or set environment to development, which during development this error wont occur"
                   );

                if (forwardedIPNetworks == "*")
                {
                    options.KnownProxies.Clear();
                    options.KnownIPNetworks.Clear();
                }
                else
                {
                    var networks = forwardedIPNetworks.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var network in networks)
                    {
                        var trimmed = network.Trim();

                        if (trimmed.IsNot()) continue;

                        var parts = trimmed.Split('/');
                        if (parts.Length != 2)
                            throw new Exception(
                                $"Network '{trimmed}' is invalid. Must include CIDR, e.g. ending in '/32' for single IP.");

                        var ip = IPAddress.Parse(parts[0]);
                        var prefix = int.Parse(parts[1]);

                        options.KnownIPNetworks.Add(new System.Net.IPNetwork(ip, prefix));
                    }
                };
            }
        });
    }
}