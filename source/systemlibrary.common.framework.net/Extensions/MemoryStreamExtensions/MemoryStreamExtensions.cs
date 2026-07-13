namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for MemoryStream.
/// </summary>
public static class MemoryStreamExtensions
{
    /// <summary>
    /// Returns a base64 encoded string of the stream contents.
    /// </summary>
    public static string ToBase64(this MemoryStream memoryStream)
    {
        if (memoryStream == null) return null;

        if (memoryStream.Length == 0) return "";

        return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
    }
}
