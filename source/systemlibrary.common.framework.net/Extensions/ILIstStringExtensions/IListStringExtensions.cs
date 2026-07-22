namespace SystemLibrary.Common.Framework.Extensions;
/// <summary>
/// Extension methods for IList of strings.
/// </summary>
public static class IListStringExtensions
{
    /// <summary>
    /// Converts a list of strings to a list of <c>TEnum</c>, matching case-insensitively against key name, <c>EnumValue</c> and <c>EnumText</c>.
    /// Null and empty strings convert to the default enum value.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Cars { Car1, [EnumValue("Second car value")] Car2, [EnumText("Third car")] Car3 }
    ///
    /// new List&lt;string&gt; { null, "", "SECOND CAR VALUE", "third car" }.ToEnumList&lt;Cars&gt;();
    /// // { Car1, Car1, Car2, Car3 }
    /// </code>
    /// </example>
    public static List<TEnum> ToEnumList<TEnum>(this IList<string> collection) where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        if (collection.IsNot())
            return new List<TEnum>();

        var items = new List<TEnum>();

        foreach (var item in collection)
        {
            items.Add(item.ToEnum<TEnum>());
        }
        return items;
    }
}
