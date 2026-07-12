using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.Boostrap;

static class HttpContextBoot
{
    internal static void Strap() { }

    static HttpContextBoot()
    {
        HttpContextInstance.HttpContextAccessor = ServiceProviderInstance.Current.GetRequiredService<IHttpContextAccessor>();
    }
}
