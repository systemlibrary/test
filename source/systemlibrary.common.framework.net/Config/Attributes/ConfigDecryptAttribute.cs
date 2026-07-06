namespace SystemLibrary.Common.Framework.Attributes;

/// <summary>
/// Marks a property as the target for a decrypted config value.
/// The named source property is decrypted once at startup and written into the decorated property.
/// Values must be encrypted using the parameterless string.Encrypt().
/// </summary>
/// <remarks>
/// Source property must exist on the same class and the class must inherit Config.
/// Only works on public instance properties with get and set accessors.
/// Decryption failures are logged as warnings — app continues without throwing.
/// Two conventions supported:
/// - Naming convention: suffix the encrypted property name with 'Decrypt' e.g. TokenDecrypt decrypts Token into itself
/// - Attribute convention: decorate any property with [ConfigDecrypt("Token")] to receive the decrypted value
/// </remarks>
/// <example>
/// apiConfig.json:
/// <code>
/// {
///     "token": "encrypted value here"
/// }
/// </code>
/// ApiConfig.cs:
/// <code>
/// class ApiConfig : Config&lt;ApiConfig&gt;
/// {
///     public string Token { get; set; }           // encrypted value loaded from config
///     public string TokenDecrypt { get; set; }    // naming convention — decrypts Token into this
///
///     [ConfigDecrypt("Token")]
///     public string TokenDec { get; set; }        // attribute convention — decrypts Token into this
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConfigDecryptAttribute : Attribute
{
    /// <summary>
    /// Name of the property whose encrypted value will be decrypted into this property.
    /// </summary>
    public string PropertyName;

    /// <summary>
    /// Pass the name of the source property containing the encrypted value.
    /// </summary>
    public ConfigDecryptAttribute(string propertyName = null)
    {
        PropertyName = propertyName;
    }
}
