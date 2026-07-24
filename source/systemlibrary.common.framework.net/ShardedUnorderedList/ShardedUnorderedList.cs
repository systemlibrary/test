using System.Collections;

namespace SystemLibrary.Common.Framework;

// TODO: marked internal, as not part of v.1 of the framework
internal class ShardedUnorderedList<TValue> : IEnumerable<TValue>
{ 
    const int ShardCount = 4;
    const int ShardEnd = 3;

    // TODO: Consider HashSet as we have a 'Contains' method
    List<TValue>[] Shards;
    object[] ShardLocks;

    int MaxItemCount;

    public ShardedUnorderedList(int maxItemCount = 100000)
    {
        MaxItemCount = maxItemCount / ShardCount;

        if (MaxItemCount < 1) MaxItemCount = 1;

        Shards = new List<TValue>[ShardCount];
        ShardLocks = new object[ShardCount];

        for (int i = 0; i < ShardCount; i++)
        {
            Shards[i] = new List<TValue>();
            ShardLocks[i] = new object();
        }
    }

    int GetShard(TValue key)
    {
        int hash = key?.GetHashCode() ?? 0;

        return hash & ShardEnd;
    }

    public void Add(TValue value)
    {
        int shard = GetShard(value);

        lock (ShardLocks[shard])
        {
            var list = Shards[shard];

            if (list.Count > MaxItemCount)
                list.Clear();

            list.Add(value);
        }
    }

    public bool Remove(TValue value)
    {
        int shard = GetShard(value);
        lock (ShardLocks[shard])
        {
            return Shards[shard].Remove(value);
        }
    }

    public bool Contains(TValue value)
    {
        int shard = GetShard(value);
        lock (ShardLocks[shard])
        {
            return Shards[shard].Contains(value);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < ShardCount; i++)
        {
            lock (ShardLocks[i])
            {
                Shards[i].Clear();
            }
        }
    }

    public int Count
    {
        get
        {
            int total = 0;
            for (int i = 0; i < ShardCount; i++)
            {
                lock (ShardLocks[i])
                {
                    total += Shards[i].Count;
                }
            }
            return total;
        }
    }

    public IEnumerable<TValue> Items
    {
        get
        {
            List<TValue> snapshot = new List<TValue>();

            for (int i = 0; i < ShardCount; i++)
            {
                lock (ShardLocks[i])
                {
                    foreach (var value in Shards[i])
                        snapshot.Add(value);
                }
            }

            foreach (var value in snapshot)
                yield return value;
        }
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        List<TValue> snapshot = new List<TValue>();

        for (int i = 0; i < ShardCount; i++)
        {
            lock (ShardLocks[i])
            {
                foreach (var value in Shards[i])
                    snapshot.Add(value);
            }
        }

        foreach (var value in snapshot)
            yield return value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}