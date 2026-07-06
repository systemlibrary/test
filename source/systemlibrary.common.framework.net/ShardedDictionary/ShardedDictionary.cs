using System.Collections.Concurrent;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// A thread-safe dictionary sharded across 4 internal concurrent dictionaries to reduce lock contention.
/// Automatically clears a shard when it exceeds its item limit.
/// </summary>
public class ShardedDictionary<TKey, TValue>
{
    ConcurrentDictionary<TKey, TValue>[] Shards;

    const int ShardCount = 4;
    const int ShardEnd = 3;

    int MaxItemCount;

    static object ClearLock = new object();

    /// <summary>
    /// Creates a new sharded dictionary. Each shard clears itself when its share of <c>maxItemCount</c> is reached.
    /// </summary>
    public ShardedDictionary(int maxItemCount = 100000)
    {
        MaxItemCount = maxItemCount / ShardCount;

        Shards = new ConcurrentDictionary<TKey, TValue>[ShardCount];
        for (int i = 0; i < ShardCount; i++)
            Shards[i] = new ConcurrentDictionary<TKey, TValue>();
    }

    ConcurrentDictionary<TKey, TValue> GetShard(TKey key)
    {
        int hash = key?.GetHashCode() ?? 0;

        int index = hash & ShardEnd;

        return Shards[index];
    }

    /// <summary>
    /// Attempts to add the key/value pair. Clears the shard first if it has reached its item limit.
    /// </summary>
    public bool TryAdd(TKey key, TValue value)
    {
        var shard = GetShard(key);

        if (shard.Count > MaxItemCount)
        {
            lock(ClearLock)
            {
                if (shard.Count > MaxItemCount)
                {
                    shard.Clear();
                    return shard.TryAdd(key, value);
                }
            }
        }

        return shard.TryAdd(key, value);
    }

    /// <summary>
    /// Adds the key/value pair. Silently discards if key already exists.
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        TryAdd(key, value);
    }

    /// <summary>
    /// Returns the value for the key, or default if not found.
    /// </summary>
    public TValue GetValue(TKey key)
    {
        return GetShard(key).GetValueOrDefault(key);
    }

    /// <summary>
    /// Returns true if the key was found, setting <c>value</c>. Returns false if not found.
    /// </summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return GetShard(key).TryGetValue(key, out value);
    }

    /// <summary>
    /// Returns true if the key exists.
    /// </summary>
    public bool ContainsKey(TKey key) => GetShard(key).ContainsKey(key);

    /// <summary>
    /// Returns true if the key was found and removed.
    /// </summary>
    public bool TryRemove(TKey key, out TValue value) => GetShard(key).TryRemove(key, out value);

    /// <summary>
    /// Removes the key and returns its value, or default if not found.
    /// </summary>
    public TValue Remove(TKey key)
    {
        TryRemove(key, out var value);
        return value;
    }

    /// <summary>
    /// Returns true if key was found and removed, else false
    /// </summary>
    public bool TryRemove(TKey key) => GetShard(key).TryRemove(key, out TValue _);

    /// <summary>
    /// Clears all shards.
    /// </summary>
    public void Clear()
    {
        foreach (var shard in Shards)
            shard.Clear();
    }

    /// <summary>
    /// Updates the value for <c>key</c> only if the current value equals <c>comparisonValue</c>.
    /// </summary>
    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
        var shard = GetShard(key);

        return shard.TryUpdate(key, newValue, comparisonValue);
    }

    /// <summary>
    /// Gets or sets the value for the key.
    /// </summary>
    public TValue this[TKey key]
    {
        get
        {
            return GetValue(key);
        }
        set
        {
            var shard = GetShard(key);

            shard[key] = value;
        }
    }

    /// <summary>
    /// Returns the total item count across all shards.
    /// </summary>
    public int Count
    {
        get
        {
            var count = 0;
            for (int i = 0; i < ShardCount; i++)
            {
                count += Shards[i].Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Enumerates all keys across all shards.
    /// </summary>
    public IEnumerable<TKey> Keys
    {
        get
        {
            for (int i = 0; i < ShardCount; i++)
            {
                foreach (var key in Shards[i].Keys)
                    yield return key;
            }
        }
    }

    /// <summary>
    /// Returns the cached value for the key, or invokes <c>getItem</c>, stores and returns the result.
    /// Clears the shard if it has reached its item limit before storing.
    /// </summary>
    /// <example>
    /// <code>
    /// var dict = new ShardedDictionary&lt;string, string&gt;();
    /// var result = dict.Cache("myKey", () => "Hello world");
    /// </code>
    /// </example>
    public TValue Cache(TKey key, Func<TValue> getItem)
    {
        if (key == null)
            return getItem();

        var shard = GetShard(key);

        if (!shard.TryGetValue(key, out var result))
        {
            if (shard.Count > MaxItemCount)
            {
                shard.Clear();
            }
            result = getItem();

            shard[key] = result;
        }

        return result;
    }
}