using Asm = System.Reflection.Assembly;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Static class for scanning types across whitelisted loaded assemblies.
/// </summary>
/// <remarks>
/// Skips known third-party and framework assemblies — Microsoft, System, EntityFramework, AWS, Serilog, NUnit, xUnit, Newtonsoft, Autofac, AutoMapper, Dapper, and others.
/// </remarks>
public static partial class Assemblies
{
    internal static string[] BlacklistedAssemblyNames =>
    [
        "Microsoft",
        "Docfx",
        "Windows",
        "MSBuild",
        "System.",
        "Castle.",
        "Owin",
        "StructureMap",
        "EntityFramework",
        "EPiServer",
        "Umbraco",
        "Newtonsoft",
        "Swashbuckle",
        "RestSharp",
        "GraphQL",
        "log4net",
        "MSTest",
        "VSTest",
        "Serilog",
        "nlog",
        "ElasticSearch",
        "Elasticsearch",
        "Remotion",
        "YamlDotNet",
        "Antlr",
        "ClearScript",
        "nunit",
        "AWS",
        "SharpZipLib",
        "HtmlAgilityPack",
        "Azure.",
        "JavaScriptEngineSwitcher",
        "NuGet",
        "Salesforce",
        "React",
        "moq",
        "Moq",
        "automapper",
        "AutoMapper",
        "Autofac",
        "Dapper",
        "SystemLibrary.Common.Framework.App",
        "SystemLibrary.Common.Framework.Base.Tests",
        "SystemLibrary.Common.Framework.Cache",
        "SystemLibrary.Common.Framework.Client",
        "SystemLibrary.Common.Framework.Json",
        "SystemLibrary.Common.Framework.Net",
        "SystemLibrary.Common.Framework.Ssr",
        "testhost",
        "netstandard",
        "Anonymously Hosted",
        "DynamicContentModelsAssembly",
        "nunit.",
        "xunit.",
        "Polly.",
        "runtime.win",
        "FluentValidation.",
        "FluentAssertions.",
        "StackExchange.",
        "AutoFixture.",
        "Modernizr.",
        "DocumentFormat.OpenXml",
        "NLog.",
        "IdentityModel.",
        "coverlet.",
        "MediatR.",
        "StyleCop.",
        "Hangfire.",
        "Pipelines.",
        "NUnit3TestAdapter.",
        "Npgsql.",
        "Humanizer.Core",
        "NSubstitute",
        "NJsonSchema",
        "bootstrap",
        "SendGrid",
        "Portable.BouncyCastle.",
        "RabbitMQ.",
        "SQLitePCLRaw.",
        "CsvHelper.",
        "Elasticsearch.",
        "MongoDB.",
        "WebGrease.",
        "Google.",
        "jQuery.",
        "SharpCompress.",
        "JetBrains.",
        "NodaTime.",
        "Selenium.",
        "CommandLineParser.",
        "SendGrid.",
        "WebActivatorEx.",
        "MessagePack",
        "MailKit",
        "protobuf-net.",
        "Unity.",
        "MySql.Data.",
        "Xamarin.",
        "CommonServiceLocator.",
        "NuGet.Packaging.",
        "IdentityServer4.",
        "FluentEmail",
        "Grpc.",
        "Volo.Abp.",
        "OrchardCore."
    ];

    /// <summary>
    /// Returns all types inheriting from or implementing <c>TClassType</c> across whitelisted assemblies.
    /// </summary>
    /// <example>
    /// <code>
    /// public class Car : IVehicle { }
    /// 
    /// var vehicles = Assemblies.FindAllTypesInheriting&lt;IVehicle&gt;();
    /// // returns Car and all other types implementing IVehicle
    /// </code>
    /// </example>
    public static IEnumerable<Type> FindAllTypesInheriting<TClassType>() where TClassType : class
    {
        return FindTypesInheriting(typeof(TClassType));
    }

    /// <summary>
    /// Returns all types inheriting from or implementing <c>TClassType</c> that also have <c>TAttributeType</c> applied.
    /// </summary>
    /// <example>
    /// <code>
    /// [Name]
    /// public class Car : IVehicle { }
    /// 
    /// var vehicles = Assemblies.FindAllTypesInheritingWithAttribute&lt;IVehicle, NameAttribute&gt;();
    /// // returns Car and all other types implementing IVehicle with [Name] applied
    /// </code>
    /// </example>
    public static IEnumerable<Type> FindAllTypesInheritingWithAttribute<TClassType, TAttributeType>()
        where TClassType : class
        where TAttributeType : Attribute
    {
        return FindTypesInheriting(typeof(TClassType), typeof(TAttributeType));
    }

    /// <summary>
    /// Returns all types across whitelisted assemblies that have <c>TAttributeType</c> applied.
    /// </summary>
    /// <example>
    /// <code>
    /// [Name]
    /// public class Car : IVehicle { }
    /// 
    /// var types = Assemblies.FindAllTypesWithAttribute&lt;NameAttribute&gt;();
    /// // returns Car and all other types with [Name] applied
    /// </code>
    /// </example>
    public static IEnumerable<Type> FindAllTypesWithAttribute<TAttributeType>() where TAttributeType : Attribute
    {
        return Types
          .Where(type =>
              type.IsClass &&
              !type.IsInterface && 
              type.IsDefined(typeof(TAttributeType), false)
        );
    }

    /// <summary>
    /// Returns the content of an embedded resource as a string. Defaults to the calling assembly if none specified.
    /// </summary>
    /// <example>
    /// <code>
    /// // ~/Folder/file.txt marked as 'Embedded Resource' in build action
    /// var text = Assemblies.GetEmbeddedResource("Folder/file.txt");
    /// </code>
    /// </example>
    public static string GetEmbeddedResource(string relativeName, Asm assembly = null)
    {
        return ReadEmbeddedResourceAsString(relativeName, assembly ?? Asm.GetCallingAssembly());
    }

    /// <summary>
    /// Returns the content of an embedded resource as a byte array. Defaults to the calling assembly if none specified.
    /// </summary>
    /// <example>
    /// <code>
    /// // ~/Folder/image.png marked as 'Embedded Resource' in build action
    /// var bytes = Assemblies.GetEmbeddedResourceAsBytes("Folder/image.png");
    /// </code>
    /// </example>
    public static byte[] GetEmbeddedResourceAsBytes(string relativeName, Asm assembly = null)
    {
        return ReadEmbeddedResourceAsBytes(relativeName, assembly ?? Asm.GetCallingAssembly());
    }
}