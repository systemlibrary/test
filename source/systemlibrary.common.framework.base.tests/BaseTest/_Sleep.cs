namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    /// <summary>
    /// Pauses the current thread for <paramref name="milliseconds"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Sleep(100);
    /// </code>
    /// </example>
    public static void Sleep(int milliseconds = 20)
    {
        Thread.Sleep(milliseconds);
    }
}
