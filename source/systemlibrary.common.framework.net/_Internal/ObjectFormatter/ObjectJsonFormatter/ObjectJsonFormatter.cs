using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectJsonFormatter
{
    static JsonStringEnumConverter JsonStringEnumConverter;

    static JsonSerializerOptions JsonOptions;

    static ObjectJsonFormatter()
    {
        JsonStringEnumConverter = new JsonStringEnumConverter();

        JsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            MaxDepth = 5,
            IncludeFields = true,
            IgnoreReadOnlyProperties = false,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var blackListedMemberNames = ObjectFormatterBlacklist.MemberNames.Select(x => x.toCamelCase()).ToHashSet();
        var obfuscatedMemberNames = ObjectFormatterObfuscate.MemberNames.Select(x => x.toCamelCase()).ToHashSet();

        JsonOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                 ti => {
                    if (ti.Properties == null) return;

                    foreach (var prop in ti.Properties)
                    {
                        if(prop?.Name == null) continue;

                        if (blackListedMemberNames.Contains(prop.Name))
                        {
                            prop.ShouldSerialize = (obj, value) => false;
                        }
                        else if (obfuscatedMemberNames.Contains(prop.Name))
                        {
                            prop.CustomConverter = new ObjectJsonFormatterStringConverter();
                        }
                    }

                    var memberOrder = FormatInstance.ObjectTextFormatterMemberOrder;
                    if(memberOrder?.Length > 0)
                    {
                        for (int i = 0; i < memberOrder.Length; i++)
                        {
                            var memberName = memberOrder[i];

                            var property = ti.Properties.FirstOrDefault(x => x.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

                            if (property != null)
                            {
                                property.Order = -1000 + i;
                            }
                        }
                    }
                }
            }
        };

        JsonOptions.Converters.Add(JsonStringEnumConverter);
    }

    internal static StringBuilder Format(object obj, ObjectFormatterOptions options)
    {
        if (obj is string msg) return new StringBuilder("{\n\t\"message\" : " + JsonSerializer.Serialize(msg, JsonOptions) + "\n}");

        return new StringBuilder(System.Text.Json.JsonSerializer.Serialize(obj, JsonOptions));
    }
}
