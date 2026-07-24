using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseAddRouting(this IServiceCollection services)
    {
        return services.AddRouting(opt =>
        {
            opt.SuppressCheckForUnhandledSecurityMetadata = false;
            opt.AppendTrailingSlash = false;
            opt.LowercaseQueryStrings = false;
            opt.LowercaseUrls = true;
        });
    }
}