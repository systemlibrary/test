namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Concatenates one or more arrays into a new array.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = new[] { 2 }.Add(new[] { 1 });
    /// // { 2, 1 }
    /// </code>
    /// </example>
    public static T[] Add<T>(this T[] current, params T[][] additional)
    {
        return Add(current, null, additional);
    }

    public static T[] Add<T>(this T[] current, T item)
    {
        return Add(current, new T[] { item });
    }

    /// <summary>
    /// Concatenates one or more arrays into a new array, filtered by a predicate.
    /// </summary>
    /// <example>
    /// <code>
    /// var result = new[] { 3, 4 }.Add(i => i > 1, new[] { 1, 2, 3 });
    /// // { 3, 4, 2, 3 } — only values matching predicate are included from all arrays
    /// </code>
    /// </example>
    public static T[] Add<T>(this T[] current, Func<T, bool> predicate, params T[][] additional)
    {
        if (additional == null ||
            additional.Length == 0 ||
            (additional.Length == 1 && additional[0] == null)) return current;

        var sum = current?.Length ?? 0;

        foreach (var add in additional)
            sum += add?.Length ?? 0;

        var result = new T[sum];

        var i = 0;

        if (current != null)
        {
            foreach (var v in current)
            {
                if (predicate == null || predicate(v))
                {
                    result[i] = v;
                    i++;
                }
            }
        }

        foreach (var add in additional)
        {
            if (add != null)
            {
                foreach (var v in add)
                {
                    if (predicate == null || predicate(v))
                    {
                        result[i] = v;
                        i++;
                    }
                }
            }
        }

        if (i != sum)
        {
            var trimmed = new T[i];

            Array.Copy(result, 0, trimmed, 0, i);

            return trimmed;
        }

        return result;
    }
}
