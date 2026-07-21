using System;
using System.Collections.Generic;
using System.Text;

namespace SystemLibrary.Common.Framework;

internal static class EnvironmentVariable
{
    static EnvironmentVariableTarget[] EnvironmentVariableTargetsOrder = new[]
    {
            EnvironmentVariableTarget.Process,
            EnvironmentVariableTarget.User,
            EnvironmentVariableTarget.Machine
    };

    internal static string Get(string variable)
    {
        if (variable.IsNot()) return null;

        if (TryGetEnvironmentVariable(variable, out string value))
            return value;

        if (TryGetEnvironmentVariable(variable.ToUpperInvariant(), out value))
            return value;

        if (TryGetEnvironmentVariable(variable.ToLowerInvariant(), out value))
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
