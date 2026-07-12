using Microsoft.AspNetCore.DataProtection;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class ServiceProviderInstance
{
    internal static IServiceProvider Current;

    static ServiceProviderInstance()
    {
        Boot.Strap();
    }
}
