using Microsoft.AspNetCore.DataProtection;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class ServicesInstance
{
    internal static IServiceProvider ServiceProvider;

    internal static IDataProtector DataProtector;

    static ServicesInstance()
    {
        Boot.Strap();
    }
}
