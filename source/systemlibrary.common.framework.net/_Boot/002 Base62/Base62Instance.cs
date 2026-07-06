namespace SystemLibrary.Common.Framework.Boostrap;

internal static class Base62Instance
{
    /// <summary>
    /// Only to be used in char extensions
    /// Converting a char to base62 char through a pre-mapped array for chars < 383
    /// </summary>
    internal static char[] Base62Map;

    /// <summary>
    /// Use this always for (number % 62) calculations
    /// </summary>
    internal static char[] Base62;

    static Base62Instance()
    {
        Boot.Strap();
    }
}
