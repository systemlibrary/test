using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

internal static class MetricsAuthorizationMiddleware
{
    internal static bool AuthorizeMetricsRequest(HttpContext context)
    {
        var operationalKey = FrameworkSettingsInstance.Current.Access.OperationalKey;

        if (operationalKey == null) return false;

        var token = context.Request.Headers["slcf-operational-key"].FirstOrDefault().ToString();

        if (operationalKey == "" || operationalKey == token)
        {
            FrameworkLog.Debug("[Metrics] authorized");

            return true;
        }

        // Give user option to prompt for an Auth?
        //context.Response.Headers["WWW-Authenticate"] = "Basic";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "text/plain";
        context.Response.WriteAsync(StatusCodes.Status401Unauthorized.ToString() + ": Metric endpoint requires access through the 'slcf-metrics-secretKeyForUI' header. The value required is set by the application developers.")
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        FrameworkLog.Debug("[MetricsMiddleware] unauthorized");

        return false;
    }
}