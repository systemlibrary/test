using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

internal class StatusCodeLoggerMiddleware
{
    RequestDelegate next;

    public StatusCodeLoggerMiddleware(RequestDelegate next) => this.next = next;

    public async Task InvokeAsync(HttpContext ctx)
    {
        await next(ctx);

        var code = ctx?.Response?.StatusCode ?? 0;

        if (code < 299) return;

        var overrideCacheControl = false;

        if (code > 399)
        {
            if (ctx.Response.Headers.ContainsKey("Cache-Control"))
            {
                overrideCacheControl = true;
                ctx.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Response.Headers["Pragma"] = "no-cache";
                ctx.Response.Headers["Expires"] = "0";
            }
        }

        var path = ctx.Request.Path.Value;

        if (path.EndsWith("chrome.devtools.json")) return;
        if (path.EndsWith(".map")) return;

        var msg = $"[{code}] {ctx.Request.Method} {ctx.Request.Path}" + (overrideCacheControl ? " (forced Cache-Control: no-cache)" : "");

        if (code >= 500)
            Log.Error(msg);
        else if (code >= 400)
            Log.Warning(msg);
        else
            Log.Information(msg);
    }
}
