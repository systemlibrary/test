using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for integers.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Returns the value clamped between <c>min</c> and <c>max</c>. Defaults to 0–9999.
    /// </summary>
    /// <example>
    /// <code>
    /// (-1).Clamp();       // 0
    /// 10000.Clamp();      // 9999
    /// 5.Clamp(1, 10);     // 5
    /// </code>
    /// </example>
    public static int Clamp(this int value, int min = 0, int max = 9999)
    {
        if (value < min) return min;

        if (value > max) return max;

        return value;
    }

    /// <summary>
    /// Returns a base62 encoded string of the integer.
    /// </summary>
    public static string ToBase62(this uint number)
    {
        var i = 6;
        Span<char> buffer = stackalloc char[6];

        while (number > 0 && i > 0)
        {
            buffer[--i] = Base62Instance.Base62[number % 62];
            number /= 62;
        }
        return new string(buffer[i..]);
    }

    /// <summary>
    /// Returns a base62 encoded string of the integer.
    /// </summary>
    public static string ToBase62(this int number)
    {
        var i = 6;
        Span<char> buffer = stackalloc char[6];

        while (number > 0 && i > 0)
        {
            buffer[--i] = Base62Instance.Base62[number % 62];
            number /= 62;
        }
        return new string(buffer[i..]);
    }
}
