namespace SystemLibrary.Common.Framework;

partial class BaseTestServer
{
    /// <summary>
    /// Sends a GET to the given path and returns the response message.
    /// </summary>
    /// <example>
    /// <code>
    /// var response = GetResponseMessage("/api/users?id=1");
    /// Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    /// </code>
    /// </example>
    protected HttpResponseMessage GetResponseMessage(string path, params (string key, string value)[] headers)
    {
        return GetResponseAsync(path, headers)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
