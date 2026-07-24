
namespace SystemLibrary.Common.Framework.Bootstrap;

internal class CryptographySettings
{
    /// <summary>
    /// Set directory path which contains the 'frameworkenc-SomePassword.key' file or 'key.pub' and 'key.priv' files, and the filename will be used a the Global Encryption Key throughout the App whenever you invoke Encrypt or Decrypt
    /// <para>For instance on windows it could be outside the application: C:\src\keys\</para>
    /// <para>Or it can be a relative folder within your application that is protected and not served, like app_data for both windows and linux support: ./app_data/</para>
    /// <para>Relative folders must start with ./</para>
    /// </summary>
    public string KeyDirectory { get; set; }
}
