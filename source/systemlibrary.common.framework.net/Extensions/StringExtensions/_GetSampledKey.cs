using System.Runtime.CompilerServices;

using SystemLibrary.Common.Framework.Extensions;


namespace SystemLibrary.Common.Framework;


partial class StringExtensions
{
    /// <summary>
    /// Returns a short sampled key of the input.
    /// </summary>
    /// <remarks>
    /// Output length varies by input size.
    /// - Input ≤ 11 chars: returns up to 11 chars, lossless base62
    /// - Input ≤ 32 chars: returns up to 13 chars
    /// - Input ≤ 64 chars: returns up to 15 chars
    /// - Input > 64 chars: returns up to 21 chars
    /// Inputs over 256 chars trade collision resistance for performance — the fingerprint starts skipping characters.
    /// </remarks>
    public static string GetSampledKey(this string input, string prefix = "")
    {
        if (input == null || input.Length == 0) return prefix;

        var l = input.Length;

        // Returns strlen 1-11 equal to input
        if (l <= 11) return prefix + GetSmallSampledKey(input);

        // Returns strlen usually 13, max 13
        if (l <= 32) return prefix + GetMediumSampledKey(input);

        // Returns strlen usually 14-15, max 15
        if (l <= 64) return prefix + GetLargeSampledKey(input);

        // Returns strlen usually 17-21, max 21
        return prefix + GetVeryLargeSampledKey(input);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static string GetSmallSampledKey(string input)
    {
        int l = input.Length;

        Span<char> buffer = stackalloc char[l];

        int p = 0;

        while (p < l)
        {
            buffer[p] = input[p].ToBase62();
            p++;
        }

        return new string(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static string GetMediumSampledKey(string input)
    {
        int l = input.Length;

        var fp = input.GetFingerprint();

        Span<char> buffer = stackalloc char[16];

        int p = 0;

        // 1 char as sampled output
        buffer[p++] = ((char)(input[0] + input[1] + input[3] + input[l - 2] + input[l - 1])).ToBase62();

        // Returns 1 char
        p = ((ulong)l).ToBase62(buffer, p);

        // Returns max 11 chars
        p = ((ulong)fp).ToBase62(buffer, p);

        return new string(buffer[..p]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static string GetLargeSampledKey(string input)
    {
        int l = input.Length;

        var fp = input.GetFingerprint();

        Span<char> buffer = stackalloc char[16];

        int p = 0;

        // 2 chars as sampled output
        buffer[p++] = ((char)(input[0] + input[1] + input[2] + input[6] + input[12])).ToBase62();
        buffer[p++] = ((char)(input[l - 1] + input[l - 2] + input[l - 3] + input[l - 6] + input[l - 12])).ToBase62();

        // Returns max 2 chars
        p = ((ulong)l).ToBase62(buffer, p);

        // Returns max 11 chars
        p = ((ulong)fp).ToBase62(buffer, p);

        return new string(buffer[..p]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static string GetVeryLargeSampledKey(string input)
    {
        int l = input.Length;

        ulong fp = input.GetFingerprint();

        // Maximum allowed key-len returned
        Span<char> buffer = stackalloc char[23];

        int p = 0;

        // 5 chars as sampled output
        buffer[p++] = ((char)(input[0] + input[1] + input[3] + input[6] + input[12])).ToBase62();
        buffer[p++] = ((char)(input[l - 1] + input[l - 2] + input[l - 3] + input[l - 6] + input[l - 12])).ToBase62();
        buffer[p++] = ((char)(input[l / 3] + input[l / 4] + input[l / 5])).ToBase62();
        buffer[p++] = (char)(input[l / 2]).ToBase62();
        buffer[p++] = (char)(input[3 * l / 4]).ToBase62();

        // Returns max 6 chars
        p = ((ulong)l).ToBase62(buffer, p);

        // Returns max 11 chars
        p = ((ulong)fp).ToBase62(buffer, p);

        return new string(buffer[..p]);
    }
}
