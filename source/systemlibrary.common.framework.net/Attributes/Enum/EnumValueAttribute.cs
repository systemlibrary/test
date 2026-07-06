namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Assigns a custom value to an enum field, used in serialization and string conversion.
/// Falls back to the enum key name when not applied.
/// </summary>
/// <example>
/// <code>
/// enum Color
/// {
///     [EnumValue("#000")]
///     Black,
///     White,
///     Pink
/// }
///
/// Color.Black.ToValue();      // "#000"
/// Color.White.ToValue();      // "White" — fallback to key name
/// Color.Black.GetEnumValue(); // "#000" as object, castable to string
/// Color.Pink.GetEnumValue();  // null — attribute not applied
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumValueAttribute : Attribute
{
    /// <summary>
    /// The custom value for this enum field.
    /// </summary>
    public object Value;

    /// <summary>
    /// Sets the custom value for this enum field.
    /// </summary>
    public EnumValueAttribute(object value)
    {
        this.Value = value;
    }
}