using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for StringBuilder.
/// </summary>
public static partial class StringBuilderExtensions
{
    static Dictionary<string, string> HtmlEncodeReplacements = new Dictionary<string, string>
    {
        { "\"", "&quot;" },
        { "'", "&apos;" },
    };

    /// <summary>
    /// Replaces <c>"</c> with <c>&amp;quot;</c> and <c>'</c> with <c>&amp;apos;</c>. Throws on null input.
    /// </summary>
    public static StringBuilder HtmlEncodeQuotes(this StringBuilder html, Dictionary<string, string> additionalReplacements = null)
    {
        foreach (var replacement in HtmlEncodeReplacements)
            html.Replace(replacement.Key, replacement.Value);

        if (additionalReplacements != null)
        {
            foreach (var replacement in additionalReplacements)
                html.Replace(replacement.Key, replacement.Value);
        }

        return html;
    }

    static Dictionary<string, string> HtmlDecodeReplacements = new Dictionary<string, string>
    {
        { "&quot;", "\"" },
        { "&apos;", "'" }
    };

    /// <summary>
    /// Replaces <c>&amp;quot;</c> with <c>"</c> and <c>&amp;apos;</c> with <c>'</c>. Throws on null input.
    /// </summary>
    public static StringBuilder HtmlDecodeQuotes(this StringBuilder html, Dictionary<string, string> additionalReplacements = null)
    {
        foreach (var replacement in HtmlDecodeReplacements)
            html.Replace(replacement.Key, replacement.Value);

        if (additionalReplacements != null)
        {
            foreach (var replacement in additionalReplacements)
                html.Replace(replacement.Key, replacement.Value);
        }
        return html;
    }

    /// <summary>
    /// Truncates the StringBuilder to <c>maxLength</c> characters. No-op if already within limit.
    /// </summary>
    /// <example>
    /// <code>
    /// var sb = new StringBuilder("hello world");
    /// sb.MaxLength(1).ToString(); // "h"
    /// </code>
    /// </example>
    public static StringBuilder MaxLength(this StringBuilder stringBuilder, int maxLength)
    {
        if (stringBuilder == null || stringBuilder.Length <= maxLength) return stringBuilder;

        stringBuilder.Length = maxLength;

        return stringBuilder;
    }

    internal static void AppendAnsiColor(this StringBuilder sb, string color)
    {
        if (sb.Length == 0) return;

        var end = sb.Length;

        for (var i = 0; i < end; i++)
        {
            var c = sb[i];

            if (c == ':' || c == '\n')
            {
                end = i;
                break;
            }
        }

        sb.Insert(0, color);

        sb.Insert(end + color.Length, AnsiColor.End);
    }
}
