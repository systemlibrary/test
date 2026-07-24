using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Framework.Extensions;

static partial class IServiceCollectionextensions
{
    internal static IServiceCollection AddCommonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        serviceCollection.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        return serviceCollection;
    }
}
