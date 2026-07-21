namespace SystemLibrary.Common.Framework;

internal static class Events
{
    static int IsShuttingDown;

    public static event Action Shutdown;
    
    internal static void OnShutdownEvent()
    {
        if (Interlocked.Exchange(ref IsShuttingDown, 1) != 0) return;

        var handlers = Shutdown;
        if (handlers != null)
        {
            foreach (Action handler in handlers.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        Thread.Sleep(80);
    }
}
