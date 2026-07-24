using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Static class for configuring the appearance of pie charts on the <c>/metrics/ui</c> endpoint.<br/>
/// Default values apply to all charts unless overridden per label via <c>Add</c>.
/// </summary>
public static class MetricCharts
{
    public static bool DefaultShowAnimation = true;
    public static bool DefaultShowLegend = true;
    public static bool DefaultShowBorder = true;
    public static string DefaultTextColor = "#121212";

    internal static ConcurrentDictionary<string, MetricChartOption> MetricOptions = new();

    /// <summary>
    /// Registers a <c>MetricChartOption</c> for a specific metric label.<br/>
    /// Overwrites any existing option registered for the same label.
    /// </summary>
    /// <example>
    /// <code>
    /// MetricCharts.Add(new MetricChartOption
    /// {
    ///     MetricLabel = "cache",
    ///     ShowLegend = false,
    ///     TextColor = "#ffffff"
    /// });
    /// </code>
    /// </example>
    public static void Add(MetricChartOption option = null)
    {
        if (option?.MetricLabel == null) return;

        var key = option.MetricLabel;

        if (MetricOptions.ContainsKey(key))
            MetricOptions.Remove(key, out _);

        MetricOptions.TryAdd(key, option);
    }
}