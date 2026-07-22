namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for Uri.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Returns the primary domain of the URI, stripping subdomains. Returns blank, never null.
    /// </summary>
    /// <example>
    /// <code>
    /// new Uri("https://www.sub.systemlibrary.com").GetPrimaryDomain(); // "systemlibrary.com"
    /// new Uri("https://systemlibrary.github.io/path").GetPrimaryDomain(); // "github.io"
    /// </code>
    /// </example>
    public static string GetPrimaryDomain(this Uri uri, string topLevelDomain = ".com")
    {
        if (uri == null)
            return "";

        string host;

        if (uri.IsAbsoluteUri)
            host = uri.Host;
        else
            host = uri.OriginalString;

        if (host.IsNot())
            return "";

        if (!host.Contains("."))
        {
            if (host == "localhost")
                return "localhost";

            return host + topLevelDomain;
        }

        var values = host.Split('.');

        var length = values.Length;

        if (length == 1)
            return host;

        if (length == 2)
        {
            if (values[1].Length <= 4)
                return host;
        }

        if (values[length - 1].Length > 4)
            return values[length - 1] + topLevelDomain;

        return values[length - 2] + "." + values[length - 1];
    }
}
