using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

partial class ByteArrayExtensions
{
    #region AES CBC
    /// <summary>
    /// Encrypts using AES CBC with the framework's default key.
    /// </summary>
    public static byte[] Encrypt(this byte[] data)
    {
        return null;
        //return Encrypt(data, CryptographyInstance.Key, null);
    }

    /// <summary>
    /// Encrypts using AES CBC with the provided key and optional IV.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static byte[] Encrypt(this byte[] data, string key, string IV = null)
    {
        return null;
        //return Encrypt(data, key.GetBytes() ?? CryptographyInstance.Key, IV.GetBytes());
    }

    /// <summary>
    /// Encrypts using AES CBC with the provided key and optional IV.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static byte[] Encrypt(this byte[] data, byte[] key, byte[] IV = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");

        if (IV != null && IV.Length != 16)
            throw new Exception("IV must be a length of 16 bytes for AES.");

        return null;
        //return Cryptation.EncryptAesCbc(data, key ?? CryptographyInstance.Key, IV);
    }

    /// <summary>
    /// Decrypts AES CBC bytes using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] Decrypt(this byte[] cipherBytes, string key = null)
    {
        return null;
        //return Decrypt(cipherBytes, key.GetBytes() ?? CryptographyInstance.Key);
    }

    /// <summary>
    /// Decrypts AES CBC bytes using the provided key.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] Decrypt(this byte[] cipherBytes, byte[] key)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");
        return null;
        //return Cryptation.DecryptAesCbc(cipherBytes, key ?? CryptographyInstance.Key);
    }
    #endregion

    #region AES GCM
    /// <summary>
    /// Encrypts using AES GCM with the framework's default key.
    /// </summary>
    public static byte[] EncryptAesGcm(this byte[] data)
    {
        return null;
        //return EncryptAesGcm(data, CryptographyInstance.Key);
    }

    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional IV.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static byte[] EncryptAesGcm(this byte[] data, string key, string IV = null)
    {
        return null;
        //return EncryptAesGcm(data, key.GetBytes() ?? CryptographyInstance.Key, IV.GetBytes());
    }


    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional IV.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static byte[] EncryptAesGcm(this byte[] data, byte[] key, byte[] IV = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");

        if (IV != null && IV.Length != 16)
            throw new Exception("IV must be a length of 16 bytes for AES.");
        return null;
        //return Cryptation.EncryptAesGcm(data, key ?? CryptographyInstance.Key, IV);
    }

    /// <summary>
    /// Decrypts AES GCM bytes using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] DecryptAesGcm(this byte[] cipherBytes, string key = null)
    {
        return null;
        //return DecryptAesGcm(cipherBytes, key.GetBytes() ?? CryptographyInstance.Key);
    }

    /// <summary>
    /// Decrypts AES GCM bytes using the provided key.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] DecryptAesGcm(this byte[] cipherBytes, byte[] key)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");
        return null;
        //return Cryptation.DecryptAesGcm(cipherBytes, key ?? CryptographyInstance.Key);
    }
    #endregion

    #region RSA
    /// <summary>
    /// Encrypts using RSA with the public key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] EncryptRsa(this byte[] data, string publicKeyFullPath = null)
    {
        return null;
        //return Cryptation.EncryptRsa(data, publicKeyFullPath);
    }

    /// <summary>
    /// Decrypts using RSA with the private key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] DecryptRsa(this byte[] data, string publicKeyFullPath = null)
    {
        return null;
        //return Cryptation.DecryptRsa(data, publicKeyFullPath);
    }
    #endregion
}
