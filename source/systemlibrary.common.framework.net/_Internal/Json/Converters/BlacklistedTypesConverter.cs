using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.CSharp.RuntimeBinder;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class BlacklistConverterFactory : JsonConverterFactory
{
    static HashSet<Type> Blacklisted = new() {
        typeof(Exception),
        typeof(RuntimeWrappedException),
        typeof(EventHandler),
        typeof(Action),
        typeof(IntPtr),
        typeof(UIntPtr),
        typeof(Delegate),
        typeof(Thread),
        typeof(Stream),
        typeof(CancellationToken),
        typeof(Mutex),
        typeof(Semaphore),
        typeof(Process),
        typeof(GCHandle),
        typeof(WeakReference),
        typeof(RuntimeMethodHandle),
        typeof(RuntimeBinderException),
        typeof(RuntimeTypeHandle),
        typeof(System.Reflection.Assembly),
        typeof(Index),
        typeof(Range),
        typeof(Task),
        typeof(Task<>),
        typeof(CancellationTokenSource),
        typeof(HttpClient)
    };

    public override bool CanConvert(Type type)
    {
        var isBlacklisted = Blacklisted.Contains(type);

        return isBlacklisted;
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) => new BlacklistConverter(type);

    class BlacklistConverter : JsonConverter<object>
    {
        Type _type;
        public BlacklistConverter(Type type) => _type = type;

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            //if (value != null && FrameworkLog.IsDebug)
            //    FrameworkLog.Debug($"[JsonConverter] Member of type {_type.Name} is missing a [JsonIgnore], skipped...");
            
            if (_type.IsNullableType())
                writer.WriteNullValue();

            else if (_type.IsNumberType())
                writer.WriteNumberValue(0);

            else
            {
                writer.WriteNullValue();
            }
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => null;
    }
}
