using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App;

internal static class Compress
{
    const long MinThreshold = 384;
    const long MaxThreshold = 50 * 1024 * 1024;

    internal static bool IsEligibleForCompression(HttpContext context)
    {
        if (context?.Request == null) return false;

        if (context.WebSockets?.IsWebSocketRequest == true) return false;

        if (context.Response?.Headers?.ContainsKey("Content-Encoding") == true) return false;

        if (context.Request.Headers == null) return true;

        var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();

        if (acceptEncoding.IsNot() || acceptEncoding.Contains("br") || acceptEncoding.Contains("gzip"))
        {
            if (context.Response.ContentLength.HasValue &&
                (
                context.Response.ContentLength.Value < MinThreshold ||
                context.Response.ContentLength.Value > MaxThreshold)
                )
                return false;

            return true;
        }

        return false;
    }
}
