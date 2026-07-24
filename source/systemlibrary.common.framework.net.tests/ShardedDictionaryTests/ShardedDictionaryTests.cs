using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.Tests;

[TestClass]
public class ShardedDictionaryTests : BaseTest
{
    [TestMethod]
    public void Add__Adds_Data()
    {
        var dict = new ShardedDictionary<string, string>();

        dict.Add("hello", "world");

        var value = dict.GetValue("hello");

        IsOk(value);
        IsOk(value, "world");
    }

    [TestMethod]
    public void TryAdd__Adds_Data()
    {
        var dict = new ShardedDictionary<string, string>();

        dict.TryAdd("hello", "world");

        var value = dict.GetValue("hello");

        IsOk(value);
        IsOk(value, "world");
    }

    [TestMethod]
    public void Remove__Gets_Null()
    {
        var dict = new ShardedDictionary<string, string>();

        dict.Add("Remove__Gets_Null", "v");

        dict.Remove("Remove__Gets_Null");

        var value = dict.GetValue("Remove__Gets_Null");

        IsNull(value);
    }

    [TestMethod]
    public void TryRemove__Gets_Null()
    {
        var dict = new ShardedDictionary<string, string>();

        dict.Add("k", "v");

        dict.TryRemove("k");

        var value = dict.GetValue("k");

        IsNull(value);
    }

    [TestMethod]
    public void Add_Remove_Add__Returns_SecondValue()
    {
        var dict = new ShardedDictionary<string, string>();

        dict.TryAdd("k", "v1");
        dict.TryRemove("k");
        dict.TryAdd("k", "v2");

        var value = dict.GetValue("k");

        IsOk(value);
        IsOk(value, "v2");
    }

    [TestMethod]
    public void Cache__Computes_WhenMissing()
    {
        var dict = new ShardedDictionary<string, string>();

        var value = dict.Cache("k", () => "v");

        IsOk(value);
        IsOk(value, "v");
    }

    [TestMethod]
    public void Cache__Is_Idempotent()
    {
        var dict = new ShardedDictionary<string, string>();

        var key = nameof(Cache__Is_Idempotent);
        var first = dict.Cache(key, () => Guid.NewGuid().ToString());
        var second = dict.Cache(key, () => Guid.NewGuid().ToString());

        IsOk(first);
        IsOk(second);
        IsOk(first, second);
    }

    [TestMethod]
    public void Cache_Clear_Cache__Does_Recompute()
    {
        var dict = new ShardedDictionary<string, string>();

        var key = nameof(Cache_Clear_Cache__Does_Recompute);
        var first = dict.Cache(key, () => "a");

        dict.Clear();

        var second = dict.Cache(key, () => "b");

        IsOk(first, "a");
        IsOk(second, "b");
        IsFail(first, second);
    }

    [TestMethod]
    public void Concurrent_Cache__Executes_MinimumTwice()
    {
        var dict = new ShardedDictionary<string, int>();
        var calls = 0;

        var key = nameof(Concurrent_Cache__Executes_MinimumTwice);

        Parallel.For(0, 50, _ =>
        {
            dict.Cache(key, () => Interlocked.Increment(ref calls));
        });

        IsOk(dict.Count, 1, "wrong count " + dict.Count);

        IsOk(calls < 49 && calls > 1, message: "Calls too high or too low: " + calls);

        var k = dict[key];

        IsOk(k);
    }

    [TestMethod]
    public void Cache_Remove_Cache_GetValue_Gets_ParallelSafe()
    {
        var dict = new ShardedDictionary<string, string>();
        const string prefix = nameof(Cache_Remove_Cache_GetValue_Gets_ParallelSafe);

        Parallel.For(0, 100, i =>
        {
            var key = prefix + (i % 5);

            dict.Cache(key, () => Guid.NewGuid().ToString());
            dict.TryRemove(key);
            dict.Cache(key, () => "final-" + key);
        });

        for (int i = 0; i < 5; i++)
        {
            var key = prefix + i;
            var value = dict.GetValue(key);

            IsOk(value, "final-");
        }
    }

    [TestMethod]
    public void CacheLifeCycle__Is_Valid()
    {
        string GetItem()
        {
            return "hello " + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
        }

        var key = nameof(CacheLifeCycle__Is_Valid);

        var dict = new ShardedDictionary<string, string>();

        var first = dict.Cache(key, GetItem);
        IsOk(first, "hello ");

        Thread.Sleep(25);

        var second = dict.Cache(key, GetItem);
        IsOk(first, second);

        Thread.Sleep(25);
        var third = dict.Cache(key, GetItem);
        IsOk(first, third);

        dict.Clear();

        var crashed = false;
        Task.Run(() =>
        {
            Thread.Sleep(5);
            var secondThreadValue = dict.Cache(key, GetItem);

            try
            {
                IsOk(secondThreadValue);
            }
            catch
            {
                crashed = true;
            }
        });

        Thread.Sleep(50);

        IsOk(!crashed);

        var fourth = dict.Cache(key, GetItem);

        IsOk(fourth, "hello ");

        IsFail(fourth, first);
    }
}