using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static partial class Cryptography
{
    const int ShardCount = 4;
    const int ShardEnd = 3;

    static ShardedDictionary<string, RSA[]> RSACachePool = null;

    static object RSACachePoolLock = new object();

    static object[] RSAEncryptLocks;
    static object[] RSADecryptLocks;

    static void TryAddToPool(string keyFileFullPath)
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

            if (CryptographyInstance.KeyDirectory.IsNot())
            {
                throw new Exception("[Encryption] you have not specified a encryption key directory, nor could we auto detect a parent folder as the key directory. Please set the setting in appsettings: systemLibraryCommonFramework:cryptography:keyDirectory");
            }

            RSAEncryptLocks = Enumerable.Range(0, ShardCount).Select(_ => new object()).ToArray();
            RSADecryptLocks = Enumerable.Range(0, ShardCount).Select(_ => new object()).ToArray();

            var dir = CryptographyInstance.KeyDirectory;

            if (CryptographyInstance.RsaKeys.Length == 0)
            {
                // Creating a default pub and priv key files in the specified directory
                FrameworkLog.Debug("[Cryptography] auto-generating RSA key files in " + dir);

                if(!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var pubFileFullPath = Path.Combine(dir, "pem.pub");
                var privFileFullPath = Path.Combine(dir, "pem.priv");

                using (var rsa = RSA.Create(4096))
                {
                    File.WriteAllText(pubFileFullPath, rsa.ExportRSAPublicKeyPem());
                    File.WriteAllText(privFileFullPath, rsa.ExportRSAPrivateKeyPem());
                }

                CryptographyInstance.RsaKeys = CryptographyInstance.RsaKeys.Add(KeyData.Create("RSA" + Path.GetExtension(privFileFullPath).Replace(".", "").ToUpper(), null, privFileFullPath));
                CryptographyInstance.RsaKeys = CryptographyInstance.RsaKeys.Add(KeyData.Create("RSA" + Path.GetExtension(pubFileFullPath).Replace(".", "").ToUpper(), null, pubFileFullPath));
            }

            foreach (var rsaKeyFile in CryptographyInstance.RsaKeys)
            {
                TryAddToPool(rsaKeyFile.FilePath);
            }
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

        if (publicKeyFullPath.IsNot())
            publicKeyFullPath = CryptographyInstance.RsaKeys.First(x => x.Source == "RSAPUB").FilePath;

        TryAddToPool(publicKeyFullPath);

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

        if (privateKeyFullPath.IsNot())
            privateKeyFullPath = CryptographyInstance.RsaKeys.First(x => x.Source == "RSAPRIV").FilePath;

        TryAddToPool(privateKeyFullPath);

        var index = (data[0] + data.Length) & ShardEnd;

        var pool = RSACachePool[privateKeyFullPath];

        lock (RSADecryptLocks[index])
        {
            return pool[index].Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }
}