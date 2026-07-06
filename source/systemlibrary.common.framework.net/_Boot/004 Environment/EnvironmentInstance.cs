namespace SystemLibrary.Common.Framework.Boostrap;


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

    internal static string GetEnvironmentVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process)
            ?? Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
    }
}
