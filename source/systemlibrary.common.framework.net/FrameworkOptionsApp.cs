namespace SystemLibrary.Common.Framework;

partial class FrameworkOptions
{
    /// <summary>
    /// Registers the developer exception page middleware — shows detailed error information. Default: true.
    /// </summary>
    public bool UseDeveloperPage = true;

    /// <summary>
    /// Registers middleware that logs status code, path and HTTP method for all responses with status >= 300. Default: true.
    /// </summary>>
    public bool UseStatusCodeLogger = true;

    /// <summary>
    /// Registers static file middleware serving from <c>wwwroot</c> and auto-discovered folders: <c>public</c>, <c>static</c>, <c>dist</c>, <c>frontend</c>, <c>assets</c>, <c>files</c>.
    /// Only GET and HEAD requests are served. Default: true.
    /// </summary>
    /// <remarks>
    /// Serves unknown file types, enables HTTPS compression and appends <c>Cache-Control</c> if not already set.
    /// </remarks>
    public bool UseStaticFilePolicy = true;

    /// <summary>
    /// Adds security middleware.
    /// <para>Prod-only: Adds 'nosniff' for .js and .mjs in the static branch.</para>
    /// <para>Prod-only: Adds Referrer-Policy 'strict-origin-when-cross-origin' on non-static and non-API requests.</para>
    /// <para>Prod-only: Adds Content-Security-Policy allowing all, but only allowing iframing for primary domain + subdomains on non-static and non-API requests.</para>
    /// <para>Non-prod environments do not add security headers, allowing all content and resources to load freely.</para>
    /// </summary>
    public bool UseSecurityPolicy = true;

    /// <summary>
    /// Cache-Control max-age in seconds for static file responses. Default: 604800 (7 days).
    /// </summary>
    /// <remarks>
    /// Requires <c>UseStaticFilePolicy</c> to be true. Has no effect if <c>max-age</c> is already set in the response.
    /// </remarks>
    public int StaticFilesClientCacheSeconds = 604800;

    /// <summary>
    /// Relative paths from which static content is served — e.g. <c>new[] { "/static", "/public" }</c>.
    /// </summary>
    /// <remarks>
    /// Requires <c>UseStaticFilePolicy</c> to be true.
    /// </remarks>
    /// <example>
    /// StaticRequestPaths = new string[] { "/static", "/public" }
    /// </example>
    public string[] StaticRequestPaths = null;
}
