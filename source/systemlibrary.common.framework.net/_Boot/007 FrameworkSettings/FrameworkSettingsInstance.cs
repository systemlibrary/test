namespace SystemLibrary.Common.Framework.Boostrap;

internal static class FrameworkSettingsInstance
{
    internal static FrameworkSettings Current;

    static FrameworkSettingsInstance()
    {
        Boot.Strap();
    }
}