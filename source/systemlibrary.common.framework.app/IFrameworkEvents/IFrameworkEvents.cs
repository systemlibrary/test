using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Hooks into the application startup pipeline. All methods have default empty implementations.
/// </summary>
public interface IFrameworkHooks
{
    /// <summary>
    /// Called once during service registration.
    /// </summary>
    void OnConfigureServices(IServiceCollection services, EnvironmentType env) { }

    /// <summary>
    /// Called at the start of each branch pipeline — Static, Api or Web.
    /// </summary>
    void OnPipelineBegin(IApplicationBuilder branch, EnvironmentType env) { }

    /// <summary>
    /// Called before endpoints are mapped on each branch.
    /// </summary>
    void OnBeforeEndpoints(IApplicationBuilder branch, BranchType branchType, EnvironmentType env) { }

    /// <summary>
    /// Called after endpoints are mapped on each branch.
    /// </summary>
    void OnAfterEndpoints(IApplicationBuilder branch, BranchType branchType, EnvironmentType env) { }

    /// <summary>
    /// Called at the end of each branch pipeline.
    /// </summary>
    void OnPipelineEnd(IApplicationBuilder branch, BranchType branchType, EnvironmentType env) { }

    /// <summary>
    /// Called during host construction before it is built.
    /// </summary>
    void OnHostBuilding(IHostBuilder hostBuilder) { }

    /// <summary>
    /// Called immediately after the host is built.
    /// </summary>
    void OnHostBuilt(IHost host) { }

    /// <summary>
    /// Called after the host has started and is ready to receive requests.
    /// </summary>
    void OnHostStarted(IHost host) { }
}