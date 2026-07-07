using System.Text;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Returns the UTF8 byte representation of the string.
    /// </summary>
    public static byte[] GetBytes(this string text, Encoding encoding = default)
    {
        if (text == null) return null;

        if (encoding == null)
            return Encoding.UTF8.GetBytes(text);

        return encoding.GetBytes(text);
    }
}
