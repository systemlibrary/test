using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

partial class StringBuilderExtensions
{/// <summary>
 /// Returns true if the StringBuilder ends with the specified string.
 /// </summary>
    public static bool EndsWith(this StringBuilder stringBuilder, string ending, bool caseInsensitive = false)
    {
        if (stringBuilder == null || stringBuilder.Length == 0) return false;

        if (ending == null || ending == "") return false;

        var endingLength = ending.Length;

        if (endingLength > stringBuilder.Length) return false;

        var startIndex = stringBuilder.Length - endingLength;
        var endIndex = startIndex + endingLength;
        var j = 0;

        if (!caseInsensitive)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (stringBuilder[i] != ending[j]) return false;
                j++;
            }
        }
        else
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                if (char.ToLowerInvariant(stringBuilder[i]) != char.ToLowerInvariant(ending[j])) return false;
                j++;
            }
        }
        return true;
    }
}
