using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

partial class BaseTestKestrel
{
    /// <summary>
    /// Sends a GET to the <c>__Action</c> endpoint matching the calling test method name and returns the response body.
    /// </summary>
    /// <example>
    /// <code>
    /// public void MyTest()
    /// {
    ///     var text = GetActionResponseText();        // hits /MyTest__Action with a HttpGet
    ///     var text = GetActionResponseText("id=1");  // hits /MyTest__Action?id=1 with a HttpGet
    /// }
    /// </code>
    /// </example>
    [MethodImpl(MethodImplOptions.NoInlining)]
    protected string GetActionResponseText(string query = null, params (string Key, string Value)[] headers)
    {
        var response = GetActionResponseMessage(query, headers);

        return response.Content.ReadAsStringAsync().Result;
    }
}
