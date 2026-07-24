namespace SystemLibrary.Common.Framework;

partial class BaseTestKestrel
{
    /// <summary>
    /// Sends a GET to an arbitrary path and returns the response message. Use for testing controller routes directly.
    /// </summary>
    /// <example>
    /// <code>
    /// var response = GetControllerResponseMessage("/api/users?id=1");
    /// Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    /// </code>
    /// </example>
    protected HttpResponseMessage GetControllerResponseMessage(string pathAndQuery, params (string Key, string Value)[] headers)
    {
        var response = GetResponse(pathAndQuery, headers);

        if (!response.IsSuccessStatusCode && pathAndQuery?.Contains("?username=") != true)
            global::Log.Dump("Not successful: " + pathAndQuery + " " + response.StatusCode + " " + response.ReasonPhrase);

        return response;
    }
}
