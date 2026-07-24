using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Bootstrap;

static class ServicesBoot
{
    internal static void Strap() { }

    static ServicesBoot()
    {
        // NOTE:
        // Creates fallbacks so framework features can operate before
        // the application's real service container is built.
        // The application's IServiceCollection registrations overwrites these defaults through 'AddFrameworkServices()'
        ServicesInstance.ServiceProvider = GetDefaultServiceProvider();

        ServicesInstance.DataProtector = GetDefaultDataProtector();
    }

    static IServiceProvider GetDefaultServiceProvider()
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

    static IDataProtector GetDefaultDataProtector()
    {
        var provider = ServicesInstance.ServiceProvider.GetDataProtectionProvider();

        return provider.CreateProtector(AppInstance.Name ?? "");
    }

    class DummyTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) 
        {
        }
    }
}
