using System.Runtime.Loader;

namespace SystemLibrary.Common.Framework.Boostrap;

internal class BootstrapLog
{
    public static void Write(object o)
    {
        var m = "[BootLog] " + o + "\n";

        Console.Out.WriteLine(m);

        System.IO.File.AppendAllText(@"C:\logs\log.txt", m);
    }
}
internal partial class Boot
{
    // Warms up the CLR thread pool to prevent initial async starvation and serial fallback.
    public static async Task StrapAsync()
    {
        var tasks = new Task<string>[1];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                await Task.Yield();

                return "TaskApiBoundaryReached";
            });
        }

        var results = await Task.WhenAll(tasks);

        await Task.CompletedTask;
    }

    public static void Strap()
    {
    }

    static Boot()
    {
        try
        {
            BootstrapLog.Write("START");

            RegisterShutdownEvent();

            ThreadPoolBoot.Strap();

            Base62Boot.Strap();

            EnumConvertersBoot.Strap();

            EnvironmentBoot.Strap();

            AppRootBoot.Strap();

            BootstrapLog.Write("AppRoot Done");

            AppConfigBoot.Strap();

            BootstrapLog.Write("AppConfig Done");

            FrameworkSettingsBoot.Strap();

            BootstrapLog.Write("FrameworkSettings Done");

            AppBoot.Strap();

            BootstrapLog.Write("AppInstanceBoot Done");

            FormatBoot.Strap();

            LogBoot.Strap();

            BootstrapLog.Write("Log Done");

            CryptographyBoot.Strap();

            BootstrapLog.Write("Crypt Done");

            ServiceProviderBoot.Strap();

            BootstrapLog.Write("ServiceProvider Done");

            JsonBoot.Strap();

            CacheBoot.Strap();

            ClientBoot.Strap();

            MetricsBoot.Strap();

            HttpContextBoot.Strap();

            FrameworkLog.Flush();

            BootstrapLog.Write("DONE");
        }
        catch (Exception ex)
        {
            try
            {
                Console.Error.WriteLine("[Bootstrap Exception] " + ex);
            }
            catch
            {
                // swallow
            }

            FrameworkLog.Critical("[Bootstrap] " + ex);

            FrameworkLog.Flush();

            throw;
        }
    }

    static void RegisterShutdownEvent()
    {
        BootstrapLog.Write("Event Registered");
        AssemblyLoadContext.Default.Unloading += _ => LogFlusher.ShutdownFlush();
        AppDomain.CurrentDomain.DomainUnload += (s, e) => LogFlusher.ShutdownFlush();
    }
}