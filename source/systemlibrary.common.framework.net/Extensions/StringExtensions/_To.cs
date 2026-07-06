using System.Reflection;

using SystemLibrary.Common.Framework.Attributes;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Converts a string to <c>TEnum</c>, matching case-insensitively against key name, <c>EnumValue</c> and <c>EnumText</c> attributes.
    /// Returns the default enum value if no match is found.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { None, [EnumText("White")] [EnumValue("BlackAndWhite")] Black, Pink }
    ///
    /// "black".ToEnum&lt;Color&gt;();        // Color.Black — key name match
    /// "white".ToEnum&lt;Color&gt;();        // Color.Black — EnumText match
    /// "blackAndWhite".ToEnum&lt;Color&gt;(); // Color.Black — EnumValue match
    /// "brown".ToEnum&lt;Color&gt;();        // Color.None — no match, returns default
    /// </code>
    /// </example>
    public static T ToEnum<T>(this string text) where T : struct, IComparable, IFormattable, IConvertible
    {
        if (text == null) return default(T);

        return (T)text.ToEnum(typeof(T));
    }

    /// <summary>
    /// Converts a string to the specified enum type returned as <c>object</c>.
    /// Matches case-insensitively against key name, <c>EnumValue</c> and <c>EnumText</c> attributes.
    /// Returns the first enum key if no match is found.
    /// </summary>
    public static object ToEnum(this string text, Type enumType)
    {
        object result;

        if (!enumType.IsEnum)
            return Activator.CreateInstance(enumType);

        MemberInfo[] members = null;

        if (text != null && text.Length > 0 && char.IsDigit(text[0]))
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
            {
                members = EnumMembersCached.Cache(enumType, () => enumType.GetMembers(BindingFlags.Public | BindingFlags.Static));
                var name = result.ToString();
                for (int i = 0; i < members.Length; i++)
                    if (members[i].Name == name)
                        return result;
            }
        }
        else if (text != null)
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
                return result;
        }
        else
        {
            // TODO: Add cache here... 

            Enum.TryParse(enumType, text, out result);
        }

        var checkUnderscore = text?.Length > 1;

        if (members == null)
            members = EnumMembersCached.Cache(enumType, () => enumType.GetMembers(BindingFlags.Public | BindingFlags.Static));

        for (int i = 0; i < members.Length; i++)
        {
            var member = members[i];

            // TODO: Cache GetCustomAttribute to avoid that lookup over n over
            if (member.GetCustomAttribute(SystemType.EnumValueAttributeType) is EnumValueAttribute valueAttr)
            {
                if (Equals(valueAttr.Value, text) || (valueAttr.Value + "").Equals(text, StringComparison.OrdinalIgnoreCase))
                    if (Enum.TryParse(enumType, member.Name, out result))
                        return result;
            }

            if (member.GetCustomAttribute(SystemType.EnumTextAttributeType) is EnumTextAttribute textAttr)
            {
                if (textAttr.Text?.Equals(text, StringComparison.OrdinalIgnoreCase) == true)
                    if (Enum.TryParse(enumType, member.Name, out result))
                        return result;
            }

            if (checkUnderscore == true &&
                member.Name.Length == text.Length + 1 &&
                member.Name[0] == '_' &&
                string.Compare(member.Name, 1, text, 0, text.Length, true) == 0)
            {
                if (Enum.TryParse(enumType, member.Name, out result))
                    return result;
            }
        }

        // TODO: Consider if "enum 0" is always the one thing we want to default return as, instead of the "first member", whatever that is...
        // TODO: Cache the instance, return the same always
        if (result == null || result == "")
            return Activator.CreateInstance(enumType);

        return result;
    }

}
