namespace SystemLibrary.Common.Framework;

/// <summary>
/// Known environment names used by <c>EnvironmentConfig</c> for <c>IsDevelopment</c>, <c>IsTest</c> and <c>IsProd</c> checks.
/// Custom environment names still work for config transformations but won't map to those flags.
/// </summary>
public enum EnvironmentType
{
    Development = 0,
    Dev,
    Develop,
    Local,

    /// <summary>
    /// Local test runner environment (MsTest/xUnit/nUnit). Treated as development — <c>IsDevelopment</c> returns true.
    /// </summary>
    TestHost,

    Build,
    ContinuousIntegration,
    CI,
    UnitTest,

    /// <summary>
    /// Usually test runner CI/CD like github actions
    /// </summary>
    Test,

    Sandbox,
    Sb,

    QA,

    Integration,
    Inte,

    AT,

    UAT,

    Canary,

    Preview,

    Demo,

    Prep,

    Staging,
    Stage,

    PreProduction,
    PreProd,

    Production,
    Prod
}
