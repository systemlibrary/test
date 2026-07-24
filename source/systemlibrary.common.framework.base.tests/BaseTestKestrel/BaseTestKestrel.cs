using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Abstract base class for integration testing ASP.NET apps without Kestrel network overhead.<br/>
/// Maps public methods ending in <c>__Action</c> as in-memory HTTP endpoints on a local port.
/// </summary>
/// <remarks>
/// The host starts once per port and is reused across all tests on that port.<br/>
/// Multiple derived classes sharing the same <c>Port</c> share the same WebApplication instance.
/// </remarks>
public abstract partial class BaseTestKestrel : BaseTest
{
    const string Url = "http://localhost:";

    protected FrameworkOptions Options { get; init; } = new FrameworkOptions();
    protected Action<IServiceCollection> ConfigureServices { get; init; }
    protected Action<IApplicationBuilder> ConfigureApp { get; init; }

    /// <summary>
    /// Invoked during host startup to configure logging. Note: log output may not appear in all test runners.
    /// </summary>
    protected Action<WebApplicationBuilder, ILoggingBuilder> ConfigureLogging { get; init; }

    /// <summary>
    /// Port the host listens on. Each derived class must use a unique port.
    /// </summary>
    protected int Port { get; init; } = 50002;

    static object HostLock = new object();

    static Dictionary<int, WebApplication> _Apps = new Dictionary<int, WebApplication>();

    HttpClient Client
    {
        get
        {
            if (!_Apps.ContainsKey(Port))
            {
                lock (HostLock)
                {
                    if (_Apps.ContainsKey(Port))
                        return new HttpClient { BaseAddress = new Uri($"{Url}{Port}") };

                    Options.UseHttpsRedirection = false;

                    var args = Environment.GetCommandLineArgs();

                    var builder = WebApplication.CreateBuilder(args);

                    // TODO: Not sufficient enough as "LogToStd", the STD ERR/OUT is not being loaded/ran/shown in most test environments, so you dont see the log output if you forward Log module to write to STD
                    // OR STD ERR/OUT in TestServer does not follow the log level thresholds, but it might show exceptions (cant remember)
                    var appSettingsPath = AppContext.BaseDirectory + "appsettings.json";

                    if (!File.Exists(appSettingsPath))
                    {
                        appSettingsPath = AppContext.BaseDirectory + "Configs/appsettings.json";
                        if (!File.Exists(appSettingsPath))
                            appSettingsPath = AppContext.BaseDirectory + "Configurations/appsettings.json";
                    }

                    if (File.Exists(appSettingsPath))
                    {
                        builder.Configuration.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false);
                    }

                    ConfigureLogging?.Invoke(builder, builder.Logging);

                    builder.ConfigureFrameworkOptions(Options);

                    builder.Services.AddFrameworkServices<LogFileWriter>();

                    ConfigureServices?.Invoke(builder.Services);

                    builder.WebHost.UseKestrel();
                    builder.WebHost.UseUrls($"{Url}{Port}");

                    var app = builder.Build();

                    app.UseFrameworkBeginMiddlewares();
                    app.UseFrameworkEndMiddlewares();

                    ConfigureApp?.Invoke(app);

                    MapActions(app);

                    app.Run(async context =>
                    {
                        if (context.Response.StatusCode < 1 || context.Response.StatusCode == 200)
                            context.Response.StatusCode = 404;

                        var methodName = context.Request?.Path.Value;

                        if (methodName.Is() && methodName.Contains("/"))
                        {
                            var methods = methodName.Split('/', StringSplitOptions.RemoveEmptyEntries);
                            if (methods.Length > 0)
                            {
                                var tmp = methods[^1];
                                if (tmp.Is())
                                    methodName = tmp;
                            }
                        }

                        await context.Response.WriteAsync(
                            $"[404] Missing action for '{context.Request?.Path.Value}'. " +
                            $"Expected a public method named '{methodName}__Action'. " +
                            "No middleware responded."
                        );
                    });

                    _Apps.Add(Port, app);

                    Task.Run(async () =>
                    {
                        await app.RunAsync();
                    });
                }
            }
            return new HttpClient { BaseAddress = new Uri($"{Url}{Port}") };
        }
    }

    /// <summary>
    /// Starts the host by triggering WebApplication initialization on the configured port.
    /// </summary>
    /// <remarks>
    /// Subsequent calls on the same port are no-ops.
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void Initialize() => HostStart();
    /// </code>
    /// </example>
    protected void HostStart(string outputLogPath = null)
    {
        Console.WriteLine("Listening on http://localhost:" + Port + outputLogPath);

        var client = Client;
        if (client != null)
        {
            client.Dispose();
            client = null;
        }
    }

    /// <summary>
    /// Stops the running WebApplication on the current port.
    /// </summary>
    /// <example>
    /// <code>
    /// await HostStop();
    /// </code>
    /// </example>
    protected async Task HostStop()
    {
        await _Apps[Port]?.StopAsync();

        _Apps.Remove(Port);
    }

    /// <summary>
    /// Maps all public methods ending in <c>__Action</c> as GET endpoints.
    /// Action methods must return <c>string</c> and accept either no parameters or a single <c>string</c> (raw querystring).
    /// </summary>
    /// <remarks>
    /// Exceptions inside an action are caught, serialized as <c>{"error":"..."}</c>, and returned as HTTP 500.
    /// </remarks>
    /// <example>
    /// <code>
    /// public string GetUser__Action() => user.Json();
    /// public string GetUser__Action(string query) => user.Json(); // query = "?id=1"
    /// </code>
    /// </example>
    protected void MapActions(WebApplication app)
    {
        var methods = this.GetType().GetMethods();

        foreach (var method in methods)
        {
            var methodName = method.Name;

            var parameters = method.GetParameters();

            if (!methodName.EndsWith("__Action")) continue;

            app.Map("/" + methodName, runner =>
            {
                runner.Run(async context =>
                {
                    string json;

                    try
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            json = (string)method.Invoke(this, null);
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                        {
                            var query = context.Request.QueryString.Value;
                            json = (string)method.Invoke(this, new object[] { query });
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                methodName + " must have either no parameters or a single string parameter");
                        }

                        context.Response.StatusCode = 200;
                    }
                    catch (Exception ex)
                    {
                        json = "{\"error\":" + "\"" + ex.ToString() + "\"}";

                        context.Response.StatusCode = 500;
                        context.Response.Headers.TryAdd("Error", ex.Message);

                        global::Log.Error(ex);
                    }

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(json ?? "");
                });
            });
        }
    }
}