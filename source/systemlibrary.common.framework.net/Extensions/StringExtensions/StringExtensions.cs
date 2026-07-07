using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace SystemLibrary.Common.Framework;

public static partial class StringExtensions
{
    internal static ConcurrentDictionary<int, MemberInfo[]> EnumMembersCached;

    static StringExtensions()
    {
        EnumMembersCached = new ConcurrentDictionary<int, MemberInfo[]>();
    }

    /// <summary>
    /// Returns true if text ends with any of the values, case-sensitive.
    /// </summary>
    public static bool EndsWithAny(this string text, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.EndsWith(values[i], StringComparison.Ordinal))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text ends with any of the values using the specified <c>StringComparison</c>.
    /// </summary>
    public static bool EndsWithAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.EndsWith(values[i], comparison))
                return true;

        return false;
    }


    /// <summary>
    /// Returns true if text contains any of the values, case-sensitive.
    /// </summary>
    public static bool ContainsAny(this string text, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        return values.Any(x => l >= x.Length && text.Contains(x, StringComparison.Ordinal));
    }

    /// <summary>
    /// Returns true if text contains any of the values using the specified <c>StringComparison</c>.
    /// </summary>
    public static bool ContainsAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;

        return values.Any(x => l >= x.Length && text.Contains(x, comparison));
    }


    /// <summary>
    /// Replaces all occurrences of each old value with <c>newValue</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// "Hello world 123".ReplaceAllWith("A", "Hello", "world", "123"); // "A A A"
    /// </code>
    /// </example>
    public static string ReplaceAllWith(this string text, string newValue, params string[] oldValues)
    {
        if (text == null) return text;

        var l = text.Length;

        if (l == 0) return text;

        if (newValue == null) return text;

        if (oldValues == null) return text;

        var nl = newValue.Length;

        var ol = oldValues.Length;

        if (ol > 1)
        {
            if (ol > 3 || l > 128 || nl > 64)
            {
                var sb = new StringBuilder(text);

                foreach (var oldValue in oldValues)
                    sb.Replace(oldValue, newValue);

                return sb.ToString();
            }
            else
            {
                foreach (var oldValue in oldValues)
                    text = text.Replace(oldValue, newValue);

                return text;
            }
        }

        return text.Replace(oldValues[0], newValue);
    }

    /// <summary>
    /// Returns the string truncated to <c>maxLength</c> characters. Optionally appends ellipsis.
    /// </summary>
    /// <example>
    /// <code>
    /// "hello world".MaxLength(5);              // "hello"
    /// "hello world".MaxLength(8, true);        // "hello..."
    /// </code>
    /// </example>
    public static string MaxLength(this string text, int maxLength, bool useEllipsis = false)
    {
        if (text == null) return "";

        if (text.Length <= maxLength) return text;

        if (useEllipsis && maxLength > 3)
        {
            if (maxLength > 1024)
            {
                var sb = new StringBuilder(maxLength);

                sb.Append(text.AsSpan(0, maxLength - 3))
                  .Append("...");

                return sb.ToString();
            }
            return new string(text.AsSpan(0, maxLength - 3)) + "...";
        }

        if (maxLength <= 0) return "";

        return new string(text.AsSpan(0, maxLength));
    }
}
