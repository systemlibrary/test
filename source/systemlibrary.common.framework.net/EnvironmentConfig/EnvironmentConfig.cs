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
    public static bool IsDevelopment
    {
        get
        {
            if (_IsDevelopment == null)
            {
                _IsDevelopment = !(IsProd || IsTest);
            }
            return _IsDevelopment.Value;
        }
    }
    static bool? _IsDevelopment;

    /// <summary>
    /// Returns true if environment is <c>Prod</c> or <c>Production</c>.
    /// </summary>
    public static bool IsProd
    {
        get
        {
            if (_IsProd == null)
            {
                _IsProd = Current.EnvironmentType == EnvironmentType.Prod || Current.EnvironmentType == EnvironmentType.Production;
            }
            return _IsProd.Value;
        }
    }
    static bool? _IsProd;

    /// <summary>
    /// Returns true for any non-development, non-production environment: Test, QA, Stage, Staging, Sandbox, PreProd, UAT, CI, Build and others.
    /// </summary>
    public static bool IsTest
    {
        // TODO: Fix this 'IsTest' should be all environment not in development, not prod and not the 'build server environment'?
        // and rather add another flag for 'IsMsTest', locally MsTest/xUnit/nUnit/testhost environment?
        get
        {
            if (_IsTest == null)
            {
                _IsTest = Current.EnvironmentType == EnvironmentType.AT ||
                    Current.EnvironmentType == EnvironmentType.Integration ||
                    Current.EnvironmentType == EnvironmentType.PreProd ||
                    Current.EnvironmentType == EnvironmentType.PreProduction ||
                    Current.EnvironmentType == EnvironmentType.QA ||
                    Current.EnvironmentType == EnvironmentType.Sandbox ||
                    Current.EnvironmentType == EnvironmentType.Stage ||
                    Current.EnvironmentType == EnvironmentType.Staging ||
                    Current.EnvironmentType == EnvironmentType.Test ||
                    Current.EnvironmentType == EnvironmentType.UAT ||
                    Current.EnvironmentType == EnvironmentType.UnitTest ||
                    Current.EnvironmentType == EnvironmentType.CI ||
                    Current.EnvironmentType == EnvironmentType.ContinuousIntegration ||
                    Current.EnvironmentType == EnvironmentType.Build;
            }
            return _IsTest.Value;
        }
    }
    static bool? _IsTest;

    /// <summary>
    /// Full path to the application root folder, never ending with a slash.
    /// Traverses up from <c>bin</c> unless the assembly name ends in <c>Tests</c>.
    /// </summary>
    public static readonly string ContentRootPath = AppRootInstance.ContentRootPath;

    internal static string GetEnvironmentVariable(string variable)
    {
        var value = Environment.GetEnvironmentVariable(variable);

        if (value != null) return value;

        return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
    }
}