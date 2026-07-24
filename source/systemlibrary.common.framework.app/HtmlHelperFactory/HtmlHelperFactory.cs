using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Builds <c>IHtmlHelper</c> instances outside of a Razor view context.
/// </summary>
public class HtmlHelperFactory
{
    //Creds to: https://stackoverflow.com/questions/42039269/create-custom-html-helper-in-asp-net-core/51466436#51466436

    static ModelStateDictionary ModelStateDictionary = new ModelStateDictionary();
    static HtmlHelperOptions HtmlHelperOptions = new HtmlHelperOptions();
    static ControllerActionDescriptor ControllerActionDescriptor = new ControllerActionDescriptor();
    static DummyIndexView DummyIndex = new DummyIndexView();
    static ITempDataProvider TempDataProvider;

    /// <summary>
    /// Returns an <c>IHtmlHelper&lt;T&gt;</c> contextualized with the current HTTP request.
    /// </summary>
    /// <remarks>
    /// Html helpers relying on a real view path (LabelFor, EditorFor, etc.) may not render correctly<br/>
    /// outside a view context — a dummy index view is used internally.
    /// </remarks>
    /// <example>
    /// <code>
    /// var html = HtmlHelperFactory.Build&lt;MyViewModel&gt;();
    /// </code>
    /// </example>
    public static IHtmlHelper<T> Build<T>() where T : class
    {
        var viewContext = GetViewContext<T>();

        var htmlHelper = ServiceInstance.Get<IHtmlHelper<T>>();

        if (htmlHelper != null)
            ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }


    /// <summary>
    /// Returns a default <c>IHtmlHelper</c> contextualized with the current HTTP request.
    /// </summary>
    /// <example>
    /// <code>
    /// var html = HtmlHelperFactory.Build();
    /// </code>
    /// </example>
    public static IHtmlHelper Build()
    {
        var viewContext = GetViewContext();

        var htmlHelper = ServiceInstance.Get<IHtmlHelper>();

        if (htmlHelper != null)
            ((IViewContextAware)htmlHelper).Contextualize(viewContext);

        return htmlHelper;
    }

    static ViewContext GetViewContext<T>()
    {
        var httpContext = HttpContextInstance.Current;

        var modelMetadataProvider = httpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary<T>(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(httpContext, viewData);
    }

    static ViewContext GetViewContext()
    {
        var httpContext = HttpContextInstance.Current;

        var modelMetadataProvider = httpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var viewData = new ViewDataDictionary(modelMetadataProvider, ModelStateDictionary);

        return GetViewContext(httpContext, viewData);
    }

    static ViewContext GetViewContext(HttpContext httpContext, ViewDataDictionary viewData)
    {
        TempDataProvider ??= httpContext.RequestServices.GetRequiredService<ITempDataProvider>();

        var tempData = new TempDataDictionary(httpContext, TempDataProvider);

        // NOTE: Commented out, using TextWriter.Null, for performance (cpu ticks)
        // which ruins LabelFor and similar methods, but those should not be invoked outside a view context anyways...
        // var writer = new StringWriter();

        return new ViewContext(
            new ActionContext(httpContext, httpContext.GetRouteData(), ControllerActionDescriptor),
            DummyIndex,
            viewData,
            tempData,
            TextWriter.Null, // writer,
            HtmlHelperOptions
        );
    }

    internal class DummyIndexView : IView
    {
        public Task RenderAsync(ViewContext context) => context.Writer.FlushAsync();


        //public Task RenderAsync(ViewContext context)
        //{
        //    return Task.CompletedTask;
        //}

        public string Path => "Index";
    }

}