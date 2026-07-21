namespace SystemLibrary.Common.Framework;

/// <summary>
/// Specifies the text encoding used by <c>JsonEncryptAttribute</c>.
/// <c>Auto</c> defaults to UTF8.
/// </summary>
public enum EncodingType
{
    UTF8 = 0,
    UTF16 = 1,
    UTF32 = 2,
    Ascii = 4,
    Latin1 = 8,
    Unicode = 16,
    BigEndianUnicode = 32
}