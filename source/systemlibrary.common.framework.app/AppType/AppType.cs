namespace SystemLibrary.Common.Framework.App;

/// <summary>
/// Specifies whether the application serves web pages, API endpoints, or runs as a console process.
/// </summary>
/// <example>
/// <code>
/// Application.Start&lt;Hooks&gt;(AppType.Web);
/// </code>
/// </example>
public enum AppType
{
    Console = 0,
    Api = 1,
    Web = 2
}