using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

partial class Config<T>
{
    static void SetPublicEnumFields(IConfiguration configuration, object instance, Type type)
    {
        if (instance == null) return;

        if (configuration == null) return;

        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField);

        if (fields == null) return;

        foreach (var field in fields)
        {
            var fieldType = field.FieldType;
            if (fieldType.IsEnum)
            {
                var value = configuration[field.Name];
                if (value != null)
                {
                    field.SetValue(instance, value.ToEnum(fieldType));
                }
            }
        }
    }
}
