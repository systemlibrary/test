using System.Text;

using Microsoft.AspNetCore.Http;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App;

internal static class MetricsFetcher
{
    internal static string Get(HttpContext context)
    {
        var url = MetricsInstance.Url;

        if (url.IsNot())
        {
            var origin = context.Request.Headers["Origin"].FirstOrDefault();

            var forwardedHost = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault();

            var host = forwardedHost ?? origin ?? context.Request.Host.Value;

            if (host.IsNot())
                host = context.Request.Host.Value;

            url = $"{context.Request.Scheme}://{host}";
        }

        url = url + MetricsInstance.Path;

        var sb = new StringBuilder("");

        var operationalKey = FrameworkSettingsInstance.Current.Access.OperationalKey;

        // TODO:
        // There might be a proxy in between, where this application runs in a different domain, calling itself which is then blocked
        // Could therefore add a setting to Metrics - the full blown url, so the requests goes 'out' and 'back in', hopefully hitting the endpoint cleanly
        // if response - all fails, grab just the ones we already have in-memory of the serving node (partially implmented with url setting)
        // TODO: Add a mark, counter, in the UI saying how many servers was part of the response, knowing "this works", but also giving 
        // a mean to notice the developers that their LB is working somewhat and that they do have additional nodes running at this time
        var responses = Methods.Parallel<string>(
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey),
            () => GetMetricsResponse(url, operationalKey)
        );

        if (responses != null)
        {
            foreach (var response in responses)
            {
                if (response?.Length > 10)
                {
                    sb.AppendLine("\n");
                    sb.Append(response);
                }
            }
        }

        return sb.ToString();
    }

    static string GetMetricsResponse(string url, string operationalKey)
    {
        var handler = new SocketsHttpHandler
        {
            UseProxy = false
        };

        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5000));
        var client = new HttpClient(handler);

        client.Timeout = TimeSpan.FromMilliseconds(5000);

        client.DefaultRequestHeaders.Add("slcf-metrics-metricsfetcher", "true");

        if (operationalKey.Is())
            client.DefaultRequestHeaders.Add("slcf-operational-key", operationalKey);

        try
        {
            using (client)
            {
                return
                    client.GetStringAsync(url)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
        catch (Exception ex)
        {
            Log.Error("[MetricsFetcher] exception occured: " + ex.Message);

            return null;
        }
    }
}
