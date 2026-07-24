namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class FrameworkSettingsInstance
{
    internal static FrameworkSettings Current;

    static FrameworkSettingsInstance()
    {
        Boot.Strap();
    }
}