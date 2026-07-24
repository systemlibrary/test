namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class Base62Boot
{
    internal static void Strap() { }

    static Base62Boot()
    {
        // Prevents common ASCII characters from mapping to themselves by starting with capital letters instead of digits
        var base62_order = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToArray();

        int latin_extended_a_max = 383;
        var map = new char[latin_extended_a_max];

        // Map ASCII properly, and keep reusing Index for remaining char conversions
        var index = 0;
        for (int i = 0; i < map.Length; i++)
            map[i] = '?';

        for (int c = 48; c <= 57; c++)
            map[c] = base62_order[index++];

        for (int c = 65; c <= 90; c++)
            map[c] = base62_order[index++];

        for (int c = 97; c <= 122; c++)
            map[c] = base62_order[index++];

        index = 0;
        for (int c = 0; c < map.Length; c++)
        {
            if (map[c] != '?') continue;

            map[c] = base62_order[index % 62];

            index++;
        }

        Base62Instance.Base62 = base62_order;
        Base62Instance.Base62CharacterMap = map;
    }
}

