using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

partial class StringBuilderExtensions
{ /// <summary>
  /// Returns the index of the first occurrence of <c>text</c>, or -1 if not found.
  /// </summary>
    public static int IndexOf(this StringBuilder stringBuilder, string text, bool ignoreCase = false, int start = 0)
    {
        //Creds: https://stackoverflow.com/questions/1359948/why-doesnt-stringbuilder-have-indexof-method
        if (stringBuilder == null) return -1;

        if (text == null) return -1;

        int index;
        int length = text.Length;

        if (length > stringBuilder.Length) return -1;

        int maxSearchLength = (stringBuilder.Length - length) + 1;

        if (ignoreCase)
        {
            var textLowered = text.ToLower();
            for (int i = start; i < maxSearchLength; ++i)
            {
                if (Char.ToLower(stringBuilder[i]) == textLowered[0])
                {
                    index = 1;
                    while ((index < length) && (Char.ToLower(stringBuilder[i + index]) == textLowered[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        for (int i = start; i < maxSearchLength; ++i)
        {
            if (stringBuilder[i] == text[0])
            {
                index = 1;
                while ((index < length) && (stringBuilder[i + index] == text[index]))
                    ++index;

                if (index == length)
                    return i;
            }
        }

        return -1;
    }
}
