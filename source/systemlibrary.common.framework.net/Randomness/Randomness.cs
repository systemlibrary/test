using System.Security.Cryptography;
using System.Text;

using SystemLibrary.Common.Framework.Bootstrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Static class for generating random values.
/// </summary>
public static partial class Randomness
{
    static char[] Chars;

    static int CharsLength;

    static uint GlobalSeed;

    static int SentencesLength;
    static Dictionary<int, string[]> SentenceParts;

    static Randomness()
    {
        Boot.Strap();

        GlobalSeed = (uint)(DateTime.UtcNow.Ticks & 0xFFFFFFFF);

        Chars = Base62Instance.Base62;

        CharsLength = Base62Instance.Base62.Length;

        var sentences = GetSentences();
        var swaps = GetSentenceSwaps();

        var sentenceVariations = new List<string>();

        foreach (var sentence in sentences)
        {
            foreach (var swap in swaps)
            {
                int i = 0;

                var replace = " " + swap[i] + " ";
                if (sentence.Contains(replace))
                {
                    for (i = 1; i < swap.Length; i++)
                    {
                        var replacement = swap[i];

                        var sentenceVariation = sentence.Replace(replace, " " + replacement + " ");
                        sentenceVariations.Add(sentenceVariation);
                    }
                }
            }
        }
        sentences.AddRange(sentenceVariations);

        SentenceParts = new Dictionary<int, string[]>();

        var sentenceIndex = 0;
        foreach (var sentence in sentences)
        {
            var parts = sentence.Split(',').Select(p => p.Trim()).ToArray();

            for (int i = 1; i < parts.Length; i++)
            {
                parts[i] = ", " + parts[i].Trim();
            }

            SentenceParts[sentenceIndex] = parts.ToArray();
            sentenceIndex++;
        }
        SentencesLength = SentenceParts.Count - 1;
    }

    /// <summary>
    /// Returns a random integer between 0 and <c>maxValue</c> inclusive.
    /// </summary>
    public static int Int(int maxValue = 1000)
    {
        if (maxValue != int.MaxValue)
            return Random.Shared.Next(0, maxValue + 1);

        return Random.Shared.Next(0, maxValue);
    }

    /// <summary>
    /// Returns a random integer between <c>minValue</c> and <c>maxValue</c> inclusive.
    /// </summary>
    public static int Int(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue + 1);
    }

    /// <summary>
    /// Returns a random double between <c>minValue</c> and <c>maxValue</c>.
    /// </summary>
    public static double Double(double minValue = 1, double maxValue = 9999)
    {
        double randomValue = Random.Shared.NextDouble();

        return minValue + (randomValue * (maxValue - minValue));
    }

    /// <summary>
    /// Returns a byte array of the specified length filled with random values.
    /// </summary>
    public static byte[] Bytes(int length = 16)
    {
        return RandomNumberGenerator.GetBytes(length);
    }

    /// <summary>
    /// Returns a random base62 string of the specified length.
    /// </summary>
    /// <remarks>
    /// In tight loops exceeding 10K invocations, consider an ArrayPool implementation for lower memory overhead.
    /// </remarks>
    public static string String(int min = 6, int max = 6)
    {
        const int blockSize = 6144;

        if (min > max)
            max = min;

        var length = Randomness.Int(min, max);

        byte[] bytes;

        if (length <= blockSize)
        {
            var buffer = new char[length];

            bytes = new byte[length];

            Random.Shared.NextBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                buffer[i] = Chars[bytes[i] % CharsLength];
            }

            return new string(buffer);
        }
        
        char[] block = new char[blockSize];

        bytes = new byte[blockSize];

        Random.Shared.NextBytes(bytes);

        for (int i = 0; i < blockSize; i++)
            block[i] = Chars[bytes[i] % CharsLength];

        var sb = new StringBuilder(length);
        for (int i = 0; i < length / blockSize; i++)
            sb.Append(block);

        int remainder = length % blockSize;
        if (remainder > 0)
            sb.Append(block, 0, remainder);

        return sb.ToString();
    }

    /// <summary>
    /// Returns a random paragraph of roughly 200 characters. Pass <c>fixedSeed</c> to always return the same text.
    /// </summary>
    public static string Words(int minLen = 220, int maxLen = 220, int fixedSeed = -1)
    {
        var sb = new System.Text.StringBuilder(maxLen);

        return Words(sb, minLen, maxLen, fixedSeed).ToString();
    }

    /// <summary>
    /// Fills a <c>StringBuilder</c> with random paragraph text and returns it. Pass <c>fixedSeed</c> to always return the same text.
    /// </summary>
    public static StringBuilder Words(StringBuilder sb, int minLen = 220, int maxLen = 220, int fixedSeed = -1)
    {
        sb.Clear();

        int target = Random.Shared.Next(minLen, maxLen + 1);

        int i = 0;
        int sentenceIndex = 0;
        int partIndex = 0;
        string[] parts = null;

        if (fixedSeed >= 0)
        {
            sentenceIndex = fixedSeed % SentencesLength;
            parts = SentenceParts[sentenceIndex];
            partIndex = fixedSeed % parts.Length;
            if (partIndex == 0)
                partIndex = 1;
        }

        var partsForward = true;

        while (sb.Length < target)
        {
            if (fixedSeed == -1)
                sentenceIndex = Randomness.Int(0, SentencesLength);

            parts = SentenceParts[sentenceIndex];

            sb.Append(parts[0]);

            if (sb.Length < target)
            {
                if (fixedSeed == -1)
                    partIndex = Randomness.Int(0, parts.Length - 1);

                if (partIndex > 0)
                {
                    if (partsForward)
                    {
                        for (i = 1; i <= partIndex; i++)
                        {
                            sb.Append(parts[i]);
                            if (sb.Length > target)
                                break;
                        }
                        partsForward = false;
                    }
                    else
                    {
                        for (i = partIndex; i >= 1; i--)
                        {
                            sb.Append(parts[i]);
                            if (sb.Length > target)
                                break;
                        }
                        partsForward = true;
                    }
                }
            }

            if (sb[sb.Length - 1] != '.')
                sb.Append(". ");
        }

        if (sb.Length > target)
            sb.Length = target;

        if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
        {
            sb.TrimEnd();
            sb.Append(".");
        }

        return sb;
    }
}
