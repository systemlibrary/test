using System.Collections.Concurrent;
using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class Cryptography
{
    static ConcurrentDictionary<string, byte[]> AesGcmDecryptedCache = new ConcurrentDictionary<string, byte[]>();

    internal static byte[] EncryptAesGcm(byte[] bytes, byte[] key, byte[] nonce)
    {
        // TODO: implement a shard to boost throughput (benchmark disposal/creation of aesgcm)
        if (bytes.IsNot()) return bytes;

        if (nonce == null)
            nonce = Randomness.Bytes(12);

        key = GetPrimaryKey(key);

        var cipher = new byte[bytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(key, 16);

        aes.Encrypt(nonce, bytes, cipher, tag);

        var outBytes = new byte[nonce.Length + cipher.Length + tag.Length];
        Buffer.BlockCopy(nonce, 0, outBytes, 0, nonce.Length);
        Buffer.BlockCopy(cipher, 0, outBytes, nonce.Length, cipher.Length);
        Buffer.BlockCopy(tag, 0, outBytes, nonce.Length + cipher.Length, tag.Length);

        return outBytes;
    }

    internal static byte[] DecryptAesGcm(string cipherText64, byte[] key)
    {
        if (cipherText64.IsNot()) return cipherText64.GetBytes();

        string cacheKey = null;
        if (cipherText64.Length <= 512)
            cacheKey = cipherText64;

        if (cacheKey != null &&
         AesGcmDecryptedCache.TryGetValue(cacheKey, out var cached))
        {
            return cached.ToArray();
        }

        byte[] bytes = null;

        var cipherBytes = cipherText64.FromBase64ToBytes();

        if (key != null)
        {
            try
            {
                bytes = DecryptAesGcm(cipherBytes, key);
            }
            catch (Exception ex)
            {
                var message = GetExceptionMessage(cipherText64, key);

                throw new Exception(message, ex);
            }
        }

        if (bytes == null)
        {
            foreach (var tryKey in CryptographyInstance.DecryptKeys)
            {
                try
                {
                    bytes = DecryptAesGcm(cipherBytes, tryKey.Key);
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

        if (cacheKey != null && AesGcmDecryptedCache.Count < 16)
            AesGcmDecryptedCache.TryAdd(cacheKey, bytes);

        return bytes;
    }


    internal static byte[] DecryptAesGcm(byte[] data, byte[] key)
    {
        // TODO: implement a shard to boost throughput (benchmark disposal/creation of aesgcm)
        if (data.IsNot()) return data;

        if (data.Length < 28) // 12-byte nonce + 16-byte tag
            throw new Exception("Invalid AES-GCM payload.");

        var nonce = new byte[12];
        Buffer.BlockCopy(data, 0, nonce, 0, nonce.Length);

        var tag = new byte[16];
        Buffer.BlockCopy(data, data.Length - tag.Length, tag, 0, tag.Length);

        var cipherLen = data.Length - nonce.Length - tag.Length;
        var cipher = new byte[cipherLen];
        Buffer.BlockCopy(data, nonce.Length, cipher, 0, cipherLen);

        var plain = new byte[cipherLen];

        using var aes = new AesGcm(key, 16);

        aes.Decrypt(nonce, cipher, tag, plain);

        return plain;
    }
}
