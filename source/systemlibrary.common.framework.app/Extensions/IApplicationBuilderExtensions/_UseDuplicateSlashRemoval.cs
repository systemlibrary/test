using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void UseDuplicateSlashRemoval(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var path = context?.Request?.Path.Value;

            if(path != null && path.Length > 2 && path[1] == '/')
            {
                if(path.StartsWith("//"))
                {
                    context.Request.Path = new PathString(path.Substring(1));
                }
            }
            await next();
        });
    }
}