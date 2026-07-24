using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Static accessor for the current <c>HttpContext</c>, equivalent to the classic .NET Framework <c>HttpContext.Current</c>.
/// </summary>
public static class HttpContextInstance
{
    /// <summary>
    /// Returns the current <c>HttpContext</c>, or a default instance if none exists.
    /// </summary>
    /// <remarks>
    /// Never returns null — falls back to a default <c>HttpContext</c> with empty user and current service provider.<br/>
    /// Default instance is returned in console apps, unit tests, or before MVC services are registered.
    /// </remarks>
    /// <example>
    /// <code>
    /// var httpContext = HttpContextInstance.Current;
    /// </code>
    /// </example>
    public static HttpContext Current
    {
        get
        {
            var ctx1 = ContextAccessorInstance.HttpContextAccessor?.HttpContext;

            if (ctx1 == null) return CreateDefaultHttpContext();

            var ctx2 = ContextAccessorInstance.HttpContextAccessor?.HttpContext;

            return ctx1 == ctx2 ? ctx1 : CreateDefaultHttpContext();
        }
    }

    static HttpContext CreateDefaultHttpContext()
    {
        return new DefaultHttpContext()
        {
            RequestServices = ServicesInstance.ServiceProvider,
            User = new System.Security.Claims.ClaimsPrincipal()
        };
    }
}
