using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Abstract base class for environment configuration with a custom config class and environment enum.
/// Extend this instead of <c>EnvironmentConfig</c> when you need additional properties beyond <c>Name</c>.
/// </summary>
public abstract class EnvironmentConfig<T, TEnvironmentTypeEnum> : Config<T>
    where T : class
    where TEnvironmentTypeEnum : struct, IComparable, IFormattable, IConvertible
{
    TEnvironmentTypeEnum? _EnvironmentType;

    /// <summary>
    /// Returns the current environment as <c>TEnvironmentTypeEnum</c>, or the default enum value if not resolved.
    /// </summary>
    public TEnvironmentTypeEnum EnvironmentType
    {
        get
        {
            if (_EnvironmentType == null) return default;

            return _EnvironmentType.Value;
        }
    }

    string _Name;
    /// <summary>
    /// Returns the current environment name resolved from <c>ASPNETCORE_ENVIRONMENT</c> across all supported hosting scenarios.
    /// Never returns null — falls back to the default <c>TEnvironmentTypeEnum</c> key name.
    /// </summary>
    /// <remarks>
    /// Changing the environment name requires a shell restart (e.g., iisreset).
    /// A transformation file can override <c>Name</c> — if <c>environmentConfig.someEnvName.json</c> contains a <c>name</c> property, that value is returned instead of the environment variable.
    /// Setting this value also updates <c>EnvironmentType</c> — intended for framework use only.
    /// </remarks>
    /// <example>
    /// Test Explorer
    /// <code class="language-xml hljs">
    /// if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///     then: sets 'temp environment' as value
    ///     
    ///     if: 'temp environment' is set, but no transformation is found
    ///         then: sets 'temp environment' as value from 'Configuration Mode' in Visual Studio
    ///
    /// else:
    ///     then: sets 'temp environment' as value from 'Configuration Mode' in Visual Studio
    /// 
    /// if: environmentConfig.json exists
    ///     if transformation file exists for 'temp environment' 
    ///         then: run transformation for environmentConfig.json
    ///     
    ///     if: environmentConfig.json contains 'name' property
    ///         return: 'value'
    /// 
    /// if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///      return: 'value'
    ///     
    /// if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///     return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// Console Application
    /// <code class="language-xml hljs">
    /// if: environmentConfig.json do not exists:
    ///     if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///         return: 'value'
    ///     
    ///     if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///         return: 'value'
    ///     
    /// else if: 
    ///     if: environmentConfig has transformation equal to 'configuration' pass in as argument
    ///         then: run transformation
    ///         
    ///     if: environmentConfig has property 'name'
    ///         return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// DOTNET TEST 'csproj' --configuration 'release|debug|etc..' command
    /// <code class="language-csharp hljs">
    /// if: environmentConfig.json do not exists:
    ///     if: mstest.runsettings contains 'ASPNETCORE_ENVIRONMENT' variable
    ///         return: 'value'
    ///     
    /// if: ASPNETCORE_ENVIRONMENT exists in 'Environment Variables on Windows'
    ///     return: 'value'
    ///     
    /// else if: 
    ///     if: environmentConfig has transformation equal to 'configuration' pass in as argument
    ///         then: run transformation
    ///         
    ///     if: environmentConfig has property 'name'
    ///         return: 'value'
    /// 
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// IISExpress
    /// <code class="language-xml hljs">
    /// if: launchSettings.json exists
    ///     if: command "IISExpress" exists and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: 'ASPNETCORE_ENVIRONMENT' exists
    ///             if: environmentConfig.json exists
    ///                 if: transformation file exists for 'value'
    ///                 then: run transformation
    ///                     
    ///                 if: environmentConfig contains 'name' property  
    ///                 return: 'value'
    ///                 
    ///             return: 'value'
    ///                 
    /// if: 'ASPNETCORE_ENVIRONMENT' exists in web.config
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists for 'value'
    ///         then: run transformation
    ///         
    ///         if: environmentConfig contains 'name' property
    ///             return: 'value'
    ///             
    ///     return: 'value'
    ///         
    /// if: 'ASPNETCORE_ENVIRONMENT' exists as a 'environment variable' in Windows
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists for 'value'
    ///         then: run transformation
    ///         
    ///         if: environmentConfig contains 'name' property
    ///             return: 'value'
    ///             
    ///     return: 'value'
    ///     
    /// if: launchSettings.json exists
    ///     if: "iisSettings" contains "iisExpress" and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exist for 'value'
    ///             then: run transformation
    ///                 
    ///             if: environmentConfig.json contains 'name' property
    ///                 return: 'value'
    ///                         
    ///         return: 'value'
    ///         
    ///     if: "iisSettings" contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exists for 'value'
    ///             then: run transformation
    ///                     
    ///             if: environmentConfig.json contains 'name' property
    ///             return: 'value'
    ///                 
    ///         return: 'value'
    ///          
    ///     if: command "IIS" exists and contains environment variable 'ASPNETCORE_ENVIRONMENT'
    ///         if: environmentConfig.json exists
    ///             if: transformation file exists for 'value'
    ///             then: run transformation
    ///                 
    ///             if: environmentConfig.json contains 'name' property
    ///                 return: 'value'
    ///                 
    /// if: environmentConfig.json exists
    ///     if: environmentConfig.json contains 'name' property
    ///         return: 'value'
    ///                         
    /// return: "" as 'name', never null
    /// </code>
    /// 
    /// IIS
    /// <code class="language-xml hljs">
    /// if: 'ASPNETCORE_ENVIRONMENT' exists in web.config
    ///     if: environmentConfig.json exists
    ///         if: transformation file exists 'value' 
    ///         then: run transformation 
    ///         
    ///         if: environmentConfig.json contains 'name' property
    ///             return: 'value'
    ///         
    ///     return: 'value'
    ///     
    /// if: launchSettings.json exists
    ///     if: "iisSettings" contains "iisExpress"
    ///         if: "iisExpress" contains 'environmentVariables'
    ///             if: 'ASPNETCORE_ENVIRONMENT' exists
    ///                 if: environmentConfig.json exists
    ///                     if: transformation file exist for 'value'
    ///                     then: run transformation
    ///                 
    ///                     if: environmentConfig.json contains 'name' property
    ///                         return: 'value'
    ///                     
    ///     if: "iisSettings" contains 'environmentVariables'
    ///         if: 'ASPNETCORE_ENVIRONMENT' exists
    ///             if: environmentConfig.json exists
    ///                 if: transformation file exist for 'value'
    ///                 then: run transformation
    ///                 
    ///                 if: environmentConfig.json contains 'name' property
    ///                     return: 'value'
    ///                     
    ///     if: "profiles" exists
    ///         if: command "IIS" exists and contains 'environmentVariables'
    ///             if: 'ASPNETCORE_ENVIRONMENT' exists
    ///                 if: environmentConfig.json exists
    ///                     if: trnasformation file exists for 'value'
    ///                     then: transformation is ran
    ///                 
    ///                     if: environmentConfig.json contains 'name' property
    ///                         return: 'value'
    ///                         
    ///                 return: 'value'
    ///                 
    /// return: "" as 'name', never null
    /// </code>
    /// </example>
    public string Name
    {
        get
        {
            if (_Name != null && _Name != "") return _Name;

            _Name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                    ?? 
                    EnvironmentInstance.EnvironmentType.ToString();

            return _Name;
        }
        set
        {
            _Name = value;
            _EnvironmentType = _Name.ToEnum<TEnvironmentTypeEnum>();
        }
    }
}
