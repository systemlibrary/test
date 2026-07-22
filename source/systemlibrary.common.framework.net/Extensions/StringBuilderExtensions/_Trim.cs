using System.Text;

namespace SystemLibrary.Common.Framework.Extensions;

partial class StringBuilderExtensions
{
    /// <summary>
    /// Removes the first matching value from the start. Defaults to trimming space and newlines if no values specified.
    /// Returns true if anything was removed.
    /// </summary>
    public static bool TrimStart(this StringBuilder stringBuilder, params string[] values)
    {
        if (values == null || values.Length == 0)
        {
            values = new string[] { " ", "\r\n", "\n", };
        }

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (stringBuilder.BeginsWith(value, false))
            {
                var length = value.Length;

                stringBuilder.Remove(0, length);

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes the first matching value from the end. Defaults to trimming space and newlines if no values specified.
    /// Returns true if anything was removed.
    /// </summary>
    public static bool TrimEnd(this StringBuilder stringBuilder, params string[] values)
    {
        if (values == null || values.Length == 0)
        {
            values = new string[] { " ", "\r\n", "\n", };
        }

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (stringBuilder.EndsWith(value, false))
            {
                var length = value.Length;

                stringBuilder.Remove(stringBuilder.Length - length, length);

                return true;
            }
        }
        return false;
    }

}
