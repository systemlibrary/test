
namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for streams.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Returns an MD5 hash of the stream contents, or null if unreadable.
    /// </summary>
    /// <remarks>
    /// Slower than <c>ToSha1Hash()</c> for data over ~200 bytes.
    /// </remarks>
    public static async Task<string> ToMD5HashAsync(this Stream stream)
    {
        return await Md5.ComputeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns an MD5 hash of the stream contents asynchronously, or null if unreadable.
    /// </summary>
    /// <remarks>
    /// Slower than <c>ToSha1HashAsync()</c> for data over ~200 bytes.
    /// </remarks>
    public static string ToMD5Hash(this Stream stream)
    {
        return Md5.Compute(stream);
    }

    /// <summary>
    /// Returns a SHA1 hash of the stream contents, or null if unreadable.
    /// </summary>
    /// <remarks>
    /// Slower than <c>ToMD5Hash()</c> for data under ~200 bytes.
    /// </remarks>
    public static string ToSha1Hash(this Stream stream)
    {
        return Sha1.Compute(stream);
    }

    /// <summary>
    /// Returns a SHA1 hash of the stream contents asynchronously, or null if unreadable.
    /// </summary>
    /// <remarks>
    /// Slower than <c>ToMD5HashAsync()</c> for data under ~200 bytes.
    /// </remarks>
    public static async Task<string> ToSha1HashAsync(this Stream stream)
    {
        return await Sha1.ComputeAsync(stream).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a SHA256 hash of the stream contents, or null if unreadable.
    /// </summary>
    public static string ToSha256Hash(this Stream stream)
    {
        return Sha256.Compute(stream);
    }

    /// <summary>
    /// Returns a SHA256 hash of the stream contents asynchronously, or null if unreadable.
    /// </summary>
    public static async Task<string> ToSha256HashAsync(this Stream stream)
    {
        return await Sha256.ComputeAsync(stream).ConfigureAwait(false);
    }
}
