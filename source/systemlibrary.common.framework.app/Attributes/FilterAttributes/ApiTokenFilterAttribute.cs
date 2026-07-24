using Microsoft.AspNetCore.Mvc.Filters;

namespace SystemLibrary.Common.Framework.App.Attributes;

/// <summary>
/// Action filter that validates a request header value against a match pattern.<br/>
/// Defaults to the <c>api-token</c> header.<br/>
/// Match pattern rules:<br/>
/// - <c>null</c> or <c>*</c>: allow all
/// - regex (contains <c>^$*?[</c>): value must satisfy pattern
/// - pipe-delimited: must case-insensitively contain any one part
/// - wildcard: must case-insensitively satisfy wildcard pattern
/// - plain text: exact case-sensitive match
/// </summary>
/// <example>
/// <code>
/// [ApiTokenFilter("*")]                                    // allow all
/// [ApiTokenFilter("^[ab0-4]{4,}$")]                       // regex
/// [ApiTokenFilter("SystemLibrary.com")]                    // exact match
/// [ApiTokenFilter("client1|client2")]                      // pipe-delimited
/// [ApiTokenFilter("mytoken", headerName: "X-Api-Key")]     // custom header
/// </code>
/// </example>
public class ApiTokenFilterAttribute : BaseApiFilterAttribute
{
    new string Match;
    string HeaderName;

    /// <summary>
    /// Defaults header name to <c>api-token</c> if not specified.
    /// </summary>
    public ApiTokenFilterAttribute(string match, string headerName = "api-token")
    {
        this.Match = match;
        this.HeaderName = headerName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request?.Headers[this.HeaderName].ToString();

        try
        {
            var hasAccess = RequestHasValidHeaderValue(Match, value);

            if (hasAccess)
                base.OnActionExecuting(context);
            else
                base.OnAccessDenied(context, "Header " + this.HeaderName + " is incorrect");
        }
        catch (Exception ex)
        {
            base.OnAccessDenied(context, this.HeaderName + " threw: " + ex.Message);
        }
    }
}
