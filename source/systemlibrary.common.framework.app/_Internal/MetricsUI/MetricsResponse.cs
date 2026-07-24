namespace SystemLibrary.Common.Framework.App;

internal class MetricsResponse
{
    public List<MetricData> Metrics { get; set; } = new List<MetricData>();

    public List<MetricsOptionResponse> Options { get; set; }

    public string DefaultTextColor { get; set; }
    public bool DefaultShowBorder { get; set; }
    public bool DefaultShowAnimation { get; set; }
    public bool DefaultShowLegend { get; set; }
}