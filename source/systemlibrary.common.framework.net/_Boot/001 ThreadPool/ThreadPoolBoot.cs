namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class ThreadPoolBoot
{
    internal static void Strap() { }

    static ThreadPoolBoot()
    {
        var cpu = Environment.ProcessorCount;

        ThreadPool.GetMinThreads(out var workers, out var io);

        ThreadPool.SetMinThreads(
            Math.Max(workers, Math.Max(2, cpu)),
            Math.Max(io, Math.Max(2, cpu / 2)));
    }
}