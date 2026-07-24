namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    /// <summary>
    /// Asserts that the value is valid. Fails the test if the value is invalid or default.
    /// </summary>
    public static void IsOk(DateTimeOffset value, string message = null)
    {
        IsOk(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is valid. Fails the test if the value is invalid or default.
    /// </summary>
    public static void IsOk(DateTime value, string message = null)
    {
        IsOk(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is valid. Fails the test if the value is invalid or default.
    /// </summary>
    public static void IsOk(int value, string message = null)
    {
        IsOk(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is valid. Fails the test if the value is invalid or default.
    /// </summary>
    public static void IsOk(bool value, string message = null)
    {
        IsOk(value, null, message);
    }

    /// <summary>
    /// Asserts that the value is valid — non-null, non-empty, non-error, and optionally starts with matchBegins.<br/>
    /// Strings containing "Error:" or "Exception:" or HTTP 4xx status names fail the assertion.<br/>
    /// Collections pass if non-empty. Numerics pass if greater than zero. DateTimes pass if not Min/MaxValue.
    /// </summary>
    /// <example>
    /// <code>
    /// IsOk(response);
    /// IsOk(json, "{ \"id\":");
    /// IsOk(statusCode, HttpStatusCode.OK);
    /// </code>
    /// </example>
    public static void IsOk(object value, object matchBegins = null, string message = null)
    {
        if (IsValueValid(value, matchBegins)) return;

        var msg = GetAssertionMessage(value, matchBegins, message);

        throw new Exception(msg);
    }

    /// <summary>
    /// Asserts that the value is an empty string.
    /// </summary>
    /// <example>
    /// <code>
    /// IsEmpty(result);
    /// </code>
    /// </example>
    public static void IsEmpty(object value, string message = null)
    {
        if (value == "") return;

        var msg = GetAssertionMessage(value, null, message);

        throw new Exception(msg);
    }

    /// <summary>
    /// Asserts that the value is null.
    /// </summary>
    /// <example>
    /// <code>
    /// IsNull(result);
    /// </code>
    /// </example>
    public static void IsNull(object value, string message = null)
    {
        if (value == null) return;

        var msg = GetAssertionMessage(value, null, message);

        throw new Exception(msg);
    }
}
