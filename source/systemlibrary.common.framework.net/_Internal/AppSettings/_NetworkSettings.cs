
namespace SystemLibrary.Common.Framework.Bootstrap;

internal class NetworkSettings
{
    /// <summary>
    /// Comma-separated CIDR ranges of trusted proxy networks allowed to forward headers.
    /// Use "*" to trust all networks, e.g. behind Azure Front Door or similar reverse proxies.
    /// </summary>
    /// <remarks>
    /// Empty in Development skips validation. Empty in any other environment throws on startup — set explicitly or use "*".
    /// </remarks>
    public string ForwardedIPNetworks { get; set; } = "";
}
