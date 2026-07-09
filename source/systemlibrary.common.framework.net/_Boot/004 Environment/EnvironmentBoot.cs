namespace SystemLibrary.Common.Framework.Boostrap;

static class EnvironmentBoot
{
    internal static void Strap() { }

    static EnvironmentBoot()
    {
        EnvironmentInstance.Args = GetCommandLineArgs();

        EnvironmentInstance.EnvironmentName = EnvironmentName();

        EnvironmentInstance.EnvironmentType = EnvironmentInstance.EnvironmentName.ToEnum<EnvironmentType>();

        EnvironmentInstance.IsDev = IsDev(EnvironmentInstance.EnvironmentType);
        EnvironmentInstance.IsProd = IsProd(EnvironmentInstance.EnvironmentType);

        if (EnvironmentInstance.EnvironmentName.IsNot())
        {
            //  FrameworkLog.Debug($"[Bootstrap.Configurations] Environment is unset, using default environment: {EnvironmentInstance.EnvironmentType}");
        }
    }

    static string[] GetCommandLineArgs()
    {
        return Environment.GetCommandLineArgs()
          .Where(x => x.Is())
          .Select(x => x.Trim())
          .Where(x => x.Is())
          .ToArray();
    }

    static string EnvironmentName()
    {
        string name = null;

        try
        {
            name = EnvironmentInstance.Args?
                .FirstOrDefault(a => a.StartsWith("--environment ", StringComparison.OrdinalIgnoreCase))?
                .Split(' ')
                .ElementAtOrDefault(1);

            if (name.IsNot())
                name = EnvironmentInstance.Args?
                    .FirstOrDefault(a => a.StartsWith("--environment=", StringComparison.OrdinalIgnoreCase))?
                    .Split('=')
                    .ElementAtOrDefault(1);
        }
        catch (Exception ex)
        {
            //FrameworkLog.Warning("[AppInstance] Command line arguments could not be read " + ex.Message);
        }

        if (name.IsNot())
        {
            try
            {
                name = EnvironmentInstance.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }
            catch (Exception ex)
            {
                // FrameworkLog.Warning("[AppInstance] ASPNETCORE_ENVIRONMENT could not be read " + ex.Message);
            }
        }

        if (name.IsNot())
        {
            try
            {
                name = EnvironmentInstance.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            }
            catch (Exception ex)
            {
                //  FrameworkLog.Warning("[AppInstance] DOTNET_ENVIRONMENT could not be read " + ex.Message);
            }
        }

        return name ?? "";
    }

    static bool IsDev(EnvironmentType env)
    {
        return env == Framework.EnvironmentType.Dev || env == Framework.EnvironmentType.Develop || env == EnvironmentType.Development || env == EnvironmentType.Local;
    }

    static bool IsProd(EnvironmentType env)
    {
        return env == Framework.EnvironmentType.Prod || env == Framework.EnvironmentType.Production;
    }
}
