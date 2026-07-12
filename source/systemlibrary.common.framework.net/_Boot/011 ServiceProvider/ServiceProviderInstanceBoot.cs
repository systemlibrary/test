using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Boostrap;

static class ServiceProviderBoot
{
    internal static void Strap() { }

    static ServiceProviderBoot()
    {
        // NOTE: Middleware registration overwrites this instance, purely to avoid some errors during service registration and earlier
        ServiceProviderInstance.Current = CreateDefaultServiceProviderInstance();
    }

    static IServiceProvider CreateDefaultServiceProviderInstance()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection
            .AddDataProtection()
            .SetApplicationName(AppInstance.Name)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365 * 100));

        serviceCollection.TryAddSingleton<IModelMetadataProvider, EmptyModelMetadataProvider>();

        serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        serviceCollection.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        serviceCollection.TryAddSingleton<ITempDataProvider, DummyTempDataProvider>();

        return serviceCollection.BuildServiceProvider();
    }

    class DummyTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) 
        {
        }
    }
}
