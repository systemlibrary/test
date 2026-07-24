namespace SystemLibrary.Common.Framework.Bootstrap;

static class MetricsBoot
{
    internal static void Strap() { }

    static MetricsBoot()
    {
        var settings = FrameworkSettingsInstance.Current.Metrics;

        MetricsInstance.Enabled = settings.Enable;

        var url = NormalizeUrl(settings.Url);
        MetricsInstance.Url = url;

        var path = NormalizePath(settings.Url);
        MetricsInstance.Path = path;

        MetricsInstance.PathUI = NormalizePathUI(path);
    }

    static string NormalizeUrl(string url)
    {
        if (url.IsNot()) return null;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return $"{uri.Scheme}://{uri.Host}:{uri.Port}";

        return null;
    }

    static string NormalizePath(string url)
    {
        if (url.IsNot()) return "/slcf/metrics";

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return uri.AbsolutePath.Is() && uri.AbsolutePath != "/" ? uri.AbsolutePath : "/slcf/metrics";

        return "/slcf/metrics";
    }

    static string NormalizePathUI(string path)
    {
        return path.EndsWith("/")
            ? path + "ui"
            : path + "/ui";
    }
}
