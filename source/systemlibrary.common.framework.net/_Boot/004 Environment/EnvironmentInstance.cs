namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class EnvironmentInstance
{
    internal static string[] Args;

    internal static string EnvironmentName;

    internal static EnvironmentType EnvironmentType;

    internal static bool IsDev;
    internal static bool IsTest;
    internal static bool IsProd;

    static EnvironmentInstance()
    {
        Boot.Strap();
    }
}
