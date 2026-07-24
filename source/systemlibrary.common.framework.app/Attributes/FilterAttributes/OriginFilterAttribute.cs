using Microsoft.AspNetCore.Mvc.Filters;

namespace SystemLibrary.Common.Framework.App.Attributes;

/// <summary>
/// Action filter that validates the <c>Origin</c> request header against a match pattern.<br/>
/// Match pattern rules:<br/>
/// - <c>null</c> or <c>*</c>: allow all
/// - regex (contains <c>^$*?[</c>): origin must satisfy pattern
/// - pipe-delimited: must case-insensitively contain any one part
/// - wildcard: must case-insensitively satisfy wildcard pattern
/// - plain text: exact case-sensitive match
/// </summary>
/// <example>
/// <code>
/// [OriginFilter("*")]                                                         // allow all
/// [OriginFilter("^[ab0-4]{4,}$")]                                             // regex
/// [OriginFilter("SystemLibrary.com")]                                         // exact match
/// [OriginFilter("test.systemlibrary.com|test2.systemlibrary.com")]            // pipe-delimited
/// </code>
/// </example>
public class OriginFilterAttribute : BaseApiFilterAttribute
{
    new string Match;

    /// <summary>
    /// Defaults to null, allowing all origins if not specified.
    /// </summary>
    public OriginFilterAttribute(string match = null)
    {
        this.Match = match;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request?.Headers["Origin"].ToString();

        var hasAccess = RequestHasValidHeaderValue(this.Match, value);

        if (hasAccess)
        {
            base.OnActionExecuting(context);
        }
        else
        {
            base.OnAccessDenied(context, "Origin is missing or incorrect");
        }
    }
}