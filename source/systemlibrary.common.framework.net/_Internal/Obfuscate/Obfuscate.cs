using System.Runtime.CompilerServices;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal static class Obfuscator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    internal static string Obfuscate(string text, int salt)
    {
        if (salt == -1)
            salt = AppInstance.Salt;

        if (text == null) return null;

        if (text == "") return "";

        var bytes = text.GetBytes();

        var result = new byte[bytes.Length];

        int s = salt;

        for (int i = 0; i < bytes.Length; i++)
        {
            s ^= s << 3;
            s ^= s >> 7;
            s ^= i * 11;

            result[i] = unchecked((byte)(bytes[i] + (byte)s));
        }

        return result.ToBase64(urlSafe: true);
    }

    internal static string Deobfuscate(string text, int salt)
    {
        if (salt == -1)
            salt = AppInstance.Salt;

        if (text == null) return null;
        if (text == "") return "";

        var bytes = text.FromBase64ToBytes(true);
        var result = new byte[bytes.Length];

        int s = salt;

        for (int i = 0; i < bytes.Length; i++)
        {
            s ^= s << 3;
            s ^= s >> 7;
            s ^= i * 11;

            result[i] = unchecked((byte)(bytes[i] - (byte)s));
        }

        return result.ToText();
    }
}
