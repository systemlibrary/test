using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void WebBranchEnd(this IApplicationBuilder branch, IFrameworkHooks hooks = null)
    {
        if (FrameworkOptionsInstance.Current.UseOutputCache)
            branch.UseOutputCache();

        if (FrameworkOptionsInstance.Current.UseAuthorization && FrameworkOptionsInstance.Current.UseAuthentication)
            branch.UseAuthorization();

        hooks?.OnBeforeEndpoints(branch, BranchType.Web, EnvironmentType);

        if (FrameworkOptionsInstance.Current.UseHsts)
            branch.UseHsts();

        if (FrameworkOptionsInstance.Current.UseBrotliResponseCompression || FrameworkOptionsInstance.Current.UseGzipResponseCompression)
            branch.UseCompression();

        branch.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapRazorPages();

            endpoints.UseMapMetrics();
        });

        hooks?.OnAfterEndpoints(branch, BranchType.Web, EnvironmentType);

        hooks?.OnPipelineEnd(branch, BranchType.Web, EnvironmentType);
    }

    static bool IsWebBranch(HttpContext ctx)
    {
        return !IsApiBranch(ctx) && !IsStaticBranch(ctx);
    }
}