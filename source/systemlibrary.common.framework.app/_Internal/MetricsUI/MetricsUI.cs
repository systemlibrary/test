using System.Text.RegularExpressions;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

internal static class MetricsUI
{
    static string IndexTemplate;

    const int DefaultLastOrder = 9999999;

    static MetricsUI()
    {
        IndexTemplate = Assemblies.GetEmbeddedResource("_Internal/MetricsUI/Index.html");
    }

    internal static string GetHtmlView(string metricsData)
    {
        var data = ParseMetricData(metricsData);

        return IndexTemplate.Replace("//@data@", data.Json());
    }

    internal static string CleanLine(string line)
    {
        return line.Replace("'", "").Trim();
    }

    internal static MetricsResponse ParseMetricData(string metricsData)
    {
        var response = new MetricsResponse();

        response.DefaultTextColor = MetricCharts.DefaultTextColor;
        response.DefaultShowBorder = MetricCharts.DefaultShowBorder;
        response.DefaultShowLegend = MetricCharts.DefaultShowLegend;
        response.DefaultShowAnimation = MetricCharts.DefaultShowAnimation;

        if (metricsData.IsNot()) return response;

        using var reader = new StringReader(metricsData);

        string line;

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("#")) continue;

            line = CleanLine(line);

            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2) continue;

            if (!double.TryParse(parts[^1], out double value)) continue;

            var metricLabel = string.Join(" ", parts, 0, parts.Length - 1);

            var labels = ExtractLabels(metricLabel);

            var existingMetric = FindExistingMetric(response, labels.label, labels.segment);

            if (existingMetric != null)
            {
                existingMetric.Count += (int)value;
            }
            else
            {
                response.Metrics.Add(new MetricData
                {
                    Label = labels.label,
                    Segment = labels.segment,
                    Count = (int)value,
                    Order = GetMetricSliceOrder(labels.label, labels.segment),
                    Color = GetMetricSliceColor(labels.label, labels.segment)
                });
            }
        }

        if (MetricCharts.MetricOptions.Count > 0 && response.Metrics.Count > 0)
        {
            response.Options = new();
  
            foreach (var option in MetricCharts.MetricOptions)
            {
                response.Options.Add(new MetricsOptionResponse
                {
                    label = option.Value.MetricLabel,
                    showAnimation = option.Value.ShowAnimation,
                    showLegend = option.Value.ShowLegend,
                    showBorder = option.Value.ShowBorder,
                    textColor = option.Value.TextColor,
                });
            }
        }
        return response;
    }

    static int GetMetricSliceOrder(string label, string segment)
    {
        var key = label;

        if (!MetricCharts.MetricOptions.ContainsKey(key)) return DefaultLastOrder;

        var option = MetricCharts.MetricOptions[label];

        var slice = option?.Slices?.FirstOrDefault(s => s.Segment == segment);

        return slice?.Order ?? DefaultLastOrder;
    }

    static string GetMetricSliceColor(string label, string segment)
    {
        var key = label;

        if (!MetricCharts.MetricOptions.ContainsKey(key)) return null;

        var option = MetricCharts.MetricOptions[label];

        var slice = option?.Slices?.FirstOrDefault(s => s.Segment == segment);

        return slice?.Color;
    }

    static MetricData FindExistingMetric(MetricsResponse response, string label, string segment) =>
        response.Metrics.FirstOrDefault(m =>
            m.Label == label &&
            (m.Segment == segment || (segment.IsNot() && m.Segment.IsNot())));

    static (string label, string segment) ExtractLabels(string metric)
    {
        var regex = new Regex(@"(\w+)=""((?:[^""\\]|\\.)*)""");
        string label = null, segment = null;

        foreach (Match match in regex.Matches(metric))
        {
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            if (key == "label") label = value;

            else if (key == "segment") segment = value;
        }

        return (label ?? "", segment);
    }
}
