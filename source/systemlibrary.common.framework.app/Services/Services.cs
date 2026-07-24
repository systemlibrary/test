using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Static accessor for resolving services from the application's service provider.
/// </summary>
public static class ServiceInstance
{
    /// <summary>
    /// Get service as T or default if not found
    /// </summary>
    public static T Get<T>() where T : class
    {
        return ServicesInstance.ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Get service as T or throw exception if not found
    /// </summary>
    public static T GetRequired<T>() where T : class
    {
        return ServicesInstance.ServiceProvider.GetRequiredService<T>();
    }
}