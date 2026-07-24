using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class JsonInstance
{
    internal static bool AllowTrailingCommas;
    internal static int MaxDepth;
    internal static bool PropertyNameCaseInsensitive;
    internal static JsonIgnoreCondition JsonIgnoreCondition;
    internal static bool WriteIndented;
    internal static bool IgnoreReadOnlyFields;
    internal static bool IncludeFields;
    internal static bool JsonSecureAttributesEnabled;

    static JsonInstance()
    {
        Boot.Strap();
    }
}
