using Microsoft.AspNetCore.Mvc;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Abstract base class for API controllers with automatic endpoint documentation and built-in response helpers.
/// All controllers inheriting BaseApiController are automatically picked up if matching the request path.
/// </summary>
public partial class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns an HTTP 200 JSON response. Properties are camelCased by default.
    /// </summary>
    /// <example>
    /// <code>
    /// return Json(user);
    /// return Json(user, camelCase: false);
    /// </code>
    /// </example>
    public ActionResult Json(object data, bool camelCase = true)
    {
        return new ContentResult
        {
            Content = data.Json(camelCase),
            ContentType = "application/json; charset=utf-8",
            StatusCode = 200
        };
    }

    /// <summary>
    /// Returns an HTTP 200 XML response.
    /// </summary>
    /// <example>
    /// <code>
    /// return Xml(user);
    /// </code>
    /// </example>
    public ActionResult Xml(object data)
    {
        return new ContentResult
        {
            Content = data.Xml(),
            ContentType = "application/xml; charset=utf-8",
            StatusCode = 200
        };
    }
}
