
namespace SystemLibrary.Common.Framework.Bootstrap;

internal class MetricsSettings
{
    public bool Enable { get; set; } = true;
    public string Url { get; set; }
    public string SecretKeyForUI { get; set; } = "Demo";
}
