namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Returns true if text starts with any of the values, case-sensitive.
    /// </summary>
    public static bool StartsWithAny(this string text, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.StartsWith(values[i], StringComparison.Ordinal))
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text starts with any of the values using the specified <c>StringComparison</c>.
    /// </summary>
    public static bool StartsWithAny(this string text, StringComparison comparison, params string[] values)
    {
        if (text == null || text.Length == 0) return false;

        if (values == null || values.Length == 0) return false;

        var l = text.Length;
        for (int i = 0; i < values.Length; i++)
            if (values[i] != null && l >= values[i].Length && text.StartsWith(values[i], comparison))
                return true;

        return false;
    }
}
