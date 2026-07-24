using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseForwardStdToLog(this IServiceCollection services)
    {
        if (LogInstance.StdForward != StdForward.StdToLog) return services;

        return services.AddLogging(bld =>
        {
            bld.ClearProviders();

            bld.AddProvider(new StdToLogProvider());
        });
    }
}