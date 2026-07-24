using System.Reflection;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Framework;

partial class FrameworkOptions
{
    /// <summary>
    /// Additional assemblies registered as application parts — controllers within them are mapped to requests.
    /// </summary>
    public Assembly[] ApplicationParts = null;

    /// <summary>
    /// Custom Razor view location formats appended to the default search paths.
    /// </summary>
    /// <example>
    /// <code>
    /// options.ViewLocations = new[] { "~/Pages/{1}/{0}.cshtml" };
    /// </code>
    /// </example>
    public string[] ViewLocations = null;

    /// <summary>
    /// Generates a single Data Protection key file in the parent of <c>RootPath</c>, valid for 100 years.
    /// Required for <c>EncryptUsingKeyRing</c> and <c>DecryptUsingKeyRing</c> and cookie encryption across instances. Default: false.
    /// </summary>
    /// <remarks>
    /// All app instances must share the same key file. Store it outside source control and distribute via pipeline.
    /// </remarks>
    public bool UseDataProtectionPolicy = false;

    /// <summary>
    /// Registers response caching services and middleware. Default: true.
    /// </summary>
    public bool UseResponseCaching = true;

    /// <summary>
    /// Custom view location expander for advanced Razor view resolution.
    /// Use <c>ViewLocations</c> instead for simple path additions.
    /// </summary>
    public IViewLocationExpander ViewLocationExpander = null;
}