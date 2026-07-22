using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

public class LogMessage
{
    static string[] CorrelationIdKeys =
    [
        "correlationId",
        "CorrelationId",
        "CorrelationID",
        "Correlation-ID",
        "correlation_id",
        "correlationid"
    ];

    [Display(Order = 1)]
    public DateTimeOffset? Timestamp;

    [Display(Order = 5)]
    public LogLevel Level;

    [Display(Order = 10)]
    public object Message;

    [Display(Order = 15)]
    public string CorrelationId;

    [Display(Order = 20)]
    public bool? IsAuthenticated;

    [Display(Order = 25)]
    public string Url;

    [Display(Order = 35)]
    public string BrowserName;

    [Display(Order = 999)]
    public StackTrace StackTrace;

    public LogMessage(LogLevel level, object[] messages)
    {
        Level = level;
        if (messages.Length == 1)
            Message = messages[0];
        else
            Message = messages;

        if (level != LogLevel.Dump)
        {
            Timestamp = DateTimeOffset.UtcNow;

            if (level >= LogLevel.Warning)
            {
                var httpContext = ServiceProviderInstance.Current.GetService<IHttpContextAccessor>()?.HttpContext;

                CorrelationId = GetCorrelationId(httpContext);

                if (level == LogLevel.Error ||
                    level == LogLevel.Critical)
                {
                    Url = GetUrl(httpContext?.Request);
                    IsAuthenticated = GetIsAuthenticated(httpContext);
                    BrowserName = GetBrowserName(httpContext?.Request);
                    StackTrace = new StackTrace(1, EnvironmentInstance.IsDev);
                }
            }
            else
            {
                if (!EnvironmentInstance.IsDev)
                {
                    var httpContext = ServiceProviderInstance.Current.GetService<IHttpContextAccessor>()?.HttpContext;

                    CorrelationId = GetCorrelationId(httpContext);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static string GetUrl(HttpRequest request)
    {
        if (request == null) return null;

        var url = UriHelper.GetDisplayUrl(request);

        // TODO: Will GetDisplayUrl ever return such urls with :: ?
        //if (url.Contains("[::]"))
        //{
        //    url = url.Replace("[::]", "localhost");
        //    url = url + ", " + url.Trim().Replace("localhost", "127.0.0.1") + " ([::])";
        //}
        return url;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static string GetBrowserName(HttpRequest request)
    {
        var userAgent = request?.Headers[HeaderNames.UserAgent].FirstOrDefault();

        if (userAgent == null) return null;

        if (userAgent.Length < 5) return "";

        if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)) return "Safari";
        if (userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)) return "Edge";
        if (userAgent.Contains("Brave", StringComparison.OrdinalIgnoreCase)) return "Brave";
        if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)) return "Chrome";
        if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)) return "Firefox";
        if (userAgent.Contains("OPR", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase)) return "Opera";

        return "unknown";
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static bool? GetIsAuthenticated(HttpContext httpContext)
    {
        if (httpContext?.User?.Identity == null) return null;

        return httpContext.User.Identity.IsAuthenticated == true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static string GetCorrelationId(HttpContext httpContext)
    {
        var items = httpContext?.Items;

        if (items == null) return null;

        try
        {
            object id = null;

            foreach (var key in CorrelationIdKeys)
            {
                if (httpContext.Items.TryGetValue(key, out id))
                    break;
            }

            if (id == null)
            {
                const string name = "CorrelationId";

                id = Randomness.Bytes(8).ToBase62();

                httpContext.Items.TryAdd(name, id);
            }

            return (string)id;
        }
        catch
        {
            return "unknown";
        }
    }
}
