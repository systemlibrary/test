namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Removes the first matching value from the end. Case-sensitive, not recursive.
    /// Does not trim spaces unless explicitly passed.
    /// </summary>
    /// <example>
    /// <code>
    /// "abcd".TrimEnd("d", "bc"); // "abc" — matches "d" and returns, "bc" never checked
    /// </code>
    /// </example>
    public static string TrimEnd(this string text, params string[] values)
    {
        if (text.IsNot()) return text;

        if (values == null || values.Length == 0) return text;

        int start = 0;
        int valueLength;
        bool found = false;

        var textSpan = text.AsSpan();

        for (int i = 0; i < values.Length; i++)
        {
            valueLength = values[i].Length;
            start = text.Length - valueLength;

            for (int j = 0; j < valueLength; j++)
            {
                if (textSpan[start + j] != values[i][j])
                    break;

                if (j == valueLength - 1)
                    found = true;
            }

            if (found)
                break;
        }

        if (found)
            return textSpan.Slice(0, start).ToString();

        return textSpan.ToString();
    }
}
