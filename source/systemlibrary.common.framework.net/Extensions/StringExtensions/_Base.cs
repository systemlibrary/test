using System.Buffers.Text;
using System.Text;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    #region Base64
    /// <summary>
    /// Returns a URL-safe base64 encoded string. Defaults to UTF8 encoding.
    /// </summary>
    /// <remarks>
    /// Use <c>Obfuscate()</c> instead for data under ~400KB when base64 format is not required — it is faster.
    /// </remarks>
    /// <example>
    /// <code>
    /// var base64 = "Hello world".ToBase64();
    /// </code>
    /// </example>
    public static string ToBase64(this string text, Encoding encoding = default, bool urlSafe = false)
    {
        return GetBytes(text, encoding).ToBase64(urlSafe);
    }

    /// <summary>
    /// Decodes a base64 string back to a string. 
    /// Optionally a URL-safe base64 string back to a string.
    /// Defaults to UTF8 encoding.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "Hello world".ToBase64().FromBase64(); // "Hello world"
    /// </code>
    /// </example>
    public static string FromBase64(this string base64String, Encoding encoding = default, bool urlSafe = false)
    {
        // NOTE: Earlier versions had its own url safe implementation for reasons I cannot remember (perf?)

        if (base64String == null) return null;

        var bytes = FromBase64ToBytes(base64String, urlSafe);

        if (encoding == default)
            encoding = Encoding.UTF8;

        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Decodes a base64 string back to bytes. 
    /// Optionally a URL-safe base64 string back bytes.
    /// </summary>
    public static byte[] FromBase64ToBytes(this string base64String, bool urlSafe = false)
    {
        // NOTE: Earlier versions had its own url safe implementation for reasons I cannot remember (perf?)
        if (base64String == null) return null;

        var bytes = urlSafe ?
            Base64Url.DecodeFromChars(base64String)
            : Convert.FromBase64String(base64String);

        return bytes;
    }

    #endregion

    #region Base62
    /// <summary>
    /// Returns a base62 encoded string. Defaults to UTF8 encoding.
    /// </summary>
    public static string ToBase62(this string text, Encoding encoding = default)
    {
        return GetBytes(text, encoding).ToBase62();
    }

    /// <summary>
    /// Decodes a base62 string back to a string. Defaults to UTF8 encoding.
    /// </summary>
    public static string FromBase62(this string base62, Encoding encoding = default)
    {
        return base62.FromBase62ToBytes().ToText(encoding);
    }

    /// <summary>
    /// Decodes a base62 string to a byte array.
    /// </summary>
    /// <remarks>
    /// Throws <c>ArgumentException</c> if the string length is odd or contains invalid base62 characters.
    /// </remarks>
    public static byte[] FromBase62ToBytes(this string base62)
    {
        if (base62 == null) return null;

        if (base62.Length % 2 != 0) throw new ArgumentException("Invalid Base62 string length");

        int len = base62.Length / 2;

        var result = new byte[len];

        for (int i = 0, j = 0; i < base62.Length; i += 2, j++)
        {
            int first = Array.IndexOf(Base62Instance.Base62, base62[i]);
            int second = Array.IndexOf(Base62Instance.Base62, base62[i + 1]);

            if (first < 0 || second < 0)
                throw new ArgumentException("Invalid character in Base62 string: " + base62[i] + " or " + base62[i + 1]);

            result[j] = (byte)(first + second * 62);
        }

        return result;
    }
    #endregion
}