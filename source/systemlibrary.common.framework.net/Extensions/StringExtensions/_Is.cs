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

    /// <summary>
    /// Checks if input is a file path, either relative or absolute, either web or operative system path
    /// </summary>
    /// <remarks>
    /// Does not throw
    /// <para>Returns true if input is longer than 3 and contains a 'file extension' of 1 to 6 characters, else false</para>
    /// <para>Returns true if input contains /public/, /static/, /images/ or 'assets/' as we assume a file is asked for</para>
    /// <para>Supports input as relative, absolute, and with url query params</para>
    /// <para>Returns false if input exceeds 4096 characters</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var hello = "world";
    /// var isFile = hello.IsFile(); // false
    /// 
    /// var file = "/image/redcar1.jpg?qualit=80";
    /// var isFile = file.IsFile(); // true
    /// 
    /// var file2 = "/assets/bluecar";
    /// var isFile = file2.IsFile(); // true, assumes any "assets/" request is a file
    /// </code>
    /// </example>
    /// <returns>True or false</returns>
    public static bool IsFile(this string path)
    {
        if (path == null || path.Length <= 3 || path.Length > 4096) return false;

        if (path.EndsWith("/")) return false;

        if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1) return false;

        if (path.Contains("<")) return false;

        bool HasAssetPath() =>
            path.Contains("/public/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/images/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/static/", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("assets/", StringComparison.OrdinalIgnoreCase);

        var extensionIndex = path.LastIndexOf('.');

        if (extensionIndex == -1) return HasAssetPath();

        var queryIndex = path.IndexOf('?');

        if (queryIndex == -1)
        {
            if (extensionIndex == path.Length - 1) return HasAssetPath();

            if (path.LastIndexOf('/') > extensionIndex) return HasAssetPath();

            return extensionIndex >= path.Length - 7 || HasAssetPath(); // .config
        }

        if (extensionIndex > queryIndex)
        {
            var temp = path.Substring(0, queryIndex);

            return temp.LastIndexOf('.') >= temp.Length - 7 || HasAssetPath(); // .config
        }

        if (path[queryIndex - 1] == '/') return false;

        return queryIndex - 7 <= extensionIndex || HasAssetPath();
    }
}
