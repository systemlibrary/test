namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Converts an object array of integers or strings to a typed enum array.
    /// </summary>
    /// <remarks>
    /// Only supports int and string arrays — throws for any other element type.
    /// Assumes all elements in the array are the same type.
    /// </remarks>
    /// <example>
    /// <code>
    /// new object[] { 1, 2, 3 }.AsEnumArray&lt;Colors&gt;();       // by int index
    /// new object[] { "Red", "Black" }.AsEnumArray&lt;Colors&gt;(); // by key name
    /// </code>
    /// </example>
    public static TEnum[] AsEnumArray<TEnum>(this object[] objects) where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        if (objects.IsNot()) return default;

        var type = objects[0].GetType();

        if (type == SystemType.IntType)
            return objects.Cast<TEnum>().ToArray();

        else if (type == SystemType.StringType)
            return objects.Select(x => x.ToString().ToEnum<TEnum>()).ToArray();

        throw new Exception("Not supporting conversion of " + type.Name + "-array to the Enum");
    }

    /// <summary>
    /// Returns a sanitized text representation of the object's public properties in the specified format.
    /// </summary>
    /// <remarks>
    /// Only supports class types — throws <c>NotSupportedException</c> for value types.
    /// </remarks>
    public static string ToSanitizedObjectText(this object obj, ObjectSanitizedFormat format = ObjectSanitizedFormat.Plain, ObjectSanitizedFormatOptions options = null)
    {
        if (obj == null) return null;

        var type = obj.GetType();

        if (!type.IsClass)
            throw new NotSupportedException("Only class types are supported");

        if (type == typeof(string))
            return (string)obj;

        return ObjectSanitizedFormatter.Format(obj, format, options).ToString();
    }
}
