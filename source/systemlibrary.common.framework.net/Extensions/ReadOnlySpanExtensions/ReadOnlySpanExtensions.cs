using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for ReadOnlySpan.
/// </summary>
public static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Returns true if the span is non-null and has a length greater than zero.
    /// </summary>
    /// <example>
    /// <code>
    /// "Hello World".AsSpan().Is(); // true
    /// </code>
    /// </example>
    public static bool Is<T>(this ReadOnlySpan<T> span)
    {
        return span != null && span.Length != 0;
    }

    /// <summary>
    /// Returns true if the span is null or empty.
    /// </summary>
    /// <example>
    /// <code>
    /// "Hello World".AsSpan().IsNot(); // false
    /// </code>
    /// </example>
    public static bool IsNot<T>(this ReadOnlySpan<T> span)
    {
        return span == null || span.Length == 0;
    }

    /// <summary>
    /// Returns a base64 encoded string of the span. Defaults to UTF8 encoding.
    /// </summary>
    /// <example>
    /// <code>
    /// var base64 = "Hello World".AsSpan().ToBase64();
    /// </code>
    /// </example>
    public static string ToBase64(this ReadOnlySpan<char> span, Encoding encoding = default, bool urlSafe = false)
    {
        if (span == null) return default;
        
        if (encoding == default)
            encoding = Encoding.UTF8;

        var len = encoding.GetByteCount(span);

        Span<byte> bytes = new byte[len];

        encoding.GetBytes(span, bytes);

        return bytes.ToArray().ToBase64(urlSafe);
    }


    /// <summary>
    /// Returns the span encoded as a byte array. Defaults to UTF8 encoding.
    /// </summary>
    /// <example>
    /// <code>
    /// var bytes = "Hello World".AsSpan().GetBytes();
    /// </code>
    /// </example>
    public static byte[] GetBytes(this ReadOnlySpan<char> span, Encoding encoding = default)
    {
        if (span == null) return default;

        if (encoding == default)
        {
            encoding = Encoding.UTF8;
        }

        var len = encoding.GetByteCount(span);

        Span<byte> bytes = new byte[len];

        encoding.GetBytes(span, bytes);

        return bytes.ToArray();
    }
}