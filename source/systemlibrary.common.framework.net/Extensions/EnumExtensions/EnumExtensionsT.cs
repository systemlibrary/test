namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Static methods for enum types — use when extension methods on generic types are needed.
/// </summary>
public static class EnumExtensions<TEnum> where TEnum : IComparable, IFormattable, IConvertible
{
    /// <summary>
    /// Returns all enum key names as strings, ignoring any <c>EnumText</c> or <c>EnumValue</c> attributes.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumText("Black Colored Text")] Black, White }
    ///
    /// var keys = EnumExtensions&lt;Color&gt;.GetKeys();
    /// // { "Black", "White" }
    /// </code>
    /// </example>
    public static IEnumerable<string> GetKeys()
    {
        var type = typeof(TEnum);

        if (!type.IsEnum) throw new Exception("Could not get values from a non-enum object. " + type.Name + " is not an Enum");

        var values = Enum.GetValues(type);

        foreach (var value in values)
        {
            yield return value.ToString();
        }
    }


    /// <summary>
    /// Returns all enum values as <c>TEnum</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// var enums = EnumExtensions&lt;Color&gt;.GetEnums();
    /// // { Color.Black, Color.White }
    /// </code>
    /// </example>
    public static IEnumerable<TEnum> GetEnums()
    {
        var type = typeof(TEnum);
        if (!type.IsEnum) throw new Exception("Could not get values from a non-enum object. " + type.Name + " is not an Enum");

        var values = Enum.GetValues(type);

        foreach (var value in values)
        {
            yield return (TEnum)value;
        }
    }

    /// <summary>
    /// Returns all enum values via <c>ToValue()</c> — respects <c>EnumValue</c> attributes, falls back to key name.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumValue("#000")] Black, White }
    ///
    /// var values = EnumExtensions&lt;Color&gt;.GetValues();
    /// // { "#000", "White" }
    /// </code>
    /// </example>
    public static IEnumerable<string> GetValues()
    {
        var enums = GetEnums();

        foreach (var value in enums)
        {
            var e = value as Enum;

            yield return e.ToValue();
        }
    }
}
