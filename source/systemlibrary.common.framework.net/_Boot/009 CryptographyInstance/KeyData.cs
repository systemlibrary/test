namespace SystemLibrary.Common.Framework;

internal class KeyData
{
    public string Source;
    public string KeyStart;
    public byte[] KeyStartBytes;
    public byte[] Key;

    internal static KeyData Create(string source, string key)
    {
        key = key + "#?(Ø^";

        var keyHashed = key.ToSha256Hash();

        var bytes = Convert.FromHexString(keyHashed);

        return new KeyData()
        {
            Source = source,
            KeyStart = key.MaxLength(3),
            Key = bytes,
            KeyStartBytes = key.MaxLength(3).GetBytes()
        };
    }
}
