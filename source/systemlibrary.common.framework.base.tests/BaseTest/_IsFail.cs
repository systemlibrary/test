namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    /// <summary>
    /// Asserts that the value is invalid or default. Fails the test if the value is considered valid.
    /// </summary>
    public static void IsFail(DateTimeOffset value, string message = null)
    {
        IsFail(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is invalid or default. Fails the test if the value is considered valid.
    /// </summary>
    public static void IsFail(DateTime value, string message = null)
    {
        IsFail(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is invalid or default. Fails the test if the value is considered valid.
    /// </summary>
    public static void IsFail(int value, string message = null)
    {
        IsFail(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is invalid or default. Fails the test if the value is considered valid.
    /// </summary>
    public static void IsFail(bool value, string message = null)
    {
        IsFail(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is invalid or default. Optionally fails if value begins with noMatchBegins.<br/>
    /// Inverse of IsOk.
    /// </summary>
    /// <example>
    /// <code>
    /// IsFail(response);
    /// IsFail(json, "unexpected-prefix");
    /// </code>
    /// </example>
    public static void IsFail(object value, object noMatchBegins = null, string message = "")
    {
        if (!IsValueValid(value, noMatchBegins)) return;

        var msg = GetAssertionMessage(value, message);

        throw new Exception(msg);
    }
}
