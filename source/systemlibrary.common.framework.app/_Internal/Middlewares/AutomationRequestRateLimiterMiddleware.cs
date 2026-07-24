using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App;

static class AutomationRequestRateLimiterMiddleware
{
    const int RateLimit = 1200;

    static ShardedDictionary<string, (int count, long resetAt)> Rate = new();

    static long IntervalSeconds = 120;

    public static RequestDelegate Build(RequestDelegate next)
    {
        return context =>
        {
            var userAgent = context?.Request?.Headers?.UserAgent.ToString();

            if (userAgent?.StartsWith("Moz", StringComparison.Ordinal) == true)
                return next(context);

            string key = null;

            if (userAgent.IsNot())
                key = "";
            else
            {
                foreach (var script in UserAgents.Scripts)
                {
                    if (userAgent.Contains(script, StringComparison.OrdinalIgnoreCase))
                    {
                        key = script;
                        break;
                    }
                }
            }

            if (key != null)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var tries = 5;

                while (tries > 0)
                {
                    tries--;
                    if (!Rate.TryGetValue(key, out var old))
                    {
                        if (Rate.TryAdd(key, (1, now + IntervalSeconds)))
                            break;

                        continue;
                    }

                    (int count, long resetAt) updated;

                    if (now > old.resetAt)
                        updated = (1, now + IntervalSeconds);
                    else
                        updated = (old.count + 1, old.resetAt);

                    if (Rate.TryUpdate(key, updated, old))
                    {
                        if (updated.count > RateLimit)
                        {
                            context.Response.StatusCode = 429;
                            context.Response.Headers.TryAdd("Reason", "Access denied by SystemLibrary.Common.Framework due to too many requests.");
                            return Task.CompletedTask;
                        }
                        break;
                    }
                }
            }

            return next(context);
        };
    }
}
