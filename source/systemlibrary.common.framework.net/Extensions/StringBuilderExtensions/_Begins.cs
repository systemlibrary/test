using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

partial class StringBuilderExtensions
{
    /// <summary>
    /// Returns true if the StringBuilder begins with the specified string.
    /// </summary>
    public static bool BeginsWith(this StringBuilder stringBuilder, string start, bool caseInsensitive = false)
    {
        if (stringBuilder == null || stringBuilder.Length == 0) return false;

        if (start == null || start == "") return false;

        var startLength = start.Length;

        if (startLength > stringBuilder.Length) return false;

        var startIndex = 0;
        var endIndex = startIndex + startLength;
        var j = 0;

        if (!caseInsensitive)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (stringBuilder[i] != start[j]) return false;
                j++;
            }
        }
        else
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (char.ToLowerInvariant(stringBuilder[i]) != char.ToLowerInvariant(start[j])) return false;
                j++;
            }
        }
        return true;
    }

}
