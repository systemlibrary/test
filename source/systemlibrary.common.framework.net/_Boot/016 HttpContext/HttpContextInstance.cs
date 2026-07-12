using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class HttpContextInstance
{
    internal static IHttpContextAccessor HttpContextAccessor;

    static HttpContextInstance()
    {
        Boot.Strap();
    }
}
