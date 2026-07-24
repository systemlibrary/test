using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    protected static async Task WriteResponseAsync(HttpContext context)
    {
        if (context.Response.StatusCode < 1 || context.Response.StatusCode == 200)
            context.Response.StatusCode = 404;

        var method = context.Request.Method;
        var path = context.Request.Path.Value;
        var query = context.Request.QueryString.Value;

        var headers = context.Request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        string body = null;
        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(context.Request.Body);
            body = await reader.ReadToEndAsync();
        }

        var responseObj = new
        {
            Method = method,
            Path = path,
            Query = query,
            Headers = headers,
            Body = body
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(responseObj));
    }
}
