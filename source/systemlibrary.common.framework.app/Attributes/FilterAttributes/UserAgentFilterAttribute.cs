using Microsoft.AspNetCore.Mvc.Filters;

using SystemLibrary.Common.Framework.App.Extensions;

namespace SystemLibrary.Common.Framework.App.Attributes;

/// <summary>
/// Action filter that validates the <c>User-Agent</c> request header against a match pattern.<br/>
/// Always blocks known bots, spiders and crawlers regardless of match pattern.<br/>
/// Match pattern rules:
/// - <c>null</c> or <c>*</c>: allow all
/// - regex (contains <c>^$*?[</c>): user agent must satisfy pattern
/// - pipe-delimited: must case-insensitively contain any one part
/// - wildcard: must case-insensitively satisfy wildcard pattern
/// </summary>
/// <example>
/// <code>
/// [UserAgentFilter("*")]                          // allow all (bots still blocked)
/// [UserAgentFilter("^[ab0-4]{4,}$")]              // regex
/// [UserAgentFilter("*edg*")]                      // wild card match
/// [UserAgentFilter("firefox|edg|chrome")]         // pipe-delimited
/// </code>
/// </example>
public class UserAgentFilterAttribute : BaseApiFilterAttribute
{
    new string Match;

    /// <summary>
    /// Defaults to null, allowing all non-bot user agents if not specified.
    /// </summary>
    public UserAgentFilterAttribute(string match = null)
    {
        this.Match = match;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var value = context?.HttpContext?.Request.UserAgent();

        var hasAccess = value == null || !IsBlacklisted(value);

        if (hasAccess)
            hasAccess = RequestHasValidHeaderValue(Match, value);

        if (hasAccess)
        {
            base.OnActionExecuting(context);
        }
        else
        {
            base.OnAccessDenied(context, "User agent is incorrect");
        }
    }

    static bool IsBlacklisted(string value)
    {
        if (value.Length == 0) return false;

        return
            UserAgents.ScriptsRegex.IsMatch(value) ||
            UserAgents.BotsRegex.IsMatch(value);
    }
}
