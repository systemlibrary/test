using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Obfuscates a string using a reversible character shift with a salt.
    /// Output may contain non-printable Unicode characters — encode with <c>ToBase64()</c> or <c>ToBase62()</c> before transporting.
    /// </summary>
    /// <remarks>
    /// Salt defaults to -1, using the configured application name as salt.
    /// Deterministic and reversible with the same salt — not intended for security or encryption.
    /// Valid salt values are any integer whose normalized value (salt % 65536) is non-zero.
    /// </remarks>
    /// <example>
    /// <code>
    /// var obfuscated = "Hello world".Obfuscate();
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static string Obfuscate(this string text, int salt = -1)
    {
        return SystemLibrary.Common.Framework.Obfuscator.Obfuscate(text, salt);
    }

    /// <summary>
    /// Reverses obfuscation applied by <c>Obfuscate()</c>. Must use the same salt value.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = "Hello world".Obfuscate().Deobfuscate(); // "Hello world"
    /// </code>
    /// </example>
    public static string Deobfuscate(this string text, int salt = -1)
    {
        return Obfuscator.Deobfuscate(text, salt);
    }
}
