using System.Buffers.Text;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
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
}
