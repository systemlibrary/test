using System.Collections.Concurrent;
using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class Cryptography
{
    internal static byte[] EncryptAesCbc(byte[] bytes, byte[] key, byte[] iv)
    {
        if (bytes.IsNot()) return bytes;

        if (iv == null)
            iv = Randomness.Bytes(16);

        using Aes aes = Aes.Create();

        var primaryKey = GetPrimaryKey(key);
        aes.Key = primaryKey;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        using var memory = new MemoryStream();
        memory.Write(iv, 0, iv.Length);

        using var cryptoStream = new CryptoStream(memory, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(bytes, 0, bytes.Length);
        cryptoStream.FlushFinalBlock();

        return memory.ToArray();
    }

    static ConcurrentDictionary<string, byte[]> AesCbcDecryptedCache = new ConcurrentDictionary<string, byte[]>();

    internal static byte[] DecryptAesCbc(string cipherText64, byte[] key)
    {
        if (cipherText64.IsNot()) return cipherText64.GetBytes();

        string cacheKey = null;
        if (cipherText64.Length <= 512)
            cacheKey = cipherText64;

        if (cacheKey != null &&
            AesCbcDecryptedCache.TryGetValue(cacheKey, out var cached))
        {
            return cached.ToArray();
        }

        byte[] bytes = null;

        if (key != null)
        {
            try
            {
                bytes = DecryptAesCbc(cipherText64.FromBase64ToBytes(), key);
            }
            catch (Exception ex)
            {
                var message = GetExceptionMessage(cipherText64, key);

                throw new Exception(message, ex);
            }
        }

        if (bytes == null)
        {
            var cipherBytes = cipherText64.FromBase64ToBytes();
            foreach (var tryKey in CryptographyInstance.DecryptKeys)
            {
                try
                {
                    bytes = DecryptAesCbc(cipherBytes, tryKey.Key);
                    break;
                }
                catch
                {
                    // swallow, continue
                }
            }

            if (bytes == null)
                throw new Exception("Could not decrypt cipher text starting with " + cipherText64.MaxLength(3));
        }

        if (cacheKey != null && AesCbcDecryptedCache.Count < 16)
        {
            AesCbcDecryptedCache.TryAdd(cacheKey, bytes);
        }


        return bytes;
    }

    internal static byte[] DecryptAesCbc(byte[] cipherData, byte[] key)
    {
        // Slice first 16 bytes of data as IV
        var iv = cipherData[..16];
        var data = cipherData[16..];

        // TODO: Create a sharded Aes and encryptor
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var memory = new MemoryStream(data);
        using var cryptoStream = new CryptoStream(memory, decryptor, CryptoStreamMode.Read);

        using var plain = new MemoryStream();
        cryptoStream.CopyTo(plain);

        return plain.ToArray();
    }

    static string GetExceptionMessage(string cipherText, byte[] key)
    {
        var suffix = "";

        foreach (var tryKey in CryptographyInstance.Keys)
        {
            if (tryKey.Key == key)
            {
                suffix = $"Tried decrypting with key: {tryKey.Source}:{tryKey.KeyStart}...";
            }
        }

        return $"Could not decrypt value starting with: {cipherText.MaxLength(4)}. " + suffix;
    }
}
