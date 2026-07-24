namespace SystemLibrary.Common.Framework.Extensions;

partial class ByteArrayExtensions
{
    #region AES CBC
    /// <summary>
    /// Encrypts using AES CBC with the framework's default key.
    /// </summary>
    public static byte[] Encrypt(this byte[] data)
    {
        return Encrypt(data, (byte[])null, null);
    }

    /// <summary>
    /// Encrypts using AES CBC with the provided key and optional IV.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes. IV must be 16 bytes if provided.
    /// </remarks>
    public static byte[] Encrypt(this byte[] data, string key, string IV = null)
    {
        return Encrypt(data, key.GetBytes(), IV.GetBytes());
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

        return Cryptation.EncryptAesCbc(data, key, IV);
    }

    /// <summary>
    /// Decrypts AES CBC bytes using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] Decrypt(this byte[] cipherBytes, string key = null)
    {
        return Decrypt(cipherBytes, key.GetBytes());
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
        
        return Cryptation.DecryptAesCbc(cipherBytes, key);
    }
    #endregion

    #region AES GCM
    /// <summary>
    /// Encrypts using AES GCM with the framework's default key.
    /// Returns [12 bytes nonce][ciphertext][16 bytes authentication tag]
    /// </summary>
    public static byte[] EncryptAesGcm(this byte[] data)
    {
        return EncryptAesGcm(data, (byte[])null);
    }

    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional nonce.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes, nonce must be 12 bytes if provided.
    /// Returns [12 bytes nonce][ciphertext][16 bytes authentication tag]
    /// </remarks>
    public static byte[] EncryptAesGcm(this byte[] data, string key, string nonce = null)
    {
        return EncryptAesGcm(data, key.GetBytes(), nonce.GetBytes());
    }


    /// <summary>
    /// Encrypts using AES GCM with the provided key and optional nonce.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes, nonce must be 12 bytes if provided.
    /// Returns [12 bytes nonce][ciphertext][16 bytes authentication tag]
    /// </remarks>
    public static byte[] EncryptAesGcm(this byte[] data, byte[] key, byte[] nonce = null)
    {
        if (key != null && key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new Exception("Key length must be 16, 24, or 32.");

        if (nonce != null && nonce.Length != 12)
            throw new Exception("Nonce must be a length of 12 bytes for AES-GCM.");
        
        return Cryptation.EncryptAesGcm(data, key, nonce);
    }

    /// <summary>
    /// Decrypts AES GCM bytes using the provided key, or the framework's default key if null.
    /// </summary>
    /// <remarks>
    /// Key must be 16, 24 or 32 bytes.
    /// </remarks>
    public static byte[] DecryptAesGcm(this byte[] cipherBytes, string key = null)
    {
        return DecryptAesGcm(cipherBytes, key.GetBytes());
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
        
        return Cryptation.DecryptAesGcm(cipherBytes, key);
    }
    #endregion

    #region RSA
    /// <summary>
    /// Encrypts using RSA with the public key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] EncryptRsa(this byte[] data, string publicKeyFullPath = null)
    {
        return Cryptation.EncryptRsa(data, publicKeyFullPath);
    }

    /// <summary>
    /// Decrypts using RSA with the private key file at the specified path, or the auto-discovered key if null.
    /// </summary>
    public static byte[] DecryptRsa(this byte[] data, string privateKeyFullPath = null)
    {
        return Cryptation.DecryptRsa(data, privateKeyFullPath);
    }
    #endregion
}
