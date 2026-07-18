namespace SystemLibrary.Common.Framework.Boostrap;

internal static class Base62Instance
{
    /// <summary>
    /// Precomputed character mapping used only in char-extensions.
    /// Maps UTF-16 characters to the Base62 alphabet for fast conversion.
    /// This is not encoding and cannot be reversed.
    /// </summary>
    internal static char[] Base62CharacterMap;

    /// <summary>
    /// Use this always for (number % 62) calculations
    /// </summary>
    internal static char[] Base62;

    static Base62Instance()
    {
        Boot.Strap();
    }
}
