using System.Reflection;
using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for Type.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns true if <c>thisType</c> inherits or implements <c>type</c>. Returns false if both types are the same.
    /// </summary>
    /// <example>
    /// <code>
    /// typeof(Car).Inherits(typeof(IVehicle)); // true
    /// typeof(Car).Inherits(typeof(Car));       // false
    /// </code>
    /// </example>
    public static bool Inherits(this Type thisType, Type type)
    {
        if (thisType == type) return false;

        return type.IsAssignableFrom(thisType);
    }

    /// <summary>
    /// Returns true if type is a <c>List&lt;T&gt;</c> or array. Does not check <c>IList</c> or <c>Dictionary</c>.
    /// </summary>
    public static bool IsListOrArray(this Type type)
    {
        if (type.IsArray) return true;
        if (type.IsGenericType == false) return false;

        return type.GetGenericTypeDefinition() == SystemType.ListGenericType;
    }

    /// <summary>
    /// Returns true if type is a <c>Dictionary</c> or <c>IDictionary</c>.
    /// </summary>
    public static bool IsDictionary(this Type type)
    {
        if (type.IsValueType) return false;

        if (type.IsEnum) return false;

        if (type.IsArray) return false;

        if (type.IsGenericType == false)
            return type == SystemType.IDictionaryType;

        if (type.Inherits(SystemType.IDictionaryType))
            return true;

        return
            type.GetGenericTypeDefinition() == SystemType.DictionaryGenericType ||
            type.GetGenericTypeDefinition() == SystemType.IDictionaryGenericType;
    }

    /// <summary>
    /// Returns the most meaningful type name — for generics and collections returns the first type argument name.
    /// </summary>
    /// <example>
    /// <code>
    /// typeof(Car).GetTypeName();           // "Car"
    /// typeof(List&lt;Car&gt;).GetTypeName(); // "Car"
    /// typeof(Car[]).GetTypeName();          // "Car"
    /// </code>
    /// </example>
    public static string GetTypeName(this Type type)
    {
        if (type.IsListOrArray())
        {
            if (type.GenericTypeArguments?.Length > 0)
            {
                return type.GenericTypeArguments[0].Name;
            }
            if (type.GenericTypeArguments != null) return "<>";
        }

        if (type.IsDictionary())
        {
            if (type.GenericTypeArguments?.Length > 1)
            {
                return type.GenericTypeArguments[0].Name + ", " + type.GenericTypeArguments[1].Name;
            }
            return "<,>";
        }

        var firstGenericType = type.GetTypeArgument();
        if (firstGenericType != null)
        {
            return firstGenericType.Name;
        }
        return type.Name;
    }

    /// <summary>
    /// Returns the generic type argument at the specified index, or null if not found.
    /// Searches interfaces if no direct generic arguments exist.
    /// </summary>
    /// <example>
    /// <code>
    /// typeof(List&lt;Car&gt;).GetTypeArgument(); // typeof(Car)
    /// </code>
    /// </example>
    public static Type GetTypeArgument(this Type type, int index = 0)
    {
        if (type == null) return default;

        var genericArguments = type.GetGenericArguments();

        if (genericArguments.Length > index) return genericArguments[index];

        foreach (var @interface in type.GetInterfaces())
        {
            if (@interface.IsGenericType)
            {
                var genericDefinition = @interface.GetGenericTypeDefinition();

                if (genericDefinition == SystemType.ICollectionType ||
                    genericDefinition == SystemType.ICollectionGenericType ||
                    genericDefinition == SystemType.IListGenericType ||
                    genericDefinition == SystemType.IDictionaryGenericType)
                {
                    genericArguments = @interface.GetGenericArguments();
                    if (genericArguments?.Length > index) return genericArguments[index];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns all generic type arguments, or null if none found.
    /// Searches interfaces if no direct generic arguments exist.
    /// </summary>
    /// <example>
    /// <code>
    /// typeof(List&lt;Car&gt;).GetTypeArguments(); // [ typeof(Car) ]
    /// </code>
    /// </example>
    public static Type[] GetTypeArguments(this Type type)
    {
        if (type == null) return default;

        var genericArguments = type.GetGenericArguments();

        if (genericArguments?.Length > 0) return genericArguments;

        foreach (var @interface in type.GetInterfaces())
        {
            if (@interface.IsGenericType)
            {
                var genericDefinition = @interface.GetGenericTypeDefinition();

                if (genericDefinition == SystemType.ICollectionType ||
                    genericDefinition == SystemType.ICollectionGenericType ||
                    genericDefinition == SystemType.IListGenericType ||
                    genericDefinition == SystemType.IDictionaryGenericType)
                {
                    var temp = @interface.GetGenericArguments();
                    if (temp?.Length > 0) return temp;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns true if type is a <c>KeyValuePair&lt;,&gt;</c>.
    /// </summary>
    public static bool IsKeyValuePair(this Type type)
    {
        if (type == null) return false;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == SystemType.KeyValuePairType)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets a static property or field by name, including private and internal members. Throws if member not found.
    /// </summary>
    public static void SetStaticMember(this Type type, string memberName, object value)
    {
        if (type == null) return;

        var property = type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic)?
                .Where(x => x.Name == memberName)?
                .FirstOrDefault();
        if (property == null)
        {
            property = type.GetProperties(BindingFlags.Static | BindingFlags.Public)?
                .Where(x => x.Name == memberName)?
                .FirstOrDefault();
        }
        if (property != null)
        {
            property.SetValue(null, value);
        }

        else
        {
            var field = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic)?
            .Where(x => x.Name == memberName)?
            .FirstOrDefault();

            if (field == null)
            {
                field = type.GetFields(BindingFlags.Static | BindingFlags.Public)?
                 .Where(x => x.Name == memberName)?
                 .FirstOrDefault();
            }

            if (field != null)
            {
                field.SetValue(null, value);
            }
            else
            {
                throw new Exception(memberName + " do not exist on " + type.Name + " as a static member");
            }
        }
    }

    /// <summary>
    /// Returns true if type is internal and non-nested.
    /// </summary>
    public static bool IsInternal(this Type type)
    {
        return type.IsNotPublic && !type.IsNested;
    }

    /// <summary>
    /// Returns a default instance for value types, or null for reference types and interfaces.
    /// </summary>
    public static object Default(this Type type)
    {
        if (type.IsEnum) return 0;
        if (type.IsInterface) return null;

        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    /// <summary>
    /// Returns true if type is a concrete, instantiable class — not abstract, interface, enum, array, native, dictionary or delegate.
    /// </summary>
    public static bool IsClassType(this Type type)
    {
        if (type == null) return false;

        return type.IsClass &&
            !type.IsInterface &&
            !type.IsAbstract && 
            !type.IsEnum &&
            !type.IsArray &&
            !type.IsNativeType() &&
            !type.IsDictionary() &&
            type != SystemType.TupleType &&
            !type.Inherits(typeof(ITuple)) &&
            !typeof(Delegate).IsAssignableFrom(type);
    }

    internal static bool IsNativeType(this Type type)
    {
        return type.IsPrimitive ||      // int, long, ...
           type.IsValueType ||          // DateTime, DateTimeOffset, ...
           type == SystemType.StringType ||
           type == SystemType.StringBuilderType ||
           type.IsNullableType();
    }

    /// <summary>
    /// Returns true if type is nullable.
    /// </summary>
    public static bool IsNullableType(this Type type)
    {
        if (type.IsEnum) return false;
        if (type.IsArray) return false;

        return type.IsGenericType && type.GetGenericTypeDefinition() == SystemType.NullableType;
    }

    /// <summary>
    /// Returns true if type is a numeric type — int, uint, long, ulong, short, float, double, decimal, byte, ushort or sbyte.
    /// </summary>
    public static bool IsNumberType(this Type type)
    {
        // TODO: Consider if nullable int etc is a "Number Type"
        return
            type == SystemType.IntType ||
            type == SystemType.UIntType ||
            type == SystemType.Int64Type ||
            type == SystemType.UInt64Type ||
            type == SystemType.Int16Type ||
            type == SystemType.FloatType ||
            type == SystemType.DoubleType ||
            type == SystemType.DecimalType ||
            type == SystemType.ByteType ||
            type == typeof(ushort) ||
            type == typeof(sbyte);
    }

    /// <summary>
    /// Returns true if type is a compiler-generated anonymous type.
    /// </summary>
    public static bool IsAnonymousType(this Type type)
    {
        if (!type.IsGenericType) return false;

        var name = type.Name;

        if (!name.StartsWith("<>")) return false;

        if (!name.Contains("AnonymousType")) return false;

        return Attribute.IsDefined(type, SystemType.CompilerGeneratedAttributeType);
    }
}