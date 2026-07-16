using System.Reflection;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for enum values.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Returns true if the enum value matches any of the provided values.
    /// </summary>
    /// <example>
    /// <code>
    /// Color.Red.IsAny(Color.Black, Color.Blue); // false
    /// Color.Red.IsAny(Color.Red, Color.Blue);   // true
    /// </code>
    /// </example>
    public static bool IsAny(this Enum enumField, params Enum[] values)
    {
        if (values == null) return false;

        foreach (var value in values)
            if (Equals(enumField, value))
                return true;

        return false;
    }

    /// <summary>
    /// Returns the <c>EnumText</c> attribute value, or the enum key name if not applied.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumText("White")] Black, Pink }
    ///
    /// Color.Black.ToText(); // "White"
    /// Color.Pink.ToText();  // "Pink"
    /// </code>
    /// </example>
    public static string ToText(this Enum enumField)
    {
        if (enumField == null) return null;

        var textAttribute = GetAttribute<EnumTextAttribute>(enumField, SystemType.EnumTextAttributeType);

        if (textAttribute != null)
            return textAttribute.Text;

        return enumField?.ToString();
    }

    /// <summary>
    /// Returns the <c>EnumText</c> attribute value, or null if not applied.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumText("White")] Black, Pink }
    ///
    /// Color.Black.GetEnumText(); // "White"
    /// Color.Pink.GetEnumText();  // null
    /// </code>
    /// </example>
    public static string GetEnumText(this Enum enumField)
    {
        if (enumField == null) return null;

        var textAttribute = GetAttribute<EnumTextAttribute>(enumField, SystemType.EnumTextAttributeType);

        return textAttribute?.Text;
    }

    /// <summary>
    /// Returns the <c>EnumValue</c> attribute value as string, or the enum key name if not applied.
    /// Enum keys prefixed with <c>_</c> followed by digits have the underscore stripped automatically.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumValue(1234)] Black, [EnumValue(null)] Blue, Pink }
    ///
    /// Color.Black.ToValue(); // "1234"
    /// Color.Blue.ToValue();  // null
    /// Color.Pink.ToValue();  // "Pink"
    /// </code>
    /// </example>
    public static string ToValue(this Enum enumField)
    {
        if (enumField == null) return null;

        var valueAttribute = GetAttribute<EnumValueAttribute>(enumField, SystemType.EnumValueAttributeType);

        if (valueAttribute != null)
        {
            return valueAttribute.Value?.ToString();
        }

        var value = enumField.ToString();

        if (value != null && value.Length > 1 && value[0] == '_' && char.IsDigit(value[1]))
        {
            // NOTE: Will remove underscore when format is: _[digits][any text]
            if (value.Length > 2)
            {
                if (char.IsDigit(value[2]))
                {
                    return value.Substring(1);
                }
            }
            else
            {
                return value.Substring(1);
            }
        }
        return value;
    }

    /// <summary>
    /// Returns the <c>EnumValue</c> attribute value as object, or null if not applied.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { [EnumValue(1234)] Black, [EnumValue(null)] Blue, Pink }
    ///
    /// Color.Black.GetEnumValue(); // 1234 as int
    /// Color.Blue.GetEnumValue();  // null
    /// Color.Pink.GetEnumValue();  // null
    /// </code>
    /// </example>
    public static object GetEnumValue(this Enum enumField)
    {
        if (enumField == null) return null;

        var valueAttribute = GetAttribute<EnumValueAttribute>(enumField, SystemType.EnumValueAttributeType);

        return valueAttribute?.Value;
    }

    static T GetAttribute<T>(object value, Type type) where T : Attribute
    {
        if (value == null) return default;

        var enumType = value.GetType();

        // TODO: Add cache for enumType, value.ToString...

        var members = enumType.GetMember(value.ToString());

        return GetFirstAttributeFromMembers<T>(members, type);
    }

    static T GetFirstAttributeFromMembers<T>(MemberInfo[] members, Type type) where T : Attribute
    {
        if (members?.Length > 0)
        {
            foreach (var member in members)
            {
                var attributes = member.GetCustomAttributes(type, inherit: false);
                if (attributes.IsNot()) continue;

                return (T)attributes[0];
            }
        }
        return default;
    }

    public static T ToEnum<T>(this Enum enumKey) where T : struct, IComparable, IFormattable, IConvertible
    {
        if (enumKey == null) return default(T);

        return enumKey.ToValue().ToEnum<T>();
    }
}
