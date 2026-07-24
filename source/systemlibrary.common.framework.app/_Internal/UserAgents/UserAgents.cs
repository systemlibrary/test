using System.Text.RegularExpressions;

namespace SystemLibrary.Common.Framework.App;

internal class UserAgents
{
    internal static string[] Bots =
    {
        // Common Search Engine Bots
        "Googlebot", "Bingbot", "Baiduspider", "YandexBot", "DuckDuckBot",
        "AhrefsBot", "SemrushBot", "MJ12bot", "Applebot", "Sogou",
        "Exabot", "facebookexternalhit", "Twitterbot",
    };

    internal static string[] Scripts =
    {
        // Common Automation Scripts (used for scraping and attacks)
        "Java/", "Scrapy", "Lynx/",
        "Bot/", "crawler/", "spider/",
        "python-requests", "Go-http-client",
        "-perl", "zgrab" , "masscan"
        // NOTE: deliberately not registering 'curl' as 'Scripts' is used in blacklisting and ratelimiting requests
    };

    internal static Regex BotsRegex;
    internal static Regex ScriptsRegex;

    static UserAgents()
    {
        BotsRegex = BuildRegex(Bots);
        ScriptsRegex = BuildRegex(Scripts);
    }

    static Regex BuildRegex(string[] values)
    {
        var pattern = string.Join("|", values.Select(Regex.Escape));
        return new Regex(
            "(" + pattern + ")",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }

}
