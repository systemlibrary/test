using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static partial class Cryptation
{
    const int ShardCount = 4;
    const int ShardEnd = 3;

    static ShardedDictionary<string, RSA[]> RSACachePool = null;

    static object RSACachePoolLock = new object();

    static object[] RSAEncryptLocks;
    static object[] RSADecryptLocks;

    static string DefaultEncryptKeyFileFullPath;
    static string DefaultDecryptKeyFileFullPath;

    static void TryAddToCache(string keyFileFullPath)
    {
        if (keyFileFullPath.IsNot()) return;

        if (RSACachePool.ContainsKey(keyFileFullPath)) return;

        lock (RSACachePoolLock)
        {
            if (RSACachePool.ContainsKey(keyFileFullPath)) return;

            var keyFileContent = File.ReadAllText(keyFileFullPath);

            var isPriv = keyFileFullPath.EndsWith(".priv", StringComparison.OrdinalIgnoreCase);
            var isPrivateKey = IsPrivateKey(keyFileContent);

            if (!isPriv && isPrivateKey)
            {
                throw new InvalidOperationException("EncryptRsa requires a public key.");
            }

            if (isPriv && !isPrivateKey)
            {
                throw new InvalidOperationException("DecryptRsa requires a private key.");
            }

            RSA[] pool = new RSA[ShardCount];

            for (int i = 0; i < ShardCount; i++)
            {
                pool[i] = RSA.Create(4096);
                pool[i].ImportFromPem(keyFileContent);
            }

            RSACachePool.Add(keyFileFullPath, pool);
        }
    }

    static bool IsPrivateKey(string pem)
    {
        return pem.Contains("PRIVATE KEY", StringComparison.OrdinalIgnoreCase) &&
               !pem.Contains("PUBLIC KEY", StringComparison.OrdinalIgnoreCase);
    }

    static void Initialize()
    {
        if (RSACachePool != null)
        {
            return;
        }

        lock (RSACachePoolLock)
        {
            if (RSACachePool != null)
            {
                return;
            }

            RSACachePool = new();

            if (CryptographyInstance.EncryptionKeyDirectory.IsNot())
            {
                throw new Exception("[Encryption] you have not specified a encryption key directory, nor could we auto detect a parent folder as the key directory. Please set the setting in appsettings: systemLibraryCommonFramework:cryptography:encryptionKeyDirectory");
            }

            RSAEncryptLocks = Enumerable.Range(0, ShardCount).Select(_ => new object()).ToArray();
            RSADecryptLocks = Enumerable.Range(0, ShardCount).Select(_ => new object()).ToArray();

            var dir = CryptographyInstance.EncryptionKeyDirectory;

            var pubFileFullPath = ""; // CryptographyInstance.EncryptionRsaPubFileFullPath;
            var privFileFullPath = "";// CryptographyInstance.EncryptionRsaPrivFileFullPath;

            // Creating a default pub and priv key files in the specified directory
            if (pubFileFullPath.IsNot() && privFileFullPath.IsNot())
            {
                FrameworkLog.Debug("[Cryptography] auto-generating RSA key files in " + dir);

                Directory.CreateDirectory(dir);

                pubFileFullPath = Path.Combine(dir, "pem.pub");
                privFileFullPath = Path.Combine(dir, "pem.priv");

                using (var rsa = RSA.Create(4096))
                {
                    File.WriteAllText(pubFileFullPath, rsa.ExportRSAPublicKeyPem());
                    File.WriteAllText(privFileFullPath, rsa.ExportRSAPrivateKeyPem());
                }
            }

            TryAddToCache(pubFileFullPath);
            DefaultEncryptKeyFileFullPath = pubFileFullPath;

            TryAddToCache(privFileFullPath);
            DefaultDecryptKeyFileFullPath = privFileFullPath;
        }
    }

    public static byte[] EncryptRsa(string text, string publicKeyFullPath = null)
    {
        return EncryptRsa(text.GetBytes(), publicKeyFullPath);
    }

    public static byte[] EncryptRsa(byte[] bytes, string publicKeyFullPath = null)
    {
        if (bytes.IsNot()) return bytes;

        Initialize();

        TryAddToCache(publicKeyFullPath);

        if (publicKeyFullPath.IsNot())
            publicKeyFullPath = DefaultEncryptKeyFileFullPath;

        var index = (bytes[0] + bytes.Length) & ShardEnd;

        var pool = RSACachePool[publicKeyFullPath];

        lock (RSAEncryptLocks[index])
        {
            return pool[index].Encrypt(bytes, RSAEncryptionPadding.OaepSHA256);
        }
    }

    public static byte[] DecryptRsa(byte[] data, string privateKeyFullPath = null)
    {
        if (data == null) return null;

        if (data.Length == 0) return data;

        Initialize();

        TryAddToCache(privateKeyFullPath);

        if (privateKeyFullPath.IsNot())
            privateKeyFullPath = DefaultDecryptKeyFileFullPath;

        var index = (data[0] + data.Length) & ShardEnd;

        var pool = RSACachePool[privateKeyFullPath];

        lock (RSADecryptLocks[index])
        {
            return pool[index].Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }
}