using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App;

static class SecurityPolicyMiddleware
{
    static string _csp = null;

    public static RequestDelegate Build(RequestDelegate next, BranchType branchType)
    {
        if (EnvironmentConfig.IsDevelopment) return next;

        switch (branchType)
        {
            case BranchType.Web: return WebSecurityPolicy(next);
            case BranchType.Api: return ApiSecurityPolicy(next);
            case BranchType.Static: return StaticSecurityPolicy(next);
        }
        return next;
    }

    static RequestDelegate WebSecurityPolicy(RequestDelegate next)
    {
        return async context =>
        {
            var headers = context?.Response?.Headers;
            if (headers == null)
            {
                await next(context);
                return;
            }

            headers["X-Content-Type-Options"] = "nosniff";
            headers["Strict-Transport-Security"] = "max-age=2592000; includeSubDomains";

            if (context.Response.ContentType?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (_csp == null)
                {
                    var hostName = AppInstance.HostName;
                    if (hostName.IsNot())
                        hostName = context.Request.Url(); //.GetPrimaryDomain();

                    _csp = hostName.Is()
                        ? "default-src * data: blob: 'unsafe-inline' 'unsafe-eval'; base-uri 'self'; object-src 'none'; frame-ancestors 'self' https://*." + hostName + ";"
                        : "default-src * data: blob: 'unsafe-inline' 'unsafe-eval'; base-uri 'self'; object-src 'none'; frame-ancestors 'self';";
                }

                headers["Content-Security-Policy"] = _csp;
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=(), bluetooth=(), interest-cohort=()";
            }

            await next(context);
        };
    }

    static RequestDelegate ApiSecurityPolicy(RequestDelegate next)
    {
        return async context =>
        {
            var headers = context?.Response?.Headers;
            if (headers == null)
            {
                await next(context);
                return;
            }

            headers["X-Content-Type-Options"] = "nosniff";
            headers["Strict-Transport-Security"] = "max-age=2592000; includeSubDomains";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            await next(context);
        };
    }

    static RequestDelegate StaticSecurityPolicy(RequestDelegate next)
    {
        return async context =>
        {
            var headers = context?.Response?.Headers;

            if (headers != null)
            {
                headers["X-Content-Type-Options"] = "nosniff";
                headers["Strict-Transport-Security"] = "max-age=2592000; includeSubDomains";
            }

            await next(context);
        };
    }
}
