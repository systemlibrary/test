using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.Bootstrap;

static class ContextAccessorBoot
{
    internal static void Strap() { }

    static ContextAccessorBoot()
    {
        ContextAccessorInstance.HttpContextAccessor = ServicesInstance.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    }
}
