using System.Text;

namespace SystemLibrary.Common.Framework;

partial class BaseTestKestrel
{
    HttpResponseMessage GetResponse(string pathAndQuery, (string key, string value)[] headers)
    {
        var fullUrl = Url + Port + (pathAndQuery?.StartsWith("/") == true ? pathAndQuery : "/" + pathAndQuery);

        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);

        request.Headers.Add("Cookie", ".DummyCookie=DummyValue");

        request.Headers.Add("X-Dummy-Header", "Test");

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

        try
        {
            return Client.SendAsync(request)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch(Exception ex)
        {
            BootstrapLog.Dump(fullUrl + " " + ex);

            throw;
        }
    }
}
