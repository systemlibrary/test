namespace SystemLibrary.Common.Framework;

internal static class OnShutdownInstance
{
    internal static void Shutdown()
    {
        var delay = LogFlusher.ForceFlush();

        if (delay)
            Thread.Sleep(80);
    }
}
