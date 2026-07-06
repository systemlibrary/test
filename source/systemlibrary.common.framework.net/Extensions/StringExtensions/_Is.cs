namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Returns true if text equals any of the values, case-sensitive.
    /// </summary>
    public static bool IsAny(this string text, params string[] values)
    {
        if (text.IsNot()) return false;

        if (values == null || values.Length == 0) return false;

        for (int i = 0; i < values.Length; i++)
            if (text == values[i])
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if text is null, empty or a single space. Does not check multiple spaces, tabs or newlines.
    /// </summary>
    /// <remarks>
    /// <c>"  "</c> (two spaces) returns false — only a single space is treated as empty.
    /// </remarks>
    public static bool IsNot(this string text, params string[] additionalNotValues)
    {
        if (text == null || text.Length == 0) return true;

        if (text.Length == 1 && text == " ") return true;

        if (additionalNotValues == null) return false;

        if (additionalNotValues.Any(value => text == value))
            return true;

        return false;
    }

    /// <summary>
    /// Returns true if text is non-null, non-empty and not a single space.
    /// </summary>
    public static bool Is(this string text)
    {
        if (text == null || text.Length == 0) return false;

        return !(text.Length == 1 && text == " ");
    }

    /// <summary>
    /// Returns true if text is non-null, non-empty, not a single space, and not equal to any of <c>invalidTexts</c>.
    /// </summary>
    public static bool Is(this string text, params string[] invalidTexts)
    {
        if (text.IsNot())
            return false;

        if (invalidTexts == null || invalidTexts.Length == 0)
            return true;

        foreach (var word in invalidTexts)
            if (text == word)
                return false;

        return true;
    }
}
