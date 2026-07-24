using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseAuthentication(this IServiceCollection services)
    {
        if (!FrameworkOptionsInstance.Current.UseAuthentication)
        {
            // NOTE: Some ASP.NET components assume a cookie scheme exists.
            // If AddAuthentication().AddCookie() is not registered, certain
            // policies / middleware may throw exceptions during registration or runtime.
            services.AddAuthentication().AddCookie();

            return services;
        }

        var cookieSecurePolicy = EnvironmentConfig.IsDevelopment
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        services.AddAuthentication()
                .AddCookie(opt =>
                {
                    opt.Cookie.SameSite = SameSiteMode.Strict;
                    opt.Cookie.HttpOnly = true;
                    opt.Cookie.SecurePolicy = cookieSecurePolicy;
                    opt.SlidingExpiration = false;
                    opt.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

        services = services.ConfigureApplicationCookie(opt =>
        {
            opt.Cookie.SameSite = SameSiteMode.Strict;
            opt.Cookie.HttpOnly = true;
            opt.Cookie.SecurePolicy = cookieSecurePolicy;
            opt.SlidingExpiration = true;
            opt.ExpireTimeSpan = TimeSpan.FromHours(16);
            opt.LoginPath = "/login/";
        });


        return services.ConfigureExternalCookie(opt =>
        {
            opt.Cookie.SameSite = SameSiteMode.Lax;
            opt.Cookie.HttpOnly = true;
            opt.Cookie.SecurePolicy = cookieSecurePolicy;
            opt.SlidingExpiration = false;
            opt.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        });
    }
}