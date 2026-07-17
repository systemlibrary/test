using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for StringBuilder.
/// </summary>
public static partial class StringBuilderExtensions
{
    /// <summary>
    /// Returns true if non-null and has content.
    /// </summary>
    public static bool Is(this StringBuilder stringBuilder)
    {
        return stringBuilder != null && stringBuilder.Length != 0;
    }

    /// <summary>
    /// Returns true if null or empty.
    /// </summary>
    public static bool IsNot(this StringBuilder stringBuilder)
    {
        return stringBuilder == null || stringBuilder.Length == 0;
    }

    /// <summary>
    /// Returns true if the StringBuilder ends with the specified string.
    /// </summary>
    public static bool EndsWith(this StringBuilder stringBuilder, string ending, bool caseInsensitive = false)
    {
        if (stringBuilder == null || stringBuilder.Length == 0) return false;

        if (ending == null || ending == "") return false;

        var endingLength = ending.Length;

        if (endingLength > stringBuilder.Length) return false;

        var startIndex = stringBuilder.Length - endingLength;
        var endIndex = startIndex + endingLength;
        var j = 0;

        if (!caseInsensitive)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (stringBuilder[i] != ending[j]) return false;
                j++;
            }
        }
        else
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (char.ToLowerInvariant(stringBuilder[i]) != char.ToLowerInvariant(ending[j])) return false;
                j++;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns true if the StringBuilder begins with the specified string.
    /// </summary>
    public static bool BeginsWith(this StringBuilder stringBuilder, string start, bool caseInsensitive = false)
    {
        if (stringBuilder == null || stringBuilder.Length == 0) return false;

        if (start == null || start == "") return false;

        var startLength = start.Length;

        if (startLength > stringBuilder.Length) return false;

        var startIndex = 0;
        var endIndex = startIndex + startLength;
        var j = 0;

        if (!caseInsensitive)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (stringBuilder[i] != start[j]) return false;
                j++;
            }
        }
        else
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (char.ToLowerInvariant(stringBuilder[i]) != char.ToLowerInvariant(start[j])) return false;
                j++;
            }
        }
        return true;
    }

    /// <summary>
    /// Removes the first matching value from the start. Defaults to trimming space and newlines if no values specified.
    /// Returns true if anything was removed.
    /// </summary>
    public static bool TrimStart(this StringBuilder stringBuilder, params string[] values)
    {
        if (values == null || values.Length == 0)
        {
            values = new string[] { " ", "\r\n", "\n", };
        }

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (stringBuilder.BeginsWith(value, false))
            {
                var length = value.Length;

                stringBuilder.Remove(0, length);

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes the first matching value from the end. Defaults to trimming space and newlines if no values specified.
    /// Returns true if anything was removed.
    /// </summary>
    public static bool TrimEnd(this StringBuilder stringBuilder, params string[] values)
    {
        if (values == null || values.Length == 0)
        {
            values = new string[] { " ", "\r\n", "\n", };
        }

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (stringBuilder.EndsWith(value, false))
            {
                var length = value.Length;

                stringBuilder.Remove(stringBuilder.Length - length, length);

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the index of the first occurrence of <c>text</c>, or -1 if not found.
    /// </summary>
    public static int IndexOf(this StringBuilder stringBuilder, string text, bool ignoreCase = false, int start = 0)
    {
        //Creds: https://stackoverflow.com/questions/1359948/why-doesnt-stringbuilder-have-indexof-method
        if (stringBuilder == null) return -1;

        if (text == null) return -1;

        int index;
        int length = text.Length;

        if (length > stringBuilder.Length) return -1;

        int maxSearchLength = (stringBuilder.Length - length) + 1;

        if (ignoreCase)
        {
            var textLowered = text.ToLower();
            for (int i = start; i < maxSearchLength; ++i)
            {
                if (Char.ToLower(stringBuilder[i]) == textLowered[0])
                {
                    index = 1;
                    while ((index < length) && (Char.ToLower(stringBuilder[i + index]) == textLowered[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        for (int i = start; i < maxSearchLength; ++i)
        {
            if (stringBuilder[i] == text[0])
            {
                index = 1;
                while ((index < length) && (stringBuilder[i + index] == text[index]))
                    ++index;

                if (index == length)
                    return i;
            }
        }

        return -1;
    }

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
