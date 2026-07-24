using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using Prometheus;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Licensing;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    static DateTime? MetricLastReturned;
    static string MetricViewCached;
    static object MetricReadLock = new object();
    const int MetricCacheDuration = 20;

    internal static void UseMapMetrics(this IEndpointRouteBuilder endpoints)
    {
        var isMetricsEnabled = MetricsInstance.Enabled;

        if (!isMetricsEnabled)
        {
            FrameworkLog.Debug("[Metrics] disabled, continuing...");

            return;
        }

        FrameworkLog.Debug($"[Metrics] added {MetricsInstance.Path} and {MetricsInstance.PathUI} at url {MetricsInstance.Url}");

        Metrics.SuppressDefaultMetrics();

        MapMetrics(endpoints);

        MapMetricsUi(endpoints);
    }

    static void MapMetricsUi(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(MetricsInstance.PathUI, async context =>
        {
            if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context)) return;

            if (!License.Pro())
            {
                FrameworkLog.Debug("[Metrics] enabled, but UI for metrics requires gold tier license or above");

                context.Response.ContentType = "text/html";

                await context.Response.WriteAsync("[Metrics] enabled, but UI for metrics requires gold tier license or above");

                return;
            }

            if (MetricViewCached.IsNot() || !(MetricLastReturned > DateTime.Now.AddSeconds(-MetricCacheDuration)))
            {
                FrameworkLog.Debug("[Metrics] recalculating");

                var data = MetricsFetcher.Get(context);

                MetricViewCached = MetricsUI.GetHtmlView(data);
            }
            else
            {
                FrameworkLog.Debug("[Metrics] ui returned from " + MetricCacheDuration + "s cache");
            }

            context.Response.ContentType = "text/html";

            await context.Response.WriteAsync(MetricViewCached);
        });
    }

    static void MapMetrics(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(MetricsInstance.Path, async context =>
        {
            if (!MetricsAuthorizationMiddleware.AuthorizeMetricsRequest(context)) return;

            if (context.Request.Headers.ContainsKey("slcf-metrics-metricsfetcher"))
            {
                lock (MetricReadLock)
                {
                    if (MetricLastReturned > DateTime.Now.AddSeconds(-MetricCacheDuration))
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.StatusCode = 200;
                        context.Response.WriteAsync("")
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        return;
                    }
                    MetricLastReturned = DateTime.Now;
                }
            }

            try
            {
                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync("");

                await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                throw;
            }
        });
    }
}