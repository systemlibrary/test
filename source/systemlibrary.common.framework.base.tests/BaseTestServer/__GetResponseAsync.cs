using System.Text;

namespace SystemLibrary.Common.Framework;

partial class BaseTestServer
{
    /// <summary>
    /// Async version of <c>GetResponseText</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// var text = await GetResponseTextAsync("/api/users?id=1");
    /// </code>
    /// </example>
    async Task<HttpResponseMessage> GetResponseAsync(string path, (string key, string value)[] headers)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, path);

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                if (key.Is() && value != null)
                {
                    if (key == "Content-Type")
                    {
                        request.Content = new StringContent("", Encoding.UTF8, value);
                    }
                    request.Headers.TryAddWithoutValidation(key, value);
                }
            }
        }

        return await Client.SendAsync(request);
    }
}
