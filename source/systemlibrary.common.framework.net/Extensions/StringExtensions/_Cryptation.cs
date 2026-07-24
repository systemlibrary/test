using System.Text;

using Microsoft.AspNetCore.DataProtection;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;


namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    #region AES CBC
    /// <summary>
    /// Encrypts using AES CBC with the framework's default key. Returns a URL-safe base64 string.
    /// </summary>
    public static string Encrypt(this string data, Encoding encoding = null)
    {
        return Encrypt(data, (byte[])null, null, encoding);
    }

    /// <summary>
    /// Encrypts using AES CBC with the provided key and optional IV. Returns a URL-safe base64 string.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static string Encrypt(this string data, string key, string IV = null, Encoding encoding = null)
    {
        return Encrypt(data, key.GetBytes(), IV.GetBytes(), encoding);
    }

    /// <summary>
    /// Encrypts using AES CBC with the provided key and optional IV. Returns a URL-safe base64 string.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static string Encrypt(this string data, byte[] key, byte[] IV = null, Encoding encoding = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");

        if (IV != null && IV.Length != 16)
            throw new Exception("IV must be a length of 16 bytes for AES.");

        return Cryptation.EncryptAesCbc(data.GetBytes(encoding), key, IV).ToBase64();
    }

    /// <summary>
    /// Decrypts an AES CBC base64 string using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static string Decrypt(this string cipherText64, string key = null, Encoding encoding = null)
    {
        return Decrypt(cipherText64, key.GetBytes(), encoding);
    }

    /// <summary>
    /// Decrypts an AES CBC base64 string using the provided key.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static string Decrypt(this string cipherText64, byte[] key, Encoding encoding = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");

        return Cryptation.DecryptAesCbc(cipherText64, key).ToText(encoding);
    }
    #endregion

    #region AES GCM
    /// <summary>
    /// Encrypts using AES GCM with the framework's default key. Returns a URL-safe base64 string.
    /// </summary>
    public static string EncryptAesGcm(this string data, Encoding encoding = default)
    {
        return EncryptAesGcm(data, (byte[])null, null, encoding);
    }

    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional nonce. Returns a URL-safe base64 string.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes, nonce must be 12 bytes if provided.
    /// </remarks>
    public static string EncryptAesGcm(this string data, string key, string nonce = null, Encoding encoding = null)
    {
        return EncryptAesGcm(data, key.GetBytes(), nonce.GetBytes(), encoding);
    }

    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional nonce. Returns a URL-safe base64 string.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes, nonce must be 12 bytes if provided.
    /// </remarks>
    public static string EncryptAesGcm(this string data, byte[] key, byte[] nonce = null, Encoding encoding = null)
    {
        if (data.IsNot()) return data;

        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32 bytes. "+ key.Length);

        if (nonce != null && nonce.Length != 12)
            throw new Exception("nonce must be a length of 12 for AES.");

        return Cryptation.EncryptAesGcm(data.GetBytes(encoding), key, nonce).ToBase64();
    }

    /// <summary>
    /// Decrypts an AES GCM base64 string using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static string DecryptAesGcm(this string cipherText, string key = null, Encoding encoding = null)
    {
        return DecryptAesGcm(cipherText, key.GetBytes(), encoding);
    }

    /// <summary>
    /// Decrypts an AES GCM base64 string using the provided key.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static string DecryptAesGcm(this string cipherText, byte[] key, Encoding encoding = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32 bytes. " + key.Length);

        return Cryptation.DecryptAesGcm(cipherText, key).ToText(encoding);
    }
    #endregion

    #region RSA
    /// <summary>
    /// Encrypts using RSA with the public key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] EncryptRsa(this string data, string publicKeyFullPath = null)
    {
        return Cryptation.EncryptRsa(data.GetBytes(), publicKeyFullPath);
    }

    /// <summary>
    /// Decrypts using RSA with the private key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] DecryptRsa(this string data, string publicKeyFullPath = null)
    {
        return Cryptation.DecryptRsa(data.GetBytes(), publicKeyFullPath);
    }
    #endregion

    #region Dataprotection Key Ring
    /// <summary>
    /// Encrypts using the ASP.NET Data Protection key ring. Decryptable via <c>DecryptDataProtection</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// var cipher = "hello world".EncryptDataProtection();
    /// </code>
    /// </example>
    public static string EncryptDataProtection(this string data)
    {
        return ServicesInstance.DataProtector.Protect(data);
    }

    /// <summary>
    /// Decrypts a value encrypted via <c>EncryptUsingKeyRing</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "hello world".EncryptDataProtection().DecryptDataProtection();
    /// </code>
    /// </example>
    public static string DecryptDataProtection(this string data)
    {
        return ServicesInstance.DataProtector.Unprotect(data);
    }
    #endregion
}
