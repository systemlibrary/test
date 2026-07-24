using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for configuring framework options on <c>IWebHostBuilder</c>.
/// </summary>
public static class IWebHostBuilderExtensions
{
    /// <summary>
    /// Passes a pre-built <c>FrameworkOptions</c> instance to the framework.
    /// </summary>
    public static void ConfigureFrameworkOptions(this WebApplicationBuilder builder, FrameworkOptions options)
    {
        FrameworkOptionsInstance.Configure(options);
    }

    /// <summary>
    /// Configures framework options via an action delegate on your own <c>IWebHostBuilder</c>.<br/>
    /// Use when not calling <c>Application.Start</c> but still need to pass options to the framework.
    /// </summary>
    /// <example>
    /// <code>
    /// webHostBuilder.ConfigureFrameworkOptions(options =>
    /// {
    ///     options.SomeOption = true;
    /// });
    /// </code>
    /// </example>
    public static IWebHostBuilder ConfigureFrameworkOptions(this IWebHostBuilder builder, Action<FrameworkOptions> configure)
    {
        var options = new FrameworkOptions();

        configure(options);

        FrameworkOptionsInstance.Configure(options);

        return builder;
    }
}
