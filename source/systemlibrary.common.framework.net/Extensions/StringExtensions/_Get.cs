using System.Text;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Returns the UTF8 byte representation of the string.
    /// </summary>
    public static byte[] GetBytes(this string text, Encoding encoding = default)
    {
        if (text == null) return null;

        if (encoding == null)
            return Encoding.UTF8.GetBytes(text);

        return encoding.GetBytes(text);
    }

    /// <summary>
    /// Returns the primary domain from a URL string, or blank if not resolvable.
    /// </summary>
    /// <example>
    /// <code>
    /// "https://www.sub.systemlibrary.com/path".GetPrimaryDomain(); // "systemlibrary.com"
    /// </code>
    /// </example>
    public static string GetPrimaryDomain(this string url)
    {
        if (url == null || url.Length == 0) return "";

        if (url.Contains(" ")) return "";

        Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

        return uri.GetPrimaryDomain();
    }
}
