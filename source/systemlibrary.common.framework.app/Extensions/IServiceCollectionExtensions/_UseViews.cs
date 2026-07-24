using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework.App.Extensions;

static partial class IServiceCollectionExtensions
{
    static IServiceCollection UseViews(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(razorViews =>
        {
            razorViews.ViewLocationFormats.Insert(0, "/{1}/{0}.cshtml");

            razorViews.ViewLocationExpanders.Add(new FrameworkViewLocationExpander());

            if (FrameworkOptionsInstance.Current.ViewLocationExpander != null)
                razorViews.ViewLocationExpanders.Add(FrameworkOptionsInstance.Current.ViewLocationExpander);

            if (FrameworkOptionsInstance.Current.ViewLocations != null)
            {
                foreach (var view in FrameworkOptionsInstance.Current.ViewLocations)
                {
                    if (view.Is())
                        razorViews.ViewLocationFormats.Add(view);
                }
            }
        });

        // TODO: Consider allowing embedded views so one can easily create reusable controllers/views across apps
        // WARN: MvcRazorRuntimeCompilationOptions I believe that one is huge in terms of megabytes, making our dependency huge, which slows down build time and if you have a lot of csproj files, it adds up...
        //services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        //{
        //    options.FileProviders.Add(new EmbeddedFileProvider(
        //        typeof(SomeTypeInYourDll).Assembly
        //    ));
        //});

        return services;
    }

    class FrameworkViewLocationExpander : IViewLocationExpander
    {
        static string[] Paths = {
            "/{2}/{1}/{0}.cshtml",
            "/{2}/{1}/{1}.cshtml",

            "/{2}/{1}/Views/{0}.cshtml",
            "/{2}/Views/Shared/{1}/{0}.cshtml",
        };

        static bool HasInitialized = false;

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> locations)
        {
            if (!HasInitialized)
            {
                HasInitialized = true;
                AddViewNamespaces();
            }

            var controller = context.ControllerName;

            var view = context.ViewName;

            for (int i = 0; i < Paths.Length; i++)
            {
                yield return Paths[i];
            }

            foreach (var location in locations.Distinct())
            {
                yield return location;
            }
        }

        static void AddViewNamespaces()
        {
            var viewLocations = new List<string>();

            foreach (var ns in MainAssembly.ViewNamespaces)
            {
                var tmp = ns?.Replace(".", "/");

                if (tmp.Is() && tmp.Length < 512)
                {
                    viewLocations.Add(tmp + "/{1}/{0}.cshtml");
                    viewLocations.Add(tmp + "/{2}/{1}/Views/{0}.cshtml");
                }
            }

            Paths = Paths.Add(viewLocations.ToArray()).Distinct().ToArray();
        }
    }

    class OverloadActionSelector : IActionSelector
    {
        IActionDescriptorCollectionProvider Provider;

        public OverloadActionSelector(IActionDescriptorCollectionProvider provider)
        {
            Provider = provider;
        }

        public ActionDescriptor SelectBestCandidate(RouteContext context, IReadOnlyList<ActionDescriptor> candidates)
        {
            if (candidates.Count <= 1) return candidates.FirstOrDefault();

            var query = context.HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
            var route = context.HttpContext.Request.RouteValues.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? "");

            foreach (var candidate in candidates)
            {
                var method = ((ControllerActionDescriptor)candidate).MethodInfo;

                var parameters = method.GetParameters();

                bool match = parameters.All(p => query.ContainsKey(p.Name) || route.ContainsKey(p.Name) || p.HasDefaultValue);

                if (match)
                    return candidate;
            }

            return candidates.First();
        }

        public IReadOnlyList<ActionDescriptor> SelectCandidates(RouteContext context)
        {
            return Provider.ActionDescriptors.Items;
        }
    }
}