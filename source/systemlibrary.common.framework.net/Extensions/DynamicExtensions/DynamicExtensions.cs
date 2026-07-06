using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Static methods for dynamic and anonymous objects.
/// C# does not support extension methods on <c>dynamic</c> — call these as static methods directly.
/// </summary>
public static class DynamicExtensions
{
    internal static ConcurrentDictionary<int, PropertyInfo[]> FlattenPropertiesDictionary;

    static DynamicExtensions()
    {
        FlattenPropertiesDictionary = new ConcurrentDictionary<int, PropertyInfo[]>();
    }

    /// <summary>
    /// Merges one or more anonymous or dynamic objects into a new dynamic object.
    /// Later objects overwrite earlier ones on property name collision — property names are case-sensitive.
    /// Result is castable to <c>IDictionary&lt;string, object&gt;</c>.
    /// </summary>
    /// <remarks>
    /// If reflection fails on an updating object, cast it to a concrete type before passing it in.
    /// </remarks>
    /// <example>
    /// <code>
    /// var a = new { firstName = "world", age = 1 };
    /// var b = new { firstName = "hello", Age = 2 };
    /// var c = new { Age = 10 };
    ///
    /// var d = DynamicExtensions.Flatten(a, b, c);
    /// // d.firstName == "hello"
    /// // d.age       == 1
    /// // d.Age       == 10
    ///
    /// var dict = (IDictionary&lt;string, object&gt;)d;
    /// // dict["Age"] == 10
    /// </code>
    /// </example>
    public static dynamic Flatten(dynamic source, params object[] updates)
    {
        if (source == null && (updates == null || updates.Length == 0)) return null;

        var dictionary = new ExpandoObject() as IDictionary<string, object>;

        if (source != null)
        {
            var type = (Type)source.GetType();

            var properties = FlattenPropertiesDictionary.Cache(type, () =>
            {
                return type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            });

            foreach (PropertyInfo property in properties)
                if (property.CanRead)
                    dictionary[property.Name] = property.GetValue(source);
        }

        if (updates != null && updates.Length > 0)
        {
            foreach (var update in updates)
            {
                var type = update.GetType();

                var properties = FlattenPropertiesDictionary.Cache(type, () =>
                {
                    return type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                });

                foreach (PropertyInfo property in properties)
                    if (property.CanRead)
                        dictionary[property.Name] = property.GetValue(update);
            }
        }

        return dictionary as ExpandoObject;
    }
}
