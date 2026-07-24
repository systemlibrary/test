namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Styling and ordering options for a single slice in a metric pie chart.
/// </summary>
public class SliceOption
{
    /// <summary>
    /// Matches the segment text used in <c>Metric.Inc("label", "segment")</c>. Set to null to apply as default for all slices.
    /// </summary>
    public string Segment;

    /// <summary>
    /// Background color of the slice. Accepts #hex, rgb(0,0,0) or a named browser color.
    /// </summary>
    public string Color = null;

    /// <summary>
    /// Render order of the slice, clockwise from 0. Defaults to 9999999 which sorts by count descending.
    /// </summary>
    public int Order = 9999999;
}
