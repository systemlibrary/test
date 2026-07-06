namespace SystemLibrary.Common.Framework.Boostrap;

internal class _AppSettings
{
    public string Name { get; set; } = "company_unique_appname";
    public bool Debug { get; set; } = false;
    public bool Diagnostics { get; set; } = false;
    public string License { get; set; } = "";

    /// <summary>
    /// Comma-separated CIDR ranges of trusted proxy networks allowed to forward headers.
    /// Use "*" to trust all networks, e.g. behind Azure Front Door or similar reverse proxies.
    /// </summary>
    /// <remarks>
    /// Empty in Development skips validation. Empty in any other environment throws on startup — set explicitly or use "*".
    /// </remarks>
    public string ForwardedIPNetworks { get; set; } = "";
}
