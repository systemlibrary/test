using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for long integers.
/// </summary>
public static class LongExtensions
{
    /// <summary>
    /// Returns a base62 encoded string of the value.
    /// </summary>
    public static string ToBase62(this long number)
    {
        if (number == 0)
        {
            char[] baseEmpty = new char[1];

            baseEmpty[0] = Base62Instance.Base62[0];

            return new string(baseEmpty);
        }

        var len = (int)(Math.Log(number) / Math.Log(62)) + 1;

        int i = len;

        Span<char> buffer = stackalloc char[len];

        while (number > 0)
        {
            i--;
            buffer[i] = Base62Instance.Base62[(number % 62)];
            number /= 62;
        }
        return new string(buffer[i..]);
    }

    /// <summary>
    /// Returns a base62 encoded string of the value.
    /// </summary>
    public static string ToBase62(this ulong number)
    {
        if (number == 0)
        {
            char[] baseEmpty = new char[1];

            baseEmpty[0] = Base62Instance.Base62[0];

            return new string(baseEmpty);
        }

        var len = (int)(Math.Log(number) / Math.Log(62)) + 1;

        int i = len;

        Span<char> buffer = stackalloc char[len];

        while (number > 0)
        {
            i--;
            buffer[i] = Base62Instance.Base62[(number % 62)];
            number /= 62;
        }
        return new string(buffer[i..]);
    }

    /// <summary>
    /// Writes the base62 encoded value into a span buffer starting at <c>position</c>, returns the new position.
    /// </summary>
    public static int ToBase62(this ulong number, Span<char> buffer, int position)
    {
        if (number == 0) return position;

        while (number > 0)
        {
            buffer[position++] = Base62Instance.Base62[(number % 62)];
            number /= 62;
        }

        return position;
    }

    /// <summary>
    /// Returns the base62 encoded value as a char array with the encoded length via <c>out len</c>.
    /// </summary>
    public static char[] ToBase62(this ulong number, out int len)
    {
        // NOITE: Remove this as public?
        if (number == 0)
        {
            char[] baseEmpty = new char[1];

            baseEmpty[0] = Base62Instance.Base62[0];

            len = 1;

            return baseEmpty.ToArray();
        }

        len = (int)(Math.Log(number) / Math.Log(62)) + 1;

        Span<char> buffer = stackalloc char[len];

        int i = len;

        while (number > 0)
        {
            i--;
            buffer[i] = Base62Instance.Base62[(number % 62)];
            number /= 62;
        }

        return buffer.ToArray();
    }
}
