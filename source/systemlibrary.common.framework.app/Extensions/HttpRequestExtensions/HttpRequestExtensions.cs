using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// HttpRequest extension helpers.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Returns the full URL including scheme, host, path and query string.
    /// </summary>
    /// <remarks>
    /// Requires UseForwardedHeaders to be configured first to reflect the actual client URL.
    /// </remarks>
    /// <example>
    /// <code>
    /// var url = request.Url();
    /// // https://www.systemlibrary.com/hello?world=1
    /// </code>
    /// </example>
    public static string Url(this HttpRequest request)
    {
        if (request == null) return null;

        return request.Scheme + "://" + request.Host + request.Path + request.QueryString.Value;
    }

    /// <summary>
    /// Returns the path segment of the request URL, or null if not present.
    /// </summary>
    public static string Path(this HttpRequest request)
    {
        if (request?.Path.Value == null) return null;

        return request.Path.Value;
    }

    /// <summary>
    /// Returns the query string without the leading <c>?</c>, or null if not present.
    /// </summary>
    public static string Query(this HttpRequest request)
    {
        var q = request?.QueryString.Value;
        
        if (q == null) return null;

        if (q.Length <= 1) return "";

        return q[0] == '?' ? q.Substring(1) : q;
    }

    /// <summary>
    /// Returns query string as a key/value dictionary. Multiple values for the same key are comma-separated.
    /// Returns null if request or query is null, empty dictionary if no pairs exist.
    /// </summary>
    public static Dictionary<string, string> QueryKeyValues(this HttpRequest request)
    {
        var query = request?.Query;
        if (query == null) return null;
        if (query.Count == 0) return new Dictionary<string, string>(0);

        var dict = new Dictionary<string, string>(query.Count);
        foreach (var kv in query)
        {
            dict[kv.Key] = kv.Value.ToString();
        }

        return dict;
    }

    /// <summary>
    /// Returns cookies as a key/value dictionary.
    /// Returns null if request or cookies are null, empty dictionary if no cookies exist.
    /// </summary>
    public static Dictionary<string, string> CookieKeyValues(this HttpRequest request)
    {
        var cookies = request?.Cookies;

        if (cookies == null) return null;

        if (cookies.Count == 0) return new Dictionary<string, string>(0);

        var dict = new Dictionary<string, string>(cookies.Count);
        foreach (var kv in cookies)
        {
            dict[kv.Key] = kv.Value;
        }

        return dict;
    }

    /// <summary>
    /// Returns the value of a specific cookie, or null if not found.
    /// </summary>
    public static string Cookie(this HttpRequest request, string name)
    {
        var cookies = request?.Cookies;
        if (cookies == null) return null;

        if (cookies.TryGetValue(name, out var value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Returns true if the request path contains a file extension.
    /// </summary>
    public static bool IsFileRequest(this HttpRequest request)
    {
        if (request == null || !request.Path.HasValue) return false;

        var path = request.Path.Value;
        if (path == null || path.Length <= 3) return false;

        var extension = System.IO.Path.GetExtension(path);
        return extension.Length > 1;
    }

    /// <summary>
    /// Returns true if <c>X-Requested-With</c> header equals <c>XMLHttpRequest</c>.
    /// </summary>
    /// <remarks>
    /// Throws ArgumentNullException if request is null — unlike other extensions in this class which return null.
    /// </remarks>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        if (request.Headers == null) return false;

        if (request.Headers.ContainsKey("X-Requested-With") &&
            request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the Referer header as a Uri, or null if not present.
    /// </summary>
    public static Uri Referer(this HttpRequest request)
    {
        var header = request?.GetTypedHeaders();
        return header?.Referer;
    }

    /// <summary>
    /// Returns the Accept header value, or null if not present.
    /// </summary>
    public static string Accept(this HttpRequest request)
    {
        if (request?.Headers.TryGetValue("Accept", out var value) == true)
        {
            return value.ToString();
        }

        return null;
    }

    /// <summary>
    /// Returns the User-Agent header value, or null if not present.
    /// </summary>
    public static string UserAgent(this HttpRequest request)
    {
        if (request?.Headers.TryGetValue("User-Agent", out var value) == true)
        {
            return value.ToString();
        }

        return null;
    }

    /// <summary>
    /// Returns the Content-Type header value, or null if not present.
    /// </summary>
    public static string? ContentType(this HttpRequest request)
    {
        if (request?.Headers.TryGetValue("Content-Type", out var value) == true)
        {
            return value.ToString();
        }

        return null;
    }
}

