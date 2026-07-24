using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseModelViewControllers(this IServiceCollection services)
    {
        return services.AddMvc(opt =>
        {
            opt.AllowEmptyInputInBodyModelBinding = false; // Empty body if param is set, throws and returns auto 400

            opt.OutputFormatters.Add(new OutputContentTypesSupported());

            if (FrameworkOptionsInstance.Current.UseOutputCache)
                opt.Filters.AddService(typeof(OutputCacheTagFilter));

            opt.Conventions.Add(new FrameworkRoutingConventions());
        });
    }
}