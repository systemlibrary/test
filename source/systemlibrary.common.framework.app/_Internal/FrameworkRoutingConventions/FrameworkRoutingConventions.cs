using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

using SystemLibrary.Common.Framework.App;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal class FrameworkRoutingConventions : IApplicationModelConvention
{
    List<string> ControllerRoutes = new();

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (!IsEligibleForExtendedActionConstraint(controller)) continue;

            ExtendActionConstraint(controller);

            ControllerRoutes.Clear();

            var controllerType = controller.ControllerType;

            if (!controllerType.Inherits(typeof(ControllerBase))) continue;

            MainAssembly.AddViewNamespace(controllerType.Namespace);

            if (!IsEligibleForAnyRouteConventions(controller, controllerType)) continue;

            ApplyApiPathConvention(controller, controllerType);
        }
    }

    bool IsEligibleForExtendedActionConstraint(ControllerModel model)
    {
        var controllerType = model.ControllerType;
        if (controllerType == typeof(ControllerBase)) return false;
        if (controllerType == typeof(BaseApiController)) return false;
        if (!(model.Actions?.Count > 0)) return false;

        return true;
    }
    
    void ExtendActionConstraint(ControllerModel controller)
    {
        foreach (var action in controller.Actions)
        {
            foreach (var selector in action.Selectors)
            {
                selector.ActionConstraints.Add(new ExtendedActionConstraint());
            }
        }
    }

    bool IsEligibleForAnyRouteConventions(ControllerModel controller, TypeInfo controllerType)
    {
        var routeTemplate = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;
        if (routeTemplate.Is()) return false;

        var apiControllerAttribute = controllerType.GetCustomAttribute<ApiControllerAttribute>();
        if (apiControllerAttribute != null) return false;

        return true;
    }

    void ApplyApiPathConvention(ControllerModel controller, TypeInfo controllerType)
    {
        if (!controllerType.Inherits(typeof(BaseApiController))) return;

        var template = "[controller]/[action]";

        AddApiRoute(controller, template);

        AddNamespaceRoute(controller.ControllerType, controller, template);
    }

    void ApplyNormalControllerConventions(ControllerModel controller, TypeInfo controllerType)
    {
        if (controllerType.Inherits(typeof(BaseApiController))) return;

        if (controllerType.Inherits(typeof(Controller))) return;

        if (!controller.ControllerName.EndsWith("Api", StringComparison.OrdinalIgnoreCase)) return;

        var template = "[controller]/[action]/{id?}";

        AddApiRoute(controller, template);

        AddNamespaceRoute(controller.ControllerType, controller, template);
    }


    void AddNamespaceRoute(TypeInfo controllerTypeInfo, ControllerModel controller, string template)
    {
        var namespacePath = GetNamespaceAsPath(controllerTypeInfo.AsType());

        if (namespacePath.IsNot()) return;

        ApplyRoute(controller, $"{namespacePath}/" + template);
    }

    void AddApiRoute(ControllerModel controller, string template)
    {
        var hasExplicitRoute = controller.Actions.Any(a => a.Attributes.OfType<RouteAttribute>().Any());
        if (hasExplicitRoute) return;

        var route = "api/" + template;

        ApplyRoute(controller, route);

        ApplyShortRoute(controller, route);
    }

    void ApplyShortRoute(ControllerModel controller, string route)
    {
        if (!controller.ControllerName.EndsWith("Api")) return;

        if (!route.Contains("[controller]")) return;

        var shortname = controller.ControllerName[..^3];

        var shortnamePath = route.Replace("[controller]", shortname);

        ApplyRoute(controller, shortnamePath);
    }

    void ApplyRoute(ControllerModel controller, string route)
    {
        if (ControllerRoutes.Contains(route))
        {
            FrameworkLog.Debug("[Route] Route " + route + " already registered, continuing...");
            return;
        }

        ControllerRoutes.Add(route);

        controller.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(route))
        });
    }

    internal static string GetNamespaceAsPath(Type controllerType)
    {
        if (controllerType == null) return "";

        var fullNamespace = controllerType.Namespace;
        if (fullNamespace == null) return "";

        var rootNamespace = controllerType.Assembly?.GetName()?.Name;
        if (fullNamespace == rootNamespace) return "";

        var fullSegments = fullNamespace.Split('.');
        var rootSegments = rootNamespace.Split('.');

        int index = 0;
        while (index < fullSegments.Length && index < rootSegments.Length && fullSegments[index].Equals(rootSegments[index], StringComparison.OrdinalIgnoreCase))
            index++;

        var remainingSegments = fullSegments.Skip(index);

        if (remainingSegments.IsNot()) return "";

        var camelCaseSegments = remainingSegments.Select(segment => ToCamelCase(segment)).ToArray();

        return string.Join("/", camelCaseSegments);
    }

    static string ToCamelCase(string name)
    {
        if (name.Length <= 1)
            return name.ToLower();

        return char.ToLower(name[0]) + name.Substring(1);
    }
}

