using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework.App;

internal static class StaticFilePolicyMiddleware
{
    // TODO: Extend to reading Storage Account in Azure through Default Creds or a Storage Account access key (con string)
    internal static RequestDelegate Build(RequestDelegate next)
    {
        var env = ServicesInstance.ServiceProvider.GetService<IWebHostEnvironment>();

        var contentRootPath = SystemLibrary.Common.Framework.EnvironmentConfig.ContentRootPath;

        var webRootPath = env?.WebRootPath;

        var fileMiddlewares = new List<RequestDelegate>();

        if (webRootPath.Is())
        {
            if (Directory.Exists(webRootPath))
            {
                var builder = new ApplicationBuilder(ServicesInstance.ServiceProvider);

                var options = GetStaticFileOptions(webRootPath, "");

                builder.UseStaticFiles(options);

                fileMiddlewares.Add(builder.Build());
            }
            else
            {
                Log.Warning("StaticFilePolicy is enabled and IWebHostEnvironment.WebRootPath is set, but the folder do not exist: " + webRootPath + ". Create the folder or unset WebRootPath. Continuing...");
            }
        }

        var assetFolders = new[] { "public", "static", "dist", "frontend", "assets", "files" };

        foreach (var assetFolder in assetFolders)
        {
            var assetFolderFullPath = Path.Combine(contentRootPath, assetFolder);

            try
            {
                if (!Directory.Exists(assetFolderFullPath)) continue;
            }
            catch
            {
                FrameworkLog.Debug(assetFolderFullPath + " threw on Directory.Exists(), continuing...");
                continue;
            }

            var builder = new ApplicationBuilder(ServicesInstance.ServiceProvider);

            var requestSegment = $"/{assetFolder}";

            var options = GetStaticFileOptions(assetFolderFullPath, requestSegment);

            builder.UseStaticFiles(options);

            fileMiddlewares.Add(builder.Build());
        }

        return async context =>
        {
            if (context.Request.Method != HttpMethods.Get && context.Request.Method != HttpMethods.Head)
            {
                await next(context);
                return;
            }

            foreach (var middleware in fileMiddlewares)
            {
                await middleware(context);

                if (context.Response.HasStarted) return;
            }

            await next(context);
        };
    }

    static StaticFileOptions GetStaticFileOptions(string fullPath, string requestPath)
    {
        return new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fullPath),
            RequestPath = requestPath,
            RedirectToAppendTrailingSlash = false,
            ServeUnknownFileTypes = true,
            HttpsCompression = HttpsCompressionMode.Compress,
            OnPrepareResponse = ctx =>
            {
                if (!ctx.Context.Response.Headers.ContainsKey("Cache-Control"))
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={FrameworkOptionsInstance.Current.StaticFilesClientCacheSeconds}, immutable");
            }
        };
    }
}