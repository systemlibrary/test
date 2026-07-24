using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class HttpContextInstance
{
    internal static IHttpContextAccessor HttpContextAccessor;

    static HttpContextInstance()
    {
        Boot.Strap();
    }
}
