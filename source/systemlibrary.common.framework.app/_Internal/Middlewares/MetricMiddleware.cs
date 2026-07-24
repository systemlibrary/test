using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App;

static class MetricMiddleware
{
    // TODO: Count all requests and their responses and various operations, static file request or api request or document or media...
    // together it gives an image of how many successful requests vs non successful. 
    // We log status codes... then "fine grain", do we care if things are 401, 402, ... do we care about 503, 504, 505, 502...
    // Do we care about partials... Which status codes ... together with "concurrent users", "uaers last 5 min", "requests last 5 min", "memory usage", together with pie charts...
    // similar to this https://github.com/xabaril/aspnetcore.diagnostics.healthchecks or just this?
    public static RequestDelegate Build(RequestDelegate next)
    {
        return context =>
        {
            return next(context);
        };
    }
}
