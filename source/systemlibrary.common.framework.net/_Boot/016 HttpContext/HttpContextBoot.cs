using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.Bootstrap;

static class HttpContextBoot
{
    internal static void Strap() { }

    static HttpContextBoot()
    {
        HttpContextInstance.HttpContextAccessor = ServicesInstance.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    }
}
