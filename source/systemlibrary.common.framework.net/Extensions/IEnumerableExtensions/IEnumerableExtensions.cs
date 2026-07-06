using System.Collections;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for IEnumerable.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Returns distinct elements by a selected key property.
    /// </summary>
    /// <example>
    /// <code>
    /// var list = new List&lt;Car&gt; { new Car { Name = "Vehicle" }, new Car { Name = "Vehicle" } };
    /// var distinct = list.DistinctBy(x => x.Name).ToList();
    /// // 1 car
    /// </code>
    /// </example>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        where T : class
    {
        return items.GroupBy(selector).Select(grp => grp.FirstOrDefault());
    }

    /// <summary>
    /// Flattens a nested enumerable into a single array, skipping null collections and null elements.
    /// </summary>
    public static T[] Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        if (source == null) return Array.Empty<T>();

        return source
            .Where(x => x != null)
            .SelectMany(x => x.Where(y => y != null))
            .ToArray();
    }

    /// <summary>
    /// Returns true if the enumerable contains the value. Returns false on null input.
    /// </summary>
    /// <example>
    /// <code>
    /// new[] { "Hello", "World" }.Has("Abc"); // false
    /// </code>
    /// </example>
    public static bool Has<T>(this IEnumerable<T> enumerable, T value) where T : IComparable, IConvertible
    {
        if (enumerable == null) return false;

        if (value == null) return false;

        return enumerable.Contains(value);
    }

    /// <summary>
    /// Returns true if the enumerable contains the object. Returns false on null input or type mismatch.
    /// </summary>
    /// <example>
    /// <code>
    /// var user = new User();
    /// new List&lt;User&gt; { user }.Has(user); // true
    /// </code>
    /// </example>
    public static bool Has<T>(this IEnumerable<T> enumerable, object value) where T : class
    {
        if (enumerable == null) return false;

        if (value == null) return false;

        if (value is T t)
            return enumerable.Contains(t);

        return false;
    }

    /// <summary>
    /// Returns true if the enumerable is null, empty, or contains only a single null element.
    /// </summary>
    /// <example>
    /// <code>
    /// new List&lt;string&gt;().IsNot();  // true
    /// ((List&lt;string&gt;)null).IsNot(); // true
    /// </code>
    /// </example>
    public static bool IsNot<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return true;

        if (enumerable is ICollection iCollection)
        {
            if (iCollection.Count == 0) return true;

            if (iCollection.Count == 1 && iCollection is IList list)
                return list[0] == null;

            return false;
        }

        return !enumerable.Any();
    }

    /// <summary>
    /// Returns true if the enumerable is non-null and contains at least one element.
    /// </summary>
    /// <example>
    /// <code>
    /// new List&lt;string&gt; { "hello" }.Is(); // true
    /// ((List&lt;string&gt;)null).Is();         // false
    /// </code>
    /// </example>
    public static bool Is<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return false;

        if (enumerable is ICollection iCollection)
            return iCollection.Count > 0;

        return enumerable.Any();
    }
}