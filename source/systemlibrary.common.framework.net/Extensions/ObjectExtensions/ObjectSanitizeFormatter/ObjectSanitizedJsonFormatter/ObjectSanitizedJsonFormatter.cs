using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectSanitizedJsonFormatter
{
    static JsonStringEnumConverter JsonStringEnumConverter;

    static JsonSerializerOptions JsonOptions;

    static ObjectSanitizedJsonFormatter()
    {
        JsonStringEnumConverter = new JsonStringEnumConverter();

        JsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            MaxDepth = 4,
            IncludeFields = true,
            IgnoreReadOnlyProperties = false,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var blackListedMemberNames = ObjectSanitizedBlacklist.MemberNames.Select(x => x.toCamelCase()).ToHashSet();
        var obfuscatedMemberNames = ObjectSanitizedObfuscate.MemberNames.Select(x => x.toCamelCase()).ToHashSet();

        JsonOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                ti =>
                {
                    if(ti.Properties != null)
                    {
                        foreach (var prop in ti.Properties)
                        {
                            if(prop?.Name == null) continue;

                            if (blackListedMemberNames.Contains(prop.Name))
                            {
                                prop.ShouldSerialize = (obj, value) =>
                                {
                                    return false;
                                };
                            }
                            else
                            {
                                if(obfuscatedMemberNames.Contains(prop.Name))
                                {
                                    prop.CustomConverter = new ObjectSanitizedStringJsonConverter();
                                }
                            }
                        }
                    }
                }
            }
        };

        JsonOptions.Converters.Add(JsonStringEnumConverter);
    }

    internal static StringBuilder Format(object obj, ObjectSanitizedFormatOptions options)
    {
        if (obj is string msg) return new StringBuilder("{\n\t\"message\" : " + JsonSerializer.Serialize(msg, JsonOptions) + "\n}");
        return new StringBuilder(System.Text.Json.JsonSerializer.Serialize(obj, JsonOptions));
    }
}
