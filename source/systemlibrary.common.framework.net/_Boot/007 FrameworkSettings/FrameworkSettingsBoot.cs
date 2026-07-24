namespace SystemLibrary.Common.Framework.Bootstrap;

internal static partial class FrameworkSettingsBoot
{
    internal static void Strap() { }

    static FrameworkSettingsBoot()
    {
        FrameworkSettingsInstance.Current = AppSettings.Current.SystemLibraryCommonFramework;
    }
}