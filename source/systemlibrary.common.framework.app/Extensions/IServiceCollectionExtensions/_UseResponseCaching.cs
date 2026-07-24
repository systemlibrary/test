using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseResponseCaching(this IServiceCollection services)
    {
        if (!FrameworkOptionsInstance.Current.UseResponseCaching) return services;

        return services.AddResponseCaching(opt2 =>
        {
            opt2.SizeLimit = 1000L * 1024 * 1024;       //1GB
            opt2.MaximumBodySize = 10 * 1024 * 1024;    //10MB
            opt2.UseCaseSensitivePaths = false;
        });
    }
}