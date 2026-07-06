namespace SystemLibrary.Common.Framework.Boostrap;

internal static class Base62Boot
{
    internal static void Strap() { }

    static Base62Boot()
    {
        // We do not start with digits to obfuscate output
        var base62_order = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToArray();

        var map = new char[383];

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
        Base62Instance.Base62Map = map;
    }
}

