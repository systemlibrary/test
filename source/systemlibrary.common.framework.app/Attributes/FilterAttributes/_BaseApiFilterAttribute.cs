using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SystemLibrary.Common.Framework.App.Attributes;

/// <summary>
/// Base class for custom action filter attributes. Provides header value matching and access denied responses.
/// </summary>
/// <remarks>
/// RequestHasValidHeaderValue matching order: exact → pipe-delimited (case-insensitive) → wildcard → regex.<br/>
/// Regex patterns are compiled and cached per unique pattern.<br/>
/// - Swallows invalid regex patterns and continues — no exception thrown
/// </remarks>
/// <example>
/// <code>
/// public class ClientAuthFilterAttribute : BaseApiFilterAttribute
/// {
///     string Match;
///
///     public ClientAuthFilterAttribute()
///     {
///         var config = ClientAuthConfig.Current;
///         this.Match = config.Client + ":" + config.Token;
///     }
///
///     public override void OnActionExecuting(ActionExecutingContext context)
///     {
///         var value = context?.HttpContext?.Request?.Headers["X-Api-Key"].ToString();
///
///         if (RequestHasValidHeaderValue(this.Match, value))
///             base.OnActionExecuting(context);
///         else
///             base.OnAccessDenied(context, "X-Api-Key is incorrect");
///     }
/// }
/// </code>
/// </example>
public class BaseApiFilterAttribute : ActionFilterAttribute
{
    static ShardedDictionary<string, Regex> RegexCache = new ShardedDictionary<string, Regex>();

    protected bool RequestHasValidHeaderValue(string match, string value)
    {
        if (match == null) return true;
        if (match == "*") return true;
        if (value == null) return false;
        if (value == match) return true;

        int matchLength = match.Length;

        // String delimiter where each part is case insensitive matched
        var isDelimiterMatch = match[0] != '^' && match.Contains("|");
        if (isDelimiterMatch)
        {
            // TODO: Optimize in hot paths, no need to string array every time instead of just looping once and skip till next |
            var parts = match.Split('|');

            foreach (var part in parts)
            {
                if (part == null) continue;

                if (part == "" && value == "") return true;

                if (part.Length == 0) continue;

                if (value.Contains(part, StringComparison.OrdinalIgnoreCase)) return true;
            }
        }

        var isWildcardMatch = match.Contains("*");
        if (isWildcardMatch)
        {
            string pattern = match;

            if (pattern.StartsWith("*") && pattern.EndsWith("*") && pattern.Length > 2)
            {
                var inner = pattern.AsSpan(1, pattern.Length - 2);
                if (value.Contains(inner, StringComparison.OrdinalIgnoreCase)) return true;
            }
            else if (pattern.StartsWith("*"))
            {
                var inner = pattern.AsSpan(1);
                if (value.EndsWith(inner, StringComparison.OrdinalIgnoreCase)) return true;
            }
            else if (pattern.EndsWith("*"))
            {
                var inner = pattern.AsSpan(0, pattern.Length - 1);
                if (value.StartsWith(inner, StringComparison.OrdinalIgnoreCase)) return true;
            }
        }

        var isRegexMatch = IsRegex(match);
        if (isRegexMatch)
        {
            try
            {
                var regex = RegexCache.Cache(match, () =>
                {
                    return new Regex(match, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                });

                var regexMatch = regex.IsMatch(value);

                if (regexMatch) return true;
            }
            catch
            {
                // swallow, continue as the match is not a real regex
            }
        }

        return false;
    }

    protected void OnAccessDenied(ActionExecutingContext context, string message)
    {
        var result = new ContentResult();

        // TODO: Support plain/text and xml responses too
        result.Content = @"{ ""success"": false, ""status"": 403, ""type"": """ + this.GetType().Name + "\", \"error\": \"" + message + "\" }";

        result.ContentType = "application/json";

        result.StatusCode = 403;

        context.Result = result;
    }

    static bool IsRegex(string match)
    {
        if (match.Length == 0) return false;

        if (match[0] == '^') return true;

        int l = match.Length - 1;

        if (match[l] == '$') return true;

        for (int i = 1; i <= l; i++)
        {
            char c = match[i];
            if (c == '*' || c == '?' || c == '[' || c == '(')
                return true;
        }

        return l >= 1 && match[l - 1] == '/' && match[l] == 'i';
    }

}
