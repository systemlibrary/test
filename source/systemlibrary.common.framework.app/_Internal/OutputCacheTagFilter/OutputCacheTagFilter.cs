using System.Reflection;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;

using SystemLibrary.Common.Framework.App;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class OutputCacheTagFilter : IAsyncActionFilter
{
    IOutputCacheStore _cacheStore;

    public OutputCacheTagFilter(IOutputCacheStore cacheStore)
    {
        _cacheStore = cacheStore;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context?.HttpContext?.User;

        var isAuthenticated = (user?.Identity?.IsAuthenticated ?? false);

        if (!isAuthenticated)
        {
            await next();
            return;
        }

        var outputCacheAttribute = context.ActionDescriptor.EndpointMetadata.OfType<OutputCacheAttribute>().FirstOrDefault();
        var tags = outputCacheAttribute?.Tags;

        if (tags.IsNot())
        {
            await next();
            return;
        }

        var skipWhenAuthenticated = tags.Any(t => t?.Contains(OutputCacheTag.SkipWhenAuthenticated, StringComparison.OrdinalIgnoreCase) == true);

        if (skipWhenAuthenticated)
        {
            DisableOutputCacheDuration(outputCacheAttribute);
        }
        else
        {
            var skipWhenAdmin = tags.Any(t => t?.Contains(OutputCacheTag.SkipWhenAdmin, StringComparison.OrdinalIgnoreCase) == true);
            if (skipWhenAdmin)
            {
                var isAdmin = user.IsInAnyRole("administrator", "admin", "administrators", "admins", "Administrators", "Administrator", "Admins", "Admin");

                if (isAdmin)
                {
                    DisableOutputCacheDuration(outputCacheAttribute);
                }
            }
        }

        await next();
    }

    static readonly FieldInfo DurationField = typeof(OutputCacheAttribute).GetField("_duration", BindingFlags.NonPublic | BindingFlags.Instance);

    static void DisableOutputCacheDuration(OutputCacheAttribute outputCacheAttribute)
    {
        if (DurationField != null)
            DurationField.SetValue(outputCacheAttribute, 0);
        else
            Log.Error("Duration was not reset, the _duration field does no longer exist in 'AspNetCore.OutputCacheAttribute' class.");
    }
}