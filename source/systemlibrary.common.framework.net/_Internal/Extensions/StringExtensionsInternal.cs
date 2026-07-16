using System.Runtime.InteropServices;

namespace SystemLibrary.Common.Framework;

internal static class StringExtensionsInternal
{
    internal static string _ToOsFriendlyPath(this string path)
    {
        if (path.IsNot()) return path;

        if (path.Contains("\\"))
            path = path.Replace("\\", "/");

        if (path.EndsWith('/'))
            path = path.TrimEnd('/');

        if (path.Length > 4)
        {
            var duplicated = path.IndexOf("//", 4);
            if (duplicated > -1)
            {
                var prefix = path.Substring(0, 4);
                var rest = path.Substring(4).Replace("//", "/");
                path = prefix + rest;
            }
        }

        if (!path.StartsWith("%HOME%"))
            return path;

        return path
            .Replace("%HOME%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
            .Replace("\\", "/");
    }

    internal static bool _PathEndsWith(this string path, string end)
    {
        if (path == null || end == null) return false;

        if (path.EndsWith(end, StringComparison.OrdinalIgnoreCase))
            return true;

        return end.Contains('/') &&
            path.Contains("\\") &&
            path.Replace('\\', '/').EndsWith(end, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool _PathContains(this string path, string contains)
    {
        if (path == null || contains == null) return false;

        if (path.Contains(contains, StringComparison.OrdinalIgnoreCase)) return true;

        return contains.Contains('/') &&
               path.Contains("\\") &&
               path.Replace("\\", "/").Contains(contains, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool _IsJson(this string data)
    {
        if (data == null || data.Length <= 1) return false;

        if (data[0] == '{' || data[0] == '[') return true;

        return data.Contains("{") || data.Contains("[");
    }

    internal static bool _IsXml(this string data)
    {
        if (data == null || data.Length <= 1) return false;

        return data[0] == '<' ||
            data[^1] == '>' ||
            data.EndsWith("> ") ||
            data.EndsWith(">" + Environment.NewLine);
    }
}
