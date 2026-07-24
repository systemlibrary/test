using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

partial class BaseTestKestrel
{
    /// <summary>
    /// Sends a GET to an arbitrary path and returns the response body. Use for testing controller routes directly.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = GetControllerResponseText("/api/users?id=1");
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected string GetControllerResponseText(string pathAndQuery, params (string Key, string Value)[] headers)
    {
        var response = GetResponse(pathAndQuery, headers);

        if (!response.IsSuccessStatusCode && pathAndQuery?.Contains("?username=") != true)
            global::Log.Dump("Not successful: " + pathAndQuery + " " + response.StatusCode + " " + response.ReasonPhrase);

        return response.Content.ReadAsStringAsync().Result;
    }
}
