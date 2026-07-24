using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void StaticBranch(this IApplicationBuilder app, IFrameworkHooks hooks = null)
    {
        if (FrameworkOptionsInstance.Current.UseStaticFilePolicy)
        {
            app.MapWhen(IsStaticBranch, branch =>
            {
                if (FrameworkOptionsInstance.Current.UseResponseCaching)
                    branch.UseResponseCaching();

                if (FrameworkOptionsInstance.Current.UseSecurityPolicy)
                    branch.Use(next => SecurityPolicyMiddleware.Build(next, BranchType.Static));

                branch.Use(StaticFilePolicyMiddleware.Build);

                hooks?.OnPipelineEnd(branch, BranchType.Static, EnvironmentType);
            });
        }
    }

    internal static bool IsStaticBranch(HttpContext ctx)
    {
        var path = ctx?.Request?.Path.Value;

        if (path?.Length > 4)
        {
            var len = path.Length;

            if (len < 4096 &&
                (path[len - 3] == '.' ||
                path[len - 4] == '.' ||
                path[len - 5] == '.'))
            {
                return true;
            }
            else if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                return path.IsFile();
            }
        }
        return false;
    }
}