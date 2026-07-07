using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace SystemLibrary.Common.Framework;

partial class Config<T>
{
    // NOTE: Why is this only on Fields inside a Config class which uses props only
    static void SetPublicEnumProperties(IConfiguration configuration, object instance, Type type)
    {
        if (instance == null) return;

        if (configuration == null) return;

        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField);

        if (props == null) return;

        foreach (var prop in props)
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var propertyType = prop.PropertyType;

            if (propertyType.IsEnum)
            {
                var value = configuration[prop.Name];
                if (value != null)
                {
                    prop.SetValue(instance, value.ToEnum(propertyType));
                }
            }
        }
    }
}
