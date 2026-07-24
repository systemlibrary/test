namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Controls the appearance of a metric pie chart registered via <c>Metric.Init</c>.<br/>
/// A pie chart is uniquely identified by label + category + status — registering the same combination overwrites the existing one.
/// </summary>
public class MetricChartOption
{
    /// <summary>
    /// Matches the label used in <c>Metric.Inc("label")</c>. Set to null to apply as default for all pie charts.
    /// </summary>
    public string MetricLabel;

    /// <summary>
    /// Show animation when the pie chart loads.
    /// </summary>
    public bool ShowAnimation = MetricCharts.DefaultShowAnimation;

    /// <summary>
    /// Show the legend toolbar above the pie chart — hides the display label when enabled.
    /// </summary>
    public bool ShowLegend = MetricCharts.DefaultShowLegend;

    /// <summary>
    /// Show a border between each slice.
    /// </summary>
    public bool ShowBorder = MetricCharts.DefaultShowBorder;

    /// <summary>
    /// Text color for the pie chart. Supports #hex and <c>rgb(0,0,0)</c> format.
    /// </summary>
    public string TextColor = MetricCharts.DefaultTextColor;

    /// <summary>
    /// Per-slice styling options.
    /// </summary>
    public SliceOption[] Slices;
}