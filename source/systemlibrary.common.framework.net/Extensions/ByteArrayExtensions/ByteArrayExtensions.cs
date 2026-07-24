using System.Buffers.Text;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for byte arrays.
/// </summary>
public static partial class ByteArrayExtensions
{
    /// <summary>
    /// Returns a URL-safe base64 string — <c>+</c> and <c>/</c> are replaced with <c>-</c> and <c>_</c>, padding stripped.
    /// </summary>
    /// <remarks>
    /// Use <c>Obfuscate()</c> instead for data under ~400KB when base64 format is not required — it is faster.
    /// </remarks>
    /// <example>
    /// <code>
    /// var base64 = "hello world".GetBytes().ToBase64();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static string ToBase64(this byte[] bytes, bool urlSafe = false)
    {
        if (bytes == null) return null;

        if (bytes == null) return null;

        if (urlSafe)
            return Base64Url.EncodeToString(bytes);

        return Convert.ToBase64String(bytes);

        // Commented out: Cannot remember why I somehow rolled my own
        //int tmp = ((bytes.Length + 2) / 3) * 4;

        //char[] buffer = new char[tmp];
        //tmp = Convert.ToBase64CharArray(bytes, 0, bytes.Length, buffer, 0);

        //if (bytes.Length % 3 == 1) tmp -= 2;
        //else if (bytes.Length % 3 == 2) tmp -= 1;
        
        //for (int i = 0; i < tmp; i++)
        //{
        //    char c = buffer[i];
        //    if (c == '+') buffer[i] = '-';
        //    else if (c == '/') buffer[i] = '_';
        //}

        //return new string(buffer, 0, tmp);
    }

    /// <summary>
    /// Returns a base62 encoded string of the bytes.
    /// </summary>
    /// <example>
    /// <code>
    /// var base62 = "hello world".GetBytes().ToBase62();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static string ToBase62(this byte[] bytes)
    {
        if (bytes == null) return null;

        int len = bytes.Length * 2;

        var result = new char[len];

        int pos = 0;
        foreach (byte b in bytes)
        {
            result[pos++] = Base62Instance.Base62[b % 62];
            result[pos++] = Base62Instance.Base62[(b / 62) % 62];
        }

        return new string(result);
    }

    /// <summary>
    /// Returns the byte array decoded as a string. Defaults to UTF8.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "hello world".GetBytes().ToText(); // "hello world"
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static string ToText(this byte[] bytes, Encoding encoding = default)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return (encoding ?? Encoding.UTF8).GetString(bytes);
    }

    /// <summary>
    /// Returns a SHA1 hash string of the bytes.
    /// </summary>
    /// <remarks>
    /// Slower than <c>ToMD5Hash()</c> for data under ~200 bytes.
    /// </remarks>
    /// <example>
    /// <code>
    /// var hash = "hello world".GetBytes().ToSha1Hash();
    /// </code>
    /// </example>
    public static string ToSha1Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Sha1.Compute(bytes);
    }

    /// <summary>
    /// Returns a SHA256 hash string of the bytes.
    /// </summary>
    /// <example>
    /// <code>
    /// var hash = "hello world".GetBytes().ToSha256Hash();
    /// </code>
    /// </example>
    public static string ToSha256Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Sha256.Compute(bytes);
    }

    /// <summary>
    /// Returns an MD5 hash string of the bytes.
    /// </summary>
    /// <remarks>
    /// Faster than <c>ToSha1Hash()</c> for data under ~200 bytes.
    /// </remarks>
    /// <example>
    /// <code>
    /// var hash = "hello world".GetBytes().ToMD5Hash();
    /// </code>
    /// </example>
    public static string ToMD5Hash(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        return Md5.Compute(bytes);
    }

    /// <summary>
    /// GZip compresses the bytes and returns the result as a URL-safe base64 string.
    /// </summary>
    /// <example>
    /// <code>
    /// var compressed = myBytes.Compress();
    /// </code>
    /// </example>
    public static string Compress(this byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        using var input = new MemoryStream(bytes);
        using var output = new MemoryStream();
        using (var stream = new GZipStream(output, CompressionLevel.SmallestSize, true))
        {
            input.CopyTo(stream);
        }

        return output.ToArray().ToBase64();
    }

    /// <summary>
    /// GZip decompresses the bytes and returns the result as a string. Defaults to UTF8.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = compressedBytes.Decompress();
    /// </code>
    /// </example>
    public static string Decompress(this byte[] bytes, Encoding encoding = null)
    {
        if (bytes == null) return null;
        if (bytes.Length == 0) return "";

        using var output = new MemoryStream();
        using var input = new MemoryStream(bytes);

        using (var stream = new GZipStream(input, CompressionMode.Decompress))
        {
            stream.CopyTo(output);
        }

        var resultBytes = output.ToArray();

        return (encoding ?? Encoding.UTF8).GetString(resultBytes);
    }
}
