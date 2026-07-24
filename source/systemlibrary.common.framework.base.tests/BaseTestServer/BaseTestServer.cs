using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SystemLibrary.Common.Framework.App.Extensions;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

// <summary>
/// Abstract base class for integration testing ASP.NET controllers using ASP.NET's in-memory TestServer — no real port, no Kestrel.<br/>
/// Send requests via <c>GetResponseText</c> or <c>GetResponseMessage</c> and the full middleware pipeline executes against your controllers.
/// </summary>
public abstract partial class BaseTestServer : BaseTest
{
    protected FrameworkOptions Options { get; init; } = new FrameworkOptions();
    protected Action<IServiceCollection> ConfigureServices { get; init; }
    protected Action<IApplicationBuilder> ConfigureApp { get; init; }

    static object ClientLock = new object();

    TestServer _Server;
    HttpClient _Client;

    /// <summary>
    /// Lazily initializes and returns the in-memory HttpClient backed by ASP.NET TestServer.<br/>
    /// The full framework middleware pipeline is configured on first access.
    /// </summary>
    protected HttpClient Client
    {
        get
        {
            if (_Client == null)
            {
                lock (ClientLock)
                {
                    if (_Client != null) return Client;

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

                    builder.ConfigureFrameworkOptions(Options);

                    builder.Services.AddFrameworkServices<LogFileWriter>();

                    ConfigureServices?.Invoke(builder.Services);

                    builder.WebHost.UseTestServer();

                    var app = builder.Build();

                    app.UseFrameworkBeginMiddlewares();
                    app.UseFrameworkEndMiddlewares();

                    ConfigureApp?.Invoke(app);

                    app.Run(async context =>
                    {
                        await WriteResponseAsync(context);
                    });

                    app.StartAsync().GetAwaiter().GetResult();

                    _Server = app.GetTestServer();

                    _Client = _Server.CreateClient();
                }
            }

            return _Client;
        }
    }

    /// <summary>
    /// Disposes the current TestServer and client, forcing a rebuild on next request.<br/>
    /// Use when reconfiguring services or middleware between tests.
    /// </summary>
    /// <example>
    /// <code>
    /// RebuildClient();
    /// </code>
    /// </example>
    protected void RebuildClient()
    {
        lock (ClientLock)
        {
            _Client = null;
            _Server?.Dispose();
            _Server = null;
        }
    }
}