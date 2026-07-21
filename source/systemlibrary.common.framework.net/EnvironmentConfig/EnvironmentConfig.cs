using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Concrete environment configuration loaded from <c>environmentConfig.json</c>.
/// Extend with <c>EnvironmentConfig&lt;YourClass&gt;</c> if you need additional properties beyond <c>Name</c>.
/// </summary>
public class EnvironmentConfig : EnvironmentConfig<EnvironmentConfig, EnvironmentType>
{
    /// <summary>
    /// Returns true if neither <c>IsProd</c> nor <c>IsTest</c> — the default when no environment is configured.
    /// </summary>
    public static bool IsDevelopment;

    /// <summary>
    /// True for non-development, non-production environments such as Test, QA, Stage, UAT, Sandbox, etc.
    /// </summary>
    public static bool IsTest;

    /// <summary>
    /// True for test running environments, such as TestHost, Build, ContinuousIntegration, CI, UnitTest
    /// </summary>
    public static bool IsAutomation;

    /// <summary>
    /// Returns true if environment is <c>Prod</c> or <c>Production</c>.
    /// </summary>
    public static bool IsProd;

    /// <summary>
    /// Full path to the application root folder, never ending with a slash.
    /// Traverses up from <c>bin</c> unless the assembly name ends in <c>Tests</c>.
    /// </summary>
    public static readonly string ContentRootPath;

    static EnvironmentConfig()
    {
        ContentRootPath = AppRootInstance.ContentRootPath;

        IsTest = Current.EnvironmentType == EnvironmentType.AT ||
            Current.EnvironmentType == EnvironmentType.Integration ||
            Current.EnvironmentType == EnvironmentType.PreProd ||
            Current.EnvironmentType == EnvironmentType.PreProduction ||
            Current.EnvironmentType == EnvironmentType.QA ||
            Current.EnvironmentType == EnvironmentType.Sandbox ||
            Current.EnvironmentType == EnvironmentType.Stage ||
            Current.EnvironmentType == EnvironmentType.Staging ||
            Current.EnvironmentType == EnvironmentType.Test ||
            Current.EnvironmentType == EnvironmentType.UAT;

        IsAutomation = Current.EnvironmentType == EnvironmentType.Build ||
                Current.EnvironmentType == EnvironmentType.ContinuousIntegration ||
                Current.EnvironmentType == EnvironmentType.CI ||
                Current.EnvironmentType == EnvironmentType.UnitTest ||
                Current.EnvironmentType == EnvironmentType.TestHost;

        IsProd = Current.EnvironmentType == EnvironmentType.Prod ||
            Current.EnvironmentType == EnvironmentType.Production;

        IsDevelopment = !(IsProd || IsTest || IsAutomation);
    }
}