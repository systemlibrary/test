using System.Numerics;
using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    const int FingerprintUnrollCount = 5;
    const int FingerprintUnrollCountSubOne = FingerprintUnrollCount - 1;
    const int FingerprintUnrollCountAddOne = FingerprintUnrollCount + 1;

    /// <summary>
    /// Returns a 64-bit fingerprint hash of the contents. Returns 0 for null or empty input.
    /// </summary>
    /// <remarks>
    /// Skips characters on inputs exceeding 256 chars for performance — increases collision probability on large inputs.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static ulong GetFingerprint(this string input)
    {
        if (input == null || input.Length == 0) return 0;

        return GetFingerprintWithOffset(input, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    static ulong GetFingerprintWithOffset(string input, int offset)
    {
        int l = input.Length;

        ulong fp = (ulong)input[0]
            + ((ulong)input[l / 2] << 16)
            + ((ulong)input[l - 1] << 32)
            ^ (ulong)l << 48;

        if (l < offset) return fp;

        // multiply with a huge prime (cred to xxHash)
        fp *= 0x9E3779B185EBCA87UL;

        int final = l - offset;

        if (final < 1)
            final = l;

        int step = final >> 8;

        if (step <= 1)
            step = 1 + FingerprintUnrollCountSubOne;
        else
            step = step + FingerprintUnrollCountSubOne;

        int i = 1;

        int end = final - step;

        for (; i < end; i += step)
        {
            fp += (ulong)(input[i] * i);
            fp ^= (ulong)input[i + 1] << 12;
            fp += (ulong)input[i + 2] << 24;
            fp ^= (ulong)(input[i + 3] + i) << 36;
            fp += (ulong)input[i + 4] << 48;
            fp = fp * 17 + input[i + 5];
        }
        
        // undo last += step and increment to next char
        if (l > FingerprintUnrollCountAddOne)
        {
            // multiply with a different huge prime (cred to FNV prime)
            fp *= 1099511628211UL;

            // undo step and leap unroll count +1 forward
            i = i - step + FingerprintUnrollCountAddOne;

            // limit to only last 64 chars
            int tailStartMin = final - 64;
            i = i < tailStartMin ? tailStartMin : i;

            for (; i < final; i++)
            {
                int shift = i % 44;

                fp ^= (ulong)input[i] << shift;
                fp += (ulong)input[i] << (shift + 4);
            }
        }
        else
        {
            fp = fp * 7013UL + (ulong)((ulong)input[l / 3] + (ulong)input[l / 4] + (ulong)input[l / 5]);

            fp ^= fp >> 11;

            for (; i < final; i++)
            {
                fp ^= ((ulong)input[i] << ((i * 61) & 53));
                fp = fp * 23 + input[i];
            }
        }

        fp = BitOperations.RotateLeft(fp, (int)(fp & 63));

        fp ^= fp >> 27;

        // multiply with a different huge prime (cred to MurmurHash)
        fp *= 0xC2B2AE3D27D4EB4FUL;
        fp ^= fp >> 33;

        return fp;
    }
}
