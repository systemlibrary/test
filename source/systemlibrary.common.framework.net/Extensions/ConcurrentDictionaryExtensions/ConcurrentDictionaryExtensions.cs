using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for ConcurrentDictionary.
/// </summary>
public static class ConcurrentDictionaryExtensions
{
    /// <summary>
    /// Returns a cached value by int key, or invokes <c>getItem</c> and caches the result.
    /// Clears the dictionary and starts over when it exceeds 50,000 entries.
    /// </summary>
    /// <example>
    /// <code>
    /// ConcurrentDictionary&lt;int, string&gt; cache = new();
    /// var result = cache.Cache(123, () => "Hello world");
    /// </code>
    /// </example>
    public static T Cache<T>(this ConcurrentDictionary<int, T> dictionary, int key, Func<T> getItem)
    {
        if (dictionary == null)
        {
            return getItem();
        }

        if (!dictionary.TryGetValue(key, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(key, result);
        }

        return result;
    }

    /// <summary>
    /// Returns a cached value by Type key using its hash code, or invokes <c>getItem</c> and caches the result.
    /// Clears the dictionary and starts over when it exceeds 50,000 entries.
    /// </summary>
    /// <example>
    /// <code>
    /// ConcurrentDictionary&lt;int, string&gt; cache = new();
    /// var result = cache.Cache(typeof(Car), () => "Hello world");
    /// </code>
    /// </example>
    public static T Cache<T>(this ConcurrentDictionary<int, T> dictionary, Type type, Func<T> getItem)
    {
        if (dictionary == null)
        {
            return getItem();
        }

        var hashCode = type.GetHashCode();

        if (!dictionary.TryGetValue(hashCode, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(hashCode, result);
        }

        return result;
    }

    /// <summary>
    /// Returns a cached value by string key, or invokes <c>getItem</c> and caches the result.
    /// Clears the dictionary and starts over when it exceeds 50,000 entries.
    /// </summary>
    /// <example>
    /// <code>
    /// ConcurrentDictionary&lt;string, string&gt; cache = new();
    /// var result = cache.Cache("myKey", () => "Hello world");
    /// </code>
    /// </example>
    public static T Cache<T>(this ConcurrentDictionary<string, T> dictionary, string key, Func<T> getItem)
    {
        if (dictionary == null)
            return getItem();

        if (key == null)
            return getItem();

        if (!dictionary.TryGetValue(key, out var result))
        {
            if (dictionary.Count > 50000)
            {
                dictionary.Clear();
            }
            result = getItem();

            dictionary.TryAdd(key, result);
        }

        return result;
    }
}
