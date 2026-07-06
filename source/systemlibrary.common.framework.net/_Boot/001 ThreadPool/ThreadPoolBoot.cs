namespace SystemLibrary.Common.Framework.Boostrap;

internal static class ThreadPoolBoot
{
    internal static void Strap() { }

    static ThreadPoolBoot()
    {
        var cpu = Environment.ProcessorCount;

        ThreadPool.SetMinThreads(cpu * 2, Math.Max(1, cpu / 2));
    }
}