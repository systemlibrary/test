using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

partial class StringBuilderExtensions
{
    /// <summary>
    /// Returns true if non-null and has content.
    /// </summary>
    public static bool Is(this StringBuilder stringBuilder)
    {
        return stringBuilder != null && stringBuilder.Length != 0;
    }

    /// <summary>
    /// Returns true if null or empty.
    /// </summary>
    public static bool IsNot(this StringBuilder stringBuilder)
    {
        return stringBuilder == null || stringBuilder.Length == 0;
    }
}
