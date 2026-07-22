using System.Text.Json;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectJsonFormatter
{
    static JsonSerializerOptions jsonOptions;
    static bool InitialRequest = true;
    static object InitialRequestLock = new object();

    static ObjectJsonFormatter()
    {
        jsonOptions = JsonSerializerOptionsFactory.Get(null, true, 5, true, true, NamesBlacklisted.MemberNames, NamesObfuscated.MemberNames, null);
    }

    internal static string Format(object obj, ObjectFormatterOptions options)
    {
        if (InitialRequest)
        {
            lock (InitialRequestLock)
            {
                if (InitialRequest)
                {
                    InitialRequest = false;
                    jsonOptions.MaxDepth = options.MaxLevel;

                    if (!options.ExcludeNullMembers)
                        jsonOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
                }
            }
        }
        return System.Text.Json.JsonSerializer.Serialize(obj, jsonOptions);
    }
}
