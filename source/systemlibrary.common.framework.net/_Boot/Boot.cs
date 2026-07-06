namespace SystemLibrary.Common.Framework.Boostrap;

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
            ThreadPoolBoot.Strap();

            Base62Boot.Strap();

            EnumConvertersBoot.Strap();

            EnvironmentBoot.Strap();

            AppRootBoot.Strap();

            AppConfigBoot.Strap();

            FrameworkSettingsBoot.Strap();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}