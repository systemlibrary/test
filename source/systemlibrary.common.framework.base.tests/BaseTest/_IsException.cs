namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    /// <summary>
    /// Asserts that the action throws. Fails the test if no exception is thrown.
    /// </summary>
    /// <example>
    /// <code>
    /// IsException(() => service.DoSomething(null));
    /// </code>
    /// </example>
    public static void IsException(Action action, string message = "")
    {
        var threw = false;

        try
        {
            action();
        }
        catch
        {
            threw = true;
        }

        if (!threw)
        {
            message = GetAssertionMessage(action, message);

            throw new Exception(message);
        }
    }
}
