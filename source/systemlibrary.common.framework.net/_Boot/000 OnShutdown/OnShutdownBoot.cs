using System.Runtime.Loader;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class OnShutdownBoot
{
    internal static void Strap() { }

    static OnShutdownBoot()
    {
        Events.Shutdown += OnShutdownInstance.Shutdown;

        AssemblyLoadContext.Default.Unloading += _ => Events.OnShutdownEvent();
        AppDomain.CurrentDomain.DomainUnload += (s, e) => Events.OnShutdownEvent();
    }
}
