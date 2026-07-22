using System.Runtime.CompilerServices;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for characters.
/// </summary>
public static class CharExtensions
{
    /// <summary>
    /// Returns the base62 character representation of this character.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToBase62(this char c)
    {
        if (c < 383) return Base62Instance.Base62CharacterMap[c];

        return Base62Instance.Base62[c % 62];
    }
}
