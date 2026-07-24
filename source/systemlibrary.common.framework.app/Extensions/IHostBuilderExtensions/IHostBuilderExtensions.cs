using Microsoft.Extensions.Hosting;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for configuring framework options on <c>IHostBuilder</c>.
/// </summary>
public static class IHostBuilderExtensions
{
    /// <summary>
    /// Passes a pre-built <c>FrameworkOptions</c> instance to the framework.
    /// </summary>
    public static IHostBuilder ConfigureFrameworkOptions(this IHostBuilder builder, FrameworkOptions options)
    {
        FrameworkOptionsInstance.Configure(options);

        return builder;
    }

    /// <summary>
    /// Configures framework options via an action delegate on your own <c>IHostBuilder</c>.<br/>
    /// Use when not calling <c>Application.Start</c> but still need to pass options to the framework.
    /// </summary>
    /// <example>
    /// <code>
    /// Host.CreateDefaultBuilder()
    ///     .ConfigureFrameworkOptions(options =>
    ///     {
    ///         options.SomeOption = true;
    ///     });
    /// </code>
    /// </example>
    public static IHostBuilder ConfigureFrameworkOptions(this IHostBuilder builder, Action<FrameworkOptions> configure)
    {
        var options = new FrameworkOptions();

        configure(options);

        FrameworkOptionsInstance.Configure(options);

        return builder;
    }
}
