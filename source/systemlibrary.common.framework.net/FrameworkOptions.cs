namespace SystemLibrary.Common.Framework;

/// <summary>
/// Configuration options for framework services and middleware pipeline.
/// </summary>
/// <example>
/// <code>
/// var options = new FrameworkOptions
/// {
///     UseHttpsRedirection = true,
///     ViewLocations = new[] { "~/Folder/{0}/Index.cshtml" }
/// };
///
/// services.AddFrameworkServices(AppType.Web);
/// </code>
/// </example>
public partial class FrameworkOptions
{
    /// <summary>
    /// Registers authentication services and middleware. Default: true.
    /// </summary>
    public bool UseAuthentication = true;

    /// <summary>
    /// Registers authorization services and middleware. Default: true.
    /// </summary>
    public bool UseAuthorization = true;

    /// <summary>
    /// Registers forwarded headers middleware for proxy/load balancer support. Default: true.
    /// </summary>
    public bool UseForwardedHeaders = true;

    /// <summary>
    /// Registers HTTP to HTTPS redirection middleware. Default: false.
    /// </summary>
    public bool UseHttpsRedirection = false;

    /// <summary>
    /// Registers HSTS middleware — adds <c>Strict-Transport-Security</c> header for client-side redirection. Default: false.
    /// </summary>
    public bool UseHsts = false;

    /// <summary>
    /// Registers output cache services and middleware. Default: true.
    /// </summary>
    public bool UseOutputCache = true;

    /// <summary>
    /// Registers Gzip response compression services and middleware. Default: true.
    /// </summary>
    public bool UseGzipResponseCompression = true;

    /// <summary>
    /// Registers Brotli response compression services and middleware. Default: false.
    /// </summary>
    public bool UseBrotliResponseCompression = false;
}
