using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

static internal class JsonSerializerOptionsFactory
{
    static JsonSerializerOptions _Template;
    static JsonSerializerOptions Template
    {
        get
        {
            if (_Template == null)
                _Template = Create();

            return _Template;
        }
    }

    static JavaScriptEncoder JavaScriptEncoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

    static IntJsonConverter IntJsonConverter = new IntJsonConverter();
    static DateTimeJsonConverter DateTimeJsonConverter = new DateTimeJsonConverter(FormatInstance.IsoDateTimeFormat);
    
    static DateTimeOffsetJsonConverter DateTimeOffsetJsonConverter = new DateTimeOffsetJsonConverter(FormatInstance.IsoDateTimeOffsetFormat);
    static TypeConverter TypeConverter = new TypeConverter();
    static LongJsonConverter LongJsonConverter = new LongJsonConverter();
    static DelegateJsonConverter DelegateJsonConverter = new DelegateJsonConverter();
    static StringCoercionConverter StringJsonConverter = new StringCoercionConverter();
    static BlacklistConverterFactory BlacklistConverterFactory = new BlacklistConverterFactory();
    static EncodingConverter EncodingConverter = new EncodingConverter();
    static GuidJsonConverter GuidJsonConverter = new GuidJsonConverter();
    static UriJsonConverter UriJsonConverter = new UriJsonConverter();
    static StackTraceConverter StackTraceConverter = new StackTraceConverter();

    internal static JsonSerializerOptions Get(JsonSerializerOptions options = null, bool? excludeNullValues = null, int? maxDepth = null, bool? writeIndented = null, bool orderPolicy = false, HashSet<string> blacklistedPropertyNames = null, HashSet<string> obfuscatedPropertyNames = null, params JsonConverter[] converters)
    {
        options = options ?? Create();

        AddConverters(options);

        if (converters != null)
        {
            foreach (var converter in converters)
                options.Converters.Insert(0, converter);
        }

        if (options.ReferenceHandler == null ||
            options.MaxDepth <= 0 ||
            options.PropertyNamingPolicy == null ||
            options.Encoder == null)
        {
            if (options.ReferenceHandler == null)
                options.ReferenceHandler = Template.ReferenceHandler;

            if (options.MaxDepth <= 0)
                options.MaxDepth = Template.MaxDepth;

            if (options.PropertyNamingPolicy == null)
                options.PropertyNamingPolicy = Template.PropertyNamingPolicy;

            if (options.Encoder == null)
                options.Encoder = Template.Encoder;
        }

        if (maxDepth != null)
            options.MaxDepth = maxDepth.Value;

        if (writeIndented != null)
            options.WriteIndented = writeIndented.Value;

        if (excludeNullValues != null)
        {
            if (excludeNullValues.Value)
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            else
                options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        }

        if (blacklistedPropertyNames != null ||
            obfuscatedPropertyNames != null)
        {
            options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
            {
                 ti => {
                    if (ti.Properties == null) return;

                    foreach (var prop in ti.Properties)
                    {
                        if(prop?.Name == null) continue;

                        if (blacklistedPropertyNames != null && blacklistedPropertyNames.Contains(prop.Name))
                        {
                            prop.ShouldSerialize = (obj, value) => false;
                        }
                        else if (obfuscatedPropertyNames != null && obfuscatedPropertyNames.Contains(prop.Name))
                        {
                            prop.CustomConverter = new ObjectJsonFormatterStringConverter();
                        }
                    }

                    if(orderPolicy)
                     {
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
            }
            };
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


    static JsonSerializerOptions Create()
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

        return options;
    }
}
