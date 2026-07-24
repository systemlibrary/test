using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

partial class BaseTestKestrel
{
    /// <summary>
    /// Sends a GET to the <c>__Action</c> endpoint matching the calling test method name and returns the response message.
    /// </summary>
    /// <remarks>
    /// Non-2xx responses are logged via Log.Dump but not thrown.
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = GetActionResponseMessage("id=42");
    /// Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected HttpResponseMessage GetActionResponseMessage(string query = null, params (string key, string value)[] headers)
    {
        var stackFrame = new StackFrame(1);
        var method = stackFrame.GetMethod();
        var name = method.Name;
        
        if (name == "GetActionResponseText")
        {
            stackFrame = new StackFrame(2);
            method = stackFrame.GetMethod();
            name = method.Name;
        }

        if (query != null)
        {
            if (!query.StartsWith("?"))
                query = "?" + query;
        }

        var pathAndQuery = name + "__Action" + query;

        var response = GetResponse(pathAndQuery, headers);

        if (!response.IsSuccessStatusCode && pathAndQuery?.Contains("?username=") != true)
        {
            var message = pathAndQuery + " not successful: " + response.StatusCode + " " + response.ReasonPhrase;

            Console.WriteLine(message);

            global::Log.Dump(message);
        }

        return response;
    }
}
