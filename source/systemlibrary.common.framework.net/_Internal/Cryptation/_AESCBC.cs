using System.Collections.Concurrent;
using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class Cryptation
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

    static ConcurrentDictionary<string, byte[]> AesCbcDecryptedShelf = new ConcurrentDictionary<string, byte[]>();

    internal static byte[] DecryptAesCbc(string cipherText64, byte[] key)
    {
        if (cipherText64.IsNot()) return cipherText64.GetBytes();

        string shelfKey = null;
        if (cipherText64.Length <= 255)
            shelfKey = cipherText64;

        if (shelfKey != null)
            if (AesCbcDecryptedShelf.ContainsKey(shelfKey))
                return AesCbcDecryptedShelf[shelfKey];

        byte[] bytes = null;

        try
        {
            bytes = DecryptAesCbc(cipherText64.FromBase64ToBytes(), key);
        }
        catch (Exception ex)
        {
            var message = GetExceptionMessage(cipherText64, key);

            throw new Exception(message, ex);
        }

        if (shelfKey != null && AesCbcDecryptedShelf.Count < 64)
            AesCbcDecryptedShelf.TryAdd(shelfKey, bytes);

        return bytes;
    }

    internal static byte[] DecryptAesCbc(byte[] cipherData, byte[] key)
    {
        if (cipherData.IsNot()) return cipherData;

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
