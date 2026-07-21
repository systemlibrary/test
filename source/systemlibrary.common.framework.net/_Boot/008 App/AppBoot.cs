namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppBoot
{
    internal static void Strap() { }

    static AppBoot()
    {
        var settings = FrameworkSettingsInstance.Current.App;

        AppInstance.Name = settings.Name != "company_unique_appname" ? settings.Name : FallbackNameForTestAndBenchmarkProjects();
        AppInstance.Debug = settings.Debug;
        AppInstance.Diagnostics = settings.Diagnostics;
        AppInstance.License = settings.License;
        AppInstance.Salt = Salt();
        AppInstance.HostName = HostName();
    }

    internal static string HostName()
    {
        var hostName = EnvironmentVariable.Get("WEBSITE_HOSTNAME");

        if (hostName.IsNot())
            hostName = EnvironmentVariable.Get("ASPNETCORE_PRIMARYDOMAIN") ?? "";

        if (hostName.Is())
        {
            var colonIndex = hostName.IndexOf(':');
            if (colonIndex > 0)
                hostName = hostName.Substring(0, colonIndex);

            if (hostName.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                hostName = hostName.Substring(4);
        }

        return hostName;
    }

    internal static int Salt()
    {
        var name = AppInstance.Name;

        var appname = name
            .Replace(" ", "")
            .MaxLength(4)
            .ToLowerInvariant();

        var salt = 17;

        for (int i = 0; i < appname.Length; i++)
        {
            salt = salt * 31 + appname[i];
        }

        //FrameworkLog.Debug("[Bootstrap.App] Salt is: " + salt);

        return salt;
    }

    static string FallbackNameForTestAndBenchmarkProjects()
    {
        var name = MainAssembly.Name;

        if (name == null ||
            name == "testhost" ||
            (name.Length == 12 && name.StartsWith("Job-")))
            return "company_unique_appname";
        //FrameworkLog.Debug("[App] 'app:name' is " + name + ", which was found from assembly name: " + asmName);

        return name;
    }
}
