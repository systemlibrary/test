using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Extension methods for IServiceCollection.
/// </summary>
public static partial class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers all framework services and a custom <c>ILogWriter</c> implementation.
    /// </summary>
    /// <remarks>
    /// Throws if <c>log:forward</c> is set to <c>LogToStd</c> — only one logging strategy can be active.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddFrameworkServices&lt;MyLogWriter&gt;(AppType.Web);
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices<TLogWriter>(this IServiceCollection serviceCollection, AppType type = AppType.Web) where TLogWriter : class, ILogWriter
    {
        serviceCollection.AddSingleton<ILogWriter, TLogWriter>();

        return serviceCollection.AddFrameworkServices(type);
    }

    /// <summary>
    /// Registers all framework services required for the configured <c>AppType</c>.<br/>
    /// Console apps register a minimal set — memory cache and logging only.<br/>
    /// Web and Api additionally register MVC, routing, compression, auth, output cache, forwarded headers and views.
    /// </summary>
    /// <example>
    /// <code>
    /// services.AddFrameworkServices(AppType.Web);
    /// </code>
    /// </example>
    public static IServiceCollection AddFrameworkServices(this IServiceCollection services, AppType type = AppType.Web)
    {
        services = services.UseForwardStdToLog();

        services = services.AddCommonServices();

        services = services.UseDataProtectionPolicy();

        services = services.AddMemoryCache(options =>
        {
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(2);
            options.CompactionPercentage = 0.33;
        });

        var appTypeIsWebOrApi = type > 0;

        if (appTypeIsWebOrApi)
        {
            services = services.UseForwardedHeaders();

            if (FrameworkOptionsInstance.Current.UseHttpsRedirection)
                services = services.AddHttpsRedirection(opt => opt.HttpsPort = 443);

            if (FrameworkOptionsInstance.Current.UseGzipResponseCompression)
                services = services.UseGzipCompression();

            if (FrameworkOptionsInstance.Current.UseBrotliResponseCompression)
                services = services.UseBrotliCompression();

            services = services.UseOutputCache();

            services = services.UseResponseCaching();

            services = services.UseCookiePolicy();

            services = services.UseAuthentication();

            if (FrameworkOptionsInstance.Current.UseAuthorization)
                services.AddAuthorization();

            services = services.UseAddRouting();

            services.AddSession(options =>
            {
                options.IOTimeout = TimeSpan.FromSeconds(5);
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            IMvcBuilder builder = services.UseModelViewControllers();

            builder = builder
                .UseDefaultJsonConverters()
                .UseApplicationParts();

            // REMOVED: This takes a toll on the bin/obj folders by default in consumers projects
            // making it not suitable for api's, test projects, libraries
            // Consumers will have to opt-in and register this themselves manually as it comes with too much bloat (files of total hundred MB or so...)
            // So while msbuild/build is still using obj and copying later on all files to bin, duplicated file writes, and slow reading/loading of files for every project, this wont be added as part of the framework (most likely)
            //if (options.UseRazorRuntimeCompilationOnSave)
            //    builder = AddRazorRuntimeCompilationOnSave(builder);

            if (builder != null)
            {
                services = builder.Services;
            }

            services = services.Replace(ServiceDescriptor.Singleton<IActionSelector, OverloadActionSelector>());

            services = services.UseViews();

            services.TryAddTransient<HtmlHelperFactory, HtmlHelperFactory>();

            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                });
            });
        }

        return services;
    }
}