using System.Collections.Concurrent;
using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class Cryptation
{
    static ConcurrentDictionary<string, byte[]> AesGcmDecryptedShelf = new ConcurrentDictionary<string, byte[]>();

    internal static byte[] EncryptAesGcm(byte[] bytes, byte[] key, byte[] iv)
    {
        // TODO: implement a shard to boost throughput (benchmark disposal/creation of aesgcm)
        if (bytes.IsNot()) return bytes;

        if (iv == null)
            iv = Randomness.Bytes(12);

        var cipher = new byte[bytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(key, 16);

        aes.Encrypt(iv, bytes, cipher, tag);

        var outBytes = new byte[iv.Length + cipher.Length + tag.Length];
        Buffer.BlockCopy(iv, 0, outBytes, 0, iv.Length);
        Buffer.BlockCopy(cipher, 0, outBytes, iv.Length, cipher.Length);
        Buffer.BlockCopy(tag, 0, outBytes, iv.Length + cipher.Length, tag.Length);

        return outBytes;
    }

    internal static byte[] DecryptAesGcm(string cipherText64, byte[] key)
    {
        if (cipherText64.IsNot()) return cipherText64.GetBytes();

        string shelfKey = null;
        if (cipherText64.Length <= 255)
            shelfKey = cipherText64;

        if (shelfKey != null)
            if (AesGcmDecryptedShelf.ContainsKey(shelfKey))
                return AesGcmDecryptedShelf[shelfKey];

        byte[] bytes = null;
        try
        {
            bytes = DecryptAesGcm(cipherText64.FromBase64ToBytes(), key);
        }
        catch (Exception ex)
        {
            var message = GetExceptionMessage(cipherText64, key);

            throw new Exception(message, ex);
        }

        if (shelfKey != null && AesGcmDecryptedShelf.Count < 64)
            AesGcmDecryptedShelf.TryAdd(shelfKey, bytes);

        return bytes;
    }


    internal static byte[] DecryptAesGcm(byte[] data, byte[] key)
    {
        // TODO: implement a shard to boost throughput (benchmark disposal/creation of aesgcm)
        if (data.IsNot()) return data;

        var iv = new byte[12];
        Buffer.BlockCopy(data, 0, iv, 0, iv.Length);

        var tag = new byte[16];
        Buffer.BlockCopy(data, data.Length - tag.Length, tag, 0, tag.Length);

        var cipherLen = data.Length - iv.Length - tag.Length;
        var cipher = new byte[cipherLen];
        Buffer.BlockCopy(data, iv.Length, cipher, 0, cipherLen);

        var plain = new byte[cipherLen];

        using var aes = new AesGcm(key, 16);

        aes.Decrypt(iv, cipher, tag, plain);

        return plain;
    }
}
