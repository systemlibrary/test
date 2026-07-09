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

    static EnvironmentVariableTarget[] EnvironmentVariableTargetsOrder = new[]
    {
        EnvironmentVariableTarget.Process,
        EnvironmentVariableTarget.User,
        EnvironmentVariableTarget.Machine
    };

    internal static string GetEnvironmentVariable(string variable)
    {
        if (variable.IsNot()) return null;

        if (char.IsLower(variable[0]))
            variable = variable.ToUpperInvariant();

        if (TryGetEnvironmentVariable(variable, out string value))
            return value;
        
        variable = variable.ToLowerInvariant();

        if (TryGetEnvironmentVariable(variable, out value))
            return value;

        return null;
    }

    static bool TryGetEnvironmentVariable(string v, out string result)
    {
        result = null;

        foreach (var target in EnvironmentVariableTargetsOrder)
        {
            try
            {
                result = Environment.GetEnvironmentVariable(v, target);
                if (result.Is())
                    return true;
            }
            catch
            {
                // swallow:
                // access denied
                // unsupported targets

            }
        }
        return false;
    }
}
