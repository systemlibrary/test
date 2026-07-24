using System.Security.Cryptography;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

internal static partial class Cryptography
{
    static byte[] GetPrimaryKey(byte[] key)
    {
        if (key?.Length > 0) return key;

        return CryptographyInstance.PrimaryKey.Key;
    }
}