using Microsoft.AspNetCore.Builder;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IApplicationBuilderExtensions
{
    internal static void UseCompression(this IApplicationBuilder app)
    {
        app.UseWhen((context) => Compress.IsEligibleForCompression(context), appCompression =>
        {
            appCompression.UseResponseCompression();
        });
    }
}