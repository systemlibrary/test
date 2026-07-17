namespace SystemLibrary.Common.Framework.Boostrap;

static class LogBoot
{
    internal static void Strap() { }

    static LogBoot()
    {
        var settings = FrameworkSettingsInstance.Current.Log;

        LogInstance.FullFilePath = settings.FullFilePath._ToOsFriendlyPath();

        LogInstance.MinLogLevel = GetMinLogLevel(settings.Level);

        LogInstance.IsLogDisabled = LogInstance.MinLogLevel == LogLevel.None;

        LogInstance.Format = settings.Format;
        LogInstance.StdForward = settings.StdForward;
        LogInstance.LogType = settings.LogForward;

        InitializeLogFilePath();
    }

    static void InitializeLogFilePath()
    {
        var path = LogInstance.FullFilePath;

        if (path.IsNot()) return;

        var folder = Path.GetDirectoryName(path);

        try
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
        catch (Exception ex)
        {
            FrameworkLog.Error("Log directory was not created: " + ex.Message + ". FullFilePath: " + LogInstance.FullFilePath);
            //BootstrapLog.Error(Directory could not be created: " + ex.Message);
        }
    }

    static LogLevel GetMinLogLevel(LogLevel minLogLevel)
    {
        // Not specified for the package 'systemLibraryCommonFramework', let's check the global logging level
        if (minLogLevel == LogLevel.Unset)
        {
            // If '0/unset', let's use the default log level for 'logging' 
            var defaultLogLevel = AppSettings.Current.Logging.LogLevel?.Default?.ToLower();

            if (defaultLogLevel.Is())
            {
                if (defaultLogLevel == "none")
                    minLogLevel = LogLevel.None;
                else
                    minLogLevel = defaultLogLevel.ToEnum<LogLevel>();
            }
            else
                minLogLevel = LogLevel.Debug; // Debug, Error and Critical are logged by default
        }
        return minLogLevel;
    }
}
