namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Assigns a display text to an enum field, used in serialization and string conversion.
/// Falls back to the enum key name when not applied.
/// </summary>
/// <example>
/// <code>
/// enum Color
/// {
///     [EnumText("Black Colored Text")]
///     Black,
///     White
/// }
///
/// Color.Black.ToText();      // "Black Colored Text"
/// Color.White.ToText();      // "White" — fallback to key name
/// Color.Black.GetEnumText(); // "Black Colored Text"
/// Color.White.GetEnumText(); // null — attribute not applied
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumTextAttribute : Attribute
{
    /// <summary>
    /// The display text for this enum field.
    /// </summary>
    public string Text;

    /// <summary>
    /// Sets the display text for this enum field.
    /// </summary>
    public EnumTextAttribute(string text = null)
    {
        Text = text;
    }
}