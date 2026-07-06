namespace SystemLibrary.Common.Framework.Boostrap;

internal static partial class FrameworkSettingsBoot
{
    internal static void Strap() { }

    static FrameworkSettingsBoot()
    {
        FrameworkSettingsInstance.Current = AppSettings.Current.SystemLibraryCommonFramework;
    }
}