using System.ComponentModel;
using System.Globalization;
namespace SystemLibrary.Common.Framework.Boostrap;

internal static class EnumConvertersBoot
{
    internal static void Strap() { }

    static EnumConvertersBoot()
    {
        var enumType = typeof(Enum);
        var enumTypeConverter = TypeDescriptor.GetConverter(enumType);

        TypeDescriptor.AddAttributes(enumType, new TypeConverterAttribute(typeof(ExtendedEnumConverter)));

        TypeDescriptor.AddAttributes(SystemType.BoolType,new TypeConverterAttribute(typeof(ExtendedBoolConverter)));
    }
}
