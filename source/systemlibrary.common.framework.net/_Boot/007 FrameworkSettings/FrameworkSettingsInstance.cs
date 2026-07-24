namespace SystemLibrary.Common.Framework.Bootstrap;

/// <summary>
/// Provides access to the framework configuration loaded from <c>appsettings.json</c>.
/// 
/// Settings that require validation or processing should have their own Boot and Instance classes. Example: AppBoot and AppInstance
/// </summary>
internal static class FrameworkSettingsInstance
{
    internal static FrameworkSettings Current;

    static FrameworkSettingsInstance()
    {
        Boot.Strap();
    }
}