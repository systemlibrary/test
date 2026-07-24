using System.Text;

namespace SystemLibrary.Common.Framework;

partial class BaseTestServer
{
    /// <summary>
    /// Sends a GET to the given path and returns the response body.
    /// </summary>
    /// <remarks>
    /// Non-2xx responses are logged via Log.Dump but not thrown.
    /// </remarks>
    /// <example>
    /// <code>
    /// var text = GetResponseText("/api/users?id=1");
    /// </code>
    /// </example>
    protected string GetResponseText(string path, params (string Key, string Value)[] headers)
    {
        return GetResponseTextAsync(path, headers)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    protected async Task<string> GetResponseTextAsync(string path, params (string Key, string Value)[] headers)
    {
        var response = await GetResponseAsync(path, headers);

        if (!response.IsSuccessStatusCode)
            global::Log.Dump("Not successful: " + path + " " + response.StatusCode + " " + response.ReasonPhrase);

        return await response.Content.ReadAsStringAsync();
    }
}
