using Prometheus;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Metric class count metrics in memory and exposing it to Prometheus or the /metrics/ui endpoint if you are on Pro Tier license
/// </summary>
internal static class Metric
{
    static Counter LabelCounter = Metrics.CreateCounter("total_label", "Total per label",
    new CounterConfiguration { LabelNames = new[] { "label" } });

    static Counter LabelSegmentCounter = Metrics.CreateCounter("total_label_segment", "Total per label and segment",
        new CounterConfiguration { LabelNames = new[] { "label", "segment" } });

    /// <summary>
    /// Increase a metric by 1 for a specific label and segment
    /// </summary>
    /// <param name="label">Name of the Metric, 'The Pie Chart' if you will</param>
    /// <param name="segment">Null or a specific segment to increase within the same 'Pie Chart'</param>
    /// <example>
    /// Increase cache metric hit and miss:
    /// <code>
    /// Metric.Inc("cache", "hit");
    /// Metric.Inc("cache", "miss");
    /// // Results in one metric "cache" with two segments, hit and miss, with a count of 1 in each
    /// 
    /// Metric.Inc("error");
    /// Metric.Inc("error");
    /// // Results in one metric "error" with a count of 2
    /// </code>
    /// </example>
    public static void Inc(string label, string segment = null)
    {
        if (segment == null)
        {
            LabelCounter.WithLabels(label).Inc();
        }
        else
        {
            LabelSegmentCounter.WithLabels(label, segment).Inc();
        }
    }
}
