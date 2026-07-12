namespace SystemLibrary.Common.Framework.Boostrap;

static class JsonBoot
{
    internal static void Strap() { }

    static JsonBoot()
    {
        var settings = FrameworkSettingsInstance.Current.Json;

        JsonInstance.AllowTrailingCommas = settings.AllowTrailingCommas;
        JsonInstance.IgnoreReadOnlyFields = settings.IgnoreReadOnlyFields;
        JsonInstance.IncludeFields = settings.IncludeFields;
        JsonInstance.JsonIgnoreCondition = settings.JsonIgnoreCondition;
        JsonInstance.JsonSecureAttributesEnabled = settings.JsonSecureAttributesEnabled;
        JsonInstance.MaxDepth = settings.MaxDepth;
        JsonInstance.PropertyNameCaseInsensitive = settings.PropertyNameCaseInsensitive;
        JsonInstance.WriteIndented = settings.WriteIndented;
    }
}
