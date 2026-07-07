using System.Diagnostics;
using System.Reflection;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class AppInstanceBoot
{
    internal static void Strap() { }

    static AppInstanceBoot()
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
        var hostName = EnvironmentInstance.GetEnvironmentVariable("WEBSITE_HOSTNAME");

        if (hostName.IsNot())
            hostName = EnvironmentInstance.GetEnvironmentVariable("ASPNETCORE_PRIMARYDOMAIN") ?? "";

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

        if (name == null) return 25000;

        var salt = 1;

        for (int i = 0; i < name.Length; i++)
        {
            salt = salt * 3 + name[i];
        }

        //FrameworkLog.Debug("[Bootstrap.App] Salt is: " + salt);

        return salt;
    }

    static string FallbackNameForTestAndBenchmarkProjects()
    {
        var name = MainAssembly.Name;

        if (name == null ||
            name.EndsWith("Tests") ||
            (name.Length == 12 && name.StartsWith("Job-")))
            return "company_unique_appname";

        var appname =  
            name.Replace(".", "")
            .Replace(" ", "")
            .MaxLength(6)
            .ToLowerInvariant();

        //FrameworkLog.Debug("[App] 'app:name' is " + name + ", which was found from assembly name: " + asmName);

        return appname;
    }
}
