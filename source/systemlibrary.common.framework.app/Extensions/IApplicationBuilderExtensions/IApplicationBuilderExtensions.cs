using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App.Extensions;

/// <summary>
/// Extension methods for configuring the framework middleware pipeline on <c>IApplicationBuilder</c>.
/// </summary>
public static partial class IApplicationBuilderExtensions
{
    static EnvironmentType EnvironmentType = EnvironmentConfig.Current.EnvironmentType;

    public static IApplicationBuilder UseFrameworkBeginMiddlewares(this IApplicationBuilder app, AppType type = AppType.Web, IFrameworkHooks hooks = null)
    {
        ServicesInstance.ServiceProvider = app.ApplicationServices;

        ContextAccessorInstance.HttpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

        if (type == AppType.Console) return app;

        if(type == AppType.Web)
            app.Use(AutomationRequestRateLimiterMiddleware.Build);

        app.Use(BlockRequestMiddleware.Build);

        if (FrameworkOptionsInstance.Current.UseForwardedHeaders)
            app.UseXForwardedHeaders();

        if (FrameworkOptionsInstance.Current.UseHttpsRedirection)
            app.UseHttpsRedirection();

        if (FrameworkOptionsInstance.Current.UseStatusCodeLogger)
            app.UseMiddleware<StatusCodeLoggerMiddleware>();

        if (FrameworkOptionsInstance.Current.UseDeveloperPage)
            app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions() { SourceCodeLineCount = 4 });

        hooks?.OnPipelineBegin(app, EnvironmentType);

        app.StaticBranch(hooks);
        
        app.UseDuplicateSlashRemoval();

        app.UseRouting();

        if (FrameworkOptionsInstance.Current.UseAuthentication)
            app.UseAuthentication();

        app.UseCookiePolicy();

        if (FrameworkOptionsInstance.Current.UseSecurityPolicy)
        {
            app.UseWhen(IsApiBranch, branch =>
            {
                branch.Use(next => SecurityPolicyMiddleware.Build(next, BranchType.Api));
            });

            app.UseWhen(IsWebBranch, branch =>
            {
                branch.Use(next => SecurityPolicyMiddleware.Build(next, BranchType.Web));
            });
        }

        return app;
    }

    public static IApplicationBuilder UseFrameworkEndMiddlewares(this IApplicationBuilder app, AppType type = AppType.Web, IFrameworkHooks hooks = null)
    {
        if (type == AppType.Console) return app;

        app.MapApiBranchEnd(hooks);
        app.WebBranchEnd(hooks);

        return app;
    }
}