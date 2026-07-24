namespace SystemLibrary.Common.Framework.Bootstrap;

internal class AppSettings : Config<AppSettings>
{
    public LoggingSettings Logging { get; set; }

    public FrameworkSettings SystemLibraryCommonFramework { get; set; }

    public AppSettings()
    {
        Logging = new LoggingSettings();
        SystemLibraryCommonFramework = new FrameworkSettings();
    }
}