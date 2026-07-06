using System.Reflection;

using SystemLibrary.Common.Framework.Attributes;

namespace SystemLibrary.Common.Framework;

partial class Config<T>
{
    static void DecryptPublicGetSetProperties(object instance, Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty)?.Where(prop => prop.PropertyType == SystemType.StringType);

        if (properties == null) return;

        foreach (var property in properties)
        {
            if (property == null) continue;

            if (!property.CanWrite || !property.CanRead) continue;

            var isEligibleForDecryption = property.Name.EndsWith("Decrypt", StringComparison.Ordinal);

            var attribute = property.GetCustomAttribute<ConfigDecryptAttribute>();

            // Neither 'Decrypt' end suffix nor ConfigDecrypt attribute exists on property, continue as its a normal property
            if (!isEligibleForDecryption && attribute == null) continue;

            // Either 'Customer Property Name' set in attribute, or fallback the the property itself, removing 'Decrypt'
            // Allows a '[ConfigDecrypt]' directly on the property without specifing a name, the property itself will be decrypted
            var encryptedPropertyName = attribute?.PropertyName ?? property.Name.ReplaceAllWith("", "Decrypt");

            if (encryptedPropertyName.IsNot())
            {
                Log.Warning("[Config] " + type.Name + " has a property with ConfigDecrypt attribute but no name set. Invalid property name, skipping...");
                continue;
            }

            var encryptedProperty = FindEncryptedProperty(properties, encryptedPropertyName);

            if (encryptedProperty == null)
            {
                Log.Warning("[Config] Property " + type.Name + "." + encryptedPropertyName + " was not found. Cannot decrypt a non-existing property, skipping...");
                continue;
            }

            var cipherText = encryptedProperty.GetValue(instance) as string;

            if (cipherText == null)
            {
                Log.Warning("[Config] Property " + type.Name + "." + encryptedProperty.Name + " is null. Skipping decryption and continuing...");
                continue;
            }

            //var decryptedValue = cipherText.Decrypt();

            //if (decryptedValue != null)
            //{
            //    property.SetValue(instance, decryptedValue);
            //}
            //else
            //{
            //    Log.Warning("[Config] Decryption of " + type.Name + "." + encryptedProperty.Name + " returned null. Skipping...");
            //}
        }
    }

    static PropertyInfo FindEncryptedProperty(IEnumerable<PropertyInfo> properties, string encryptedPropertyName)
    {
        return properties.FirstOrDefault(x => x.Name == encryptedPropertyName);
    }
}
