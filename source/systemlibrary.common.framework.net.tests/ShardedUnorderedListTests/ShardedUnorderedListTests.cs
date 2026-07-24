using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Framework.Tests;

[TestClass]
public class ShardedUnorderedListTests : BaseTest
{
    [TestMethod]
    public void Add__Adds_Data()
    {
        var list = new ShardedUnorderedList<string>();

        list.Add("a");

        var has = list.Contains("a");

        IsOk(has);
    }

    [TestMethod]
    public void Add__Increments_Count()
    {
        var list = new ShardedUnorderedList<string>();

        IsOk(list.Count, 0);

        list.Add("a");
        list.Add("b");

        IsOk(list.Count, 2);
    }

    [TestMethod]
    public void Remove__Deletes_Data()
    {
        var list = new ShardedUnorderedList<string>();

        list.Add("a");
        list.Remove("a");

        var has = list.Contains("a");

        IsFail(has);
    }

    [TestMethod]
    public void Remove__Decrements_Count()
    {
        var list = new ShardedUnorderedList<string>();

        list.Add("a");
        list.Add("b");
        list.Remove("a");

        IsOk(list.Count, 1);
    }

    [TestMethod]
    public void Enumeration__Enumerates_All()
    {
        var list = new ShardedUnorderedList<string>();

        list.Add("a");
        list.Add("b");

        var count = 0;

        foreach (var item in list)
        {
            IsOk(item);
            count++;
        }

        IsOk(count, 2);
    }

    [TestMethod]
    public void Add_Remove_Add__Has_Data()
    {
        var list = new ShardedUnorderedList<string>();

        list.Add("a");
        list.Remove("a");
        list.Add("a");

        IsOk(list.Contains("a"));
        IsOk(list.Count, 1);
    }

    [TestMethod]
    public void Add__Adds_ParallelSafe()
    {
        var list = new ShardedUnorderedList<string>();

        Parallel.For(0, 5000, i =>
        {
            list.Add("a " + i + ".");
        });

        var items = list.Items;

        IsOk(items.Count(), 5000);

        for (int i = 0; i < 5000; i++)
        {
            IsOk(list.Contains("a " + i + "."));
        }
    }

    [TestMethod]
    public async Task Add__Adds_ThreadSafe()
    {
        var list = new ShardedUnorderedList<string>();

        for (int i = 0; i < 2000; i++)
        {
            var r = Randomness.Int(1, 100);
            var text = "hello " + i + ".";
            _ = Task.Run(async () =>
            {
                await Task.Delay(r);

                list.Add(text);
            });
        }

        Thread.Sleep(33);

        var items = list.Items.ToList();

        IsOk(items.Count() > 30, message: "Item count too low: " + items.Count); ;
        IsOk(items.Count() < 1970, message: "Item count too high: " + items.Count);
    }
}