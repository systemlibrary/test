using System.Security.Claims;

using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static string[] VaryCacheByQueryNames = new[] { "id", "country", "sub", "subscription", "code", "lang", "type", "q", "format", "maxwidth", "maxheight", "version", "w", "h", "width", "height", "v", "category", "quality", "item", "sort", "sortBy", "sorting", "sortorder", "sortByDesc", "sortByAsc", "order", "orderBy", "pagesize", "size", "page", "pageNumber", "pageIndex", "filter", "filterBy", "name", "realm", "f", "i", "p", "l", "o" };

    static IServiceCollection UseOutputCache(this IServiceCollection services)
    {
        if (!FrameworkOptionsInstance.Current.UseOutputCache) return services;

        services.AddScoped<OutputCacheTagFilter>();

        return services.AddOutputCache(options =>
        {
            options.SizeLimit = 1000L * 1024 * 1024;       //1GB
            options.MaximumBodySize = 8 * 1024 * 1024;     //8MB
            options.UseCaseSensitivePaths = false;
            options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(20); // CacheDuration.XS

            // Note: we do not invoke AddBasePolicy as that starts to listen and apply to all requests in your application
            //options.AddBasePolicy(builder =>
            //{
            //});

            options.AddPolicy(OutputCachePolicy.CacheAuthenticated, policy =>
            {
                policy.VaryByValue(context =>
                {
                    return new("slcf__isauth", (context?.User?.Identity?.IsAuthenticated ?? false).ToString());
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });

            options.AddPolicy(OutputCachePolicy.CacheRoles, policy =>
            {
                policy.VaryByValue(context =>
                {
                    var user = context?.User;

                    var isAuth = user?.Identity?.IsAuthenticated ?? false;
                    if (!isAuth)
                        return new("slcf__r", "false");

                    var claimsIdentity = user.Identity as ClaimsIdentity;

                    if (user?.Claims != null)
                    {
                        var roles = user.Claims
                            .Where(c => c.Type == claimsIdentity.RoleClaimType ||
                                    c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                            .Select(x => x.Value);

                        var role = user.FindFirst(ClaimTypes.Role)?.Value;

                        if (roles.Is())
                            return new("slcf__r", string.Join("", roles) + role);

                        if (role.Is())
                            return new("slcf__r", role);
                    }
                    return new("slcf__r", "true");
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });

            options.AddPolicy(OutputCachePolicy.CacheUser, policy =>
            {
                policy.VaryByValue(context =>
                {
                    var user = context?.User;

                    var isAuth = user?.Identity?.IsAuthenticated ?? false;
                    if (!isAuth)
                        return new("slcf__user", "false");

                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId.IsNot())
                        userId = user.FindFirst("id")?.Value ?? user.Identity.Name;

                    var email = user.FindFirst(ClaimTypes.Email)?.Value;
                    if (email.IsNot())
                        email = user.FindFirst("email")?.Value ?? user.FindFirst("emailaddress")?.Value;

                    var phone = user.FindFirst(ClaimTypes.MobilePhone)?.Value;
                    if (phone.IsNot())
                        phone = user.FindFirst("phone")?.Value ?? user.FindFirst("phonenumber")?.Value ?? user.FindFirst("mobile")?.Value;

                    var sub = user.FindFirst("sub")?.Value;

                    if (sub.IsNot())
                        sub = user.FindFirst("user_id")?.Value;

                    var cacheKey = userId + email + phone + sub  + user?.Identity?.Name;

                    if (cacheKey.IsNot() || cacheKey.Length < 5)
                        return new("slcf__user", "false");

                    return new("slcf__user", cacheKey);
                });

                policy.SetVaryByQuery(VaryCacheByQueryNames);

                policy.Expire(TimeSpan.FromSeconds(1200)); // CacheDuration.M
            });
        });
    }
}