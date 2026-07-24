using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void MapApiBranchEnd(this IApplicationBuilder app, IFrameworkHooks hooks = null)
    {
        app.UseWhen(IsApiBranch, branch =>
        {
            branch.UseCors();

            branch.UseRouting();

            if (FrameworkOptionsInstance.Current.UseOutputCache)
                branch.UseOutputCache();

            if (FrameworkOptionsInstance.Current.UseAuthorization && FrameworkOptionsInstance.Current.UseAuthentication)
                branch.UseAuthorization();

            hooks?.OnBeforeEndpoints(branch, BranchType.Api, EnvironmentType);

            if (FrameworkOptionsInstance.Current.UseHsts)
                branch.UseHsts();

            if (FrameworkOptionsInstance.Current.UseBrotliResponseCompression || FrameworkOptionsInstance.Current.UseGzipResponseCompression)
                branch.UseCompression();

            branch.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            hooks?.OnAfterEndpoints(branch, BranchType.Api, EnvironmentType);
            
            hooks?.OnPipelineEnd(branch, BranchType.Api, EnvironmentType);
        });
    }

    static bool IsApiBranch(HttpContext ctx)
    {
        var path = ctx?.Request?.Path.Value;

        if (path?.Length > 3)
        {
            return path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}