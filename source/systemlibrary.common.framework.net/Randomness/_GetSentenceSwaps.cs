namespace SystemLibrary.Common.Framework;

partial class Randomness
{
    static List<string[]> GetSentenceSwaps()
    {
        return new List<string[]>
        {
            new[] { "every", "each", "some" },
            new[] { "the", "a", "one" },
            new[] { "her", "him", "they" }
        };
    }
}
