using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

static internal class JsonSerializerInstance
{
    // TODO: Consider moving parts into the Boot, statically created classes, requiring and reading based on Environment should be in the Bootstrap system ready to be used throughout "as is" (readonly mode)
    static IntJsonConverter IntJsonConverter = new IntJsonConverter();
    static DateTimeJsonConverter DateTimeJsonConverter = new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss");
    static DateTimeOffsetJsonConverter DateTimeOffsetJsonConverter = new DateTimeOffsetJsonConverter("yyyy-MM-dd HH:mm:ss zzz");
    static TypeConverter TypeConverter = new TypeConverter();
    static LongJsonConverter LongJsonConverter = new LongJsonConverter();
    static DelegateJsonConverter DelegateJsonConverter = new DelegateJsonConverter();
    static StringCoercionConverter StringJsonConverter = new StringCoercionConverter();
    static BlacklistConverterFactory BlacklistConverterFactory = new BlacklistConverterFactory();
    static EncodingConverter EncodingConverter = new EncodingConverter();
    static GuidJsonConverter GuidJsonConverter = new GuidJsonConverter();
    static UriJsonConverter UriJsonConverter = new UriJsonConverter();
    static StackTraceConverter StackTraceConverter = new StackTraceConverter();

    internal static JsonSerializerOptions GetNDJsonOptions()
    {
        var options = GetJsonLogOptions();

        options.WriteIndented = false;

        return options;
    }

    internal static JsonSerializerOptions GetJsonLogOptions()
    {
        var options = GetOptions();

        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.WriteIndented = true;
        options.MaxDepth = 5;
        options.IncludeFields = true;
        options.IgnoreReadOnlyProperties = false;
        options.PropertyNameCaseInsensitive = true;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        var blackListedMemberNames = JsonPropertyBlacklist.MemberNames.Select(x => x.toCamelCase()).ToHashSet();
        var obfuscatedMemberNames = JsonPropertyObfuscate.MemberNames.Select(x => x.toCamelCase()).ToHashSet();

        options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
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

        return options;
    }

    internal static JsonSerializerOptions GetOptions(JsonSerializerOptions options = null, params JsonConverter[] converters)
    {
        if (options == null && converters == null) return CreateNewDefaultOptions();

        if (options == null)
        {
            options = CreateNewDefaultOptions();

            foreach (var converter in converters)
                options.Converters.Insert(0, converter);
        }
        else
        {
            AddConverters(options);

            if (options.ReferenceHandler == null ||
                options.MaxDepth <= 0 ||
                options.PropertyNamingPolicy == null ||
                options.Encoder == null)
            {
                var tmp = CreateNewDefaultOptions();

                if (options.ReferenceHandler == null)
                    options.ReferenceHandler = tmp.ReferenceHandler;

                if (options.MaxDepth <= 0)
                    options.MaxDepth = tmp.MaxDepth;

                if (options.PropertyNamingPolicy == null)
                    options.PropertyNamingPolicy = tmp.PropertyNamingPolicy;

                if (options.Encoder == null)
                    options.Encoder = tmp.Encoder;
            }
        }

        return options;
    }

    static void AddConverters(JsonSerializerOptions options)
    {
        if (options.Converters?.Count > 0) return;

        options.Converters.Add(StringJsonConverter);
        options.Converters.Add(IntJsonConverter);
        options.Converters.Add(new EnumStringConverterFactory());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(DateTimeJsonConverter);
        options.Converters.Add(DateTimeOffsetJsonConverter);
        options.Converters.Add(LongJsonConverter);
        options.Converters.Add(TypeConverter);
        options.Converters.Add(DelegateJsonConverter);
        options.Converters.Add(EncodingConverter);
        options.Converters.Add(GuidJsonConverter);
        options.Converters.Add(UriJsonConverter);
        options.Converters.Add(BlacklistConverterFactory);
        options.Converters.Add(StackTraceConverter);
    }

    // This is for JSON escaping inside HTML
    //static JavaScriptEncoder JavaScriptEncoder = JavaScriptEncoder.Create(
    //                        UnicodeRanges.BasicLatin,
    //                        UnicodeRanges.LatinExtendedA,
    //                        UnicodeRanges.LatinExtendedB,
    //                        UnicodeRanges.LatinExtendedAdditional,
    //                        UnicodeRanges.LatinExtendedC,
    //                        UnicodeRanges.Latin1Supplement,
    //                        UnicodeRanges.CurrencySymbols,
    //                        UnicodeRanges.Cyrillic,
    //                        UnicodeRanges.GreekandCoptic);

    static JavaScriptEncoder JavaScriptEncoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    static JsonSerializerOptions CreateNewDefaultOptions()
    {
        // Note: cannot be singleton, crashes in high concurrency
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder,
            DefaultIgnoreCondition = JsonInstance.JsonIgnoreCondition,
            MaxDepth = JsonInstance.MaxDepth,
            AllowTrailingCommas = JsonInstance.AllowTrailingCommas,
            PropertyNameCaseInsensitive = JsonInstance.PropertyNameCaseInsensitive,
            WriteIndented = JsonInstance.WriteIndented,
            PropertyNamingPolicy = null,
            IncludeFields = JsonInstance.IncludeFields,
            IgnoreReadOnlyFields = JsonInstance.IgnoreReadOnlyFields,
            ReadCommentHandling = JsonCommentHandling.Skip,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };

        AddConverters(options);

        return options;
    }
}
