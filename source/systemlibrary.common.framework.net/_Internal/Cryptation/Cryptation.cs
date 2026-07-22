using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal static partial class Cryptation
{
    static byte[] GetPrimaryKey(byte[] key)
    {
        if (key?.Length > 0) return key;

        return CryptographyInstance
            .Keys
            .First(x => !x.Source.StartsWith("RSA") && x.Key != null)
            .Key;
    }
}