using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void UseXForwardedHeaders(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
    }
}