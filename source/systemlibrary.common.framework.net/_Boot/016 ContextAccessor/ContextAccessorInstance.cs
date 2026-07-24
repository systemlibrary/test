using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class ContextAccessorInstance
{
    internal static IHttpContextAccessor HttpContextAccessor;

    static ContextAccessorInstance()
    {
        Boot.Strap();
    }
}
