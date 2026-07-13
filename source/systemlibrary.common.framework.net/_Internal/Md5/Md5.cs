using System.Security.Cryptography;

namespace SystemLibrary.Common.Framework;

internal static class Md5
{
    internal static string Compute(byte[] bytes)
    {
        if (bytes == null) return null;

        if (bytes.Length == 0) return "";

        // TODO: Improve performance by sharded and recycle shard every 20-60 minutes
        using (var hasher = MD5.Create())
            return Convert.ToHexString(hasher.ComputeHash(bytes));
    }

    internal static string Compute(Stream stream)
    {
        if (stream == null) return null;

        if (!stream.CanRead) return null;

        using (var hasher = MD5.Create())
            return Convert.ToHexString(hasher.ComputeHash(stream));
    }

    internal static async Task<string> ComputeAsync(Stream stream)
    {
        if (stream == null) return null;

        if (!stream.CanRead) return null;

        using (var hasher = MD5.Create())
            return Convert.ToHexString(await hasher.ComputeHashAsync(stream).ConfigureAwait(false));
    }
}
