using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Framework.App;

internal static class BlockRequestMiddleware
{
    static string[] BlockedExtensions = new[]
    {
        ".exe", ".dll", ".iso", ".msi", ".ps1", ".cmd", ".sh", ".bash", ".vbs", ".dmg",
        ".config", ".env", ".ini", ".key", ".pem", ".cshtml", ".cs", ".sql", ".mdf",
        ".bat", ".jar", ".php", ".py", ".pl", ".rb", ".go", ".vb", ".vbs", ".hta", ".bak", ".db", ".pfx", ".crt"
    };

    static int BlockedExtensionsLength = BlockedExtensions.Length;

    public static RequestDelegate Build(RequestDelegate next)
    {
        return context =>
        {
            var path = context?.Request?.Path.Value;
            if (path == null)
                return next(context);

            int length = path.Length;
            if (length <= 6)
                return next(context);

            if (path.StartsWith("/app_data/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/properties/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/configs/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/configurations/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/bin/", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/obj/", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers.TryAdd("Reason", "Access denied by SystemLibrary.Common.Framework");
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }

            if (path[length - 1] == '/')
                return next(context);

            int num = path.LastIndexOf('.', length - 1, 7);
            if (num == -1)
                return next(context);

            if (length - num < 3)
                return next(context);

            if (path.Contains("appsettings.", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers.TryAdd("Reason", "Access denied by SystemLibrary.Common.Framework due to blacklisted path.");
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }

            for (var i = 0; i < BlockedExtensionsLength; i++)
            {
                if (path.EndsWith(BlockedExtensions[i], StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers.TryAdd("Reason", "Access denied by SystemLibrary.Common.Framework due to blacklisted extension.");
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                }
            }

            return next(context);
        };
    }
}