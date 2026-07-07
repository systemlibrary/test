using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SystemLibrary.Common.Framework;

internal static class MainAssembly
{
    static string[] BlacklistedEntryNames =
    {
        "testhost",
        "Microsoft.",
        "System.",
        "Azure.",
        "AWS.",
        "AWSSDK",
        "Serilog"
    };

    public static string FullName;
    public static string Name;
    public static string RootNamespace;
    public static string[] AreaNamespaces;
    public static List<string> ViewNamespaces;

    // NOTE: Falls back to entry assembly if no non-blacklisted assembly is found in the stack
    public static Assembly Current;
    public static Type[] Types;

    static MainAssembly()
    {
        Current = ResolveAssembly();

        Name = Current.GetName()?.Name ?? "";

        FullName = Current.FullName;

        Types = GetTypes();

        RootNamespace = ResolveNamespace(Types);

        AreaNamespaces = ResolveAreaNamespaces(Types);

        ViewNamespaces = new List<string>();
    }

    static Assembly ResolveAssembly()
    {
        var asm = Assembly.GetEntryAssembly();

        var sysLibAsm = Assembly.GetCallingAssembly();

        var name = asm.GetName()?.Name;

        bool IsSelf()
        {
            return asm == sysLibAsm;
        }

        bool IsBlacklisted()
        {
            if (name?.Length == 12 && name.StartsWith("Job-"))
                return true;

            if (name?.EndsWith("Tests") == true) return true;

            return name.StartsWithAny(BlacklistedEntryNames);
        }

        if (!IsSelf() && !IsBlacklisted()) return asm;

        var stack = new StackTrace(skipFrames: 2);

        var i = 0;

        foreach (var frame in stack.GetFrames())
        {
            i++;

            if (i > 12) break;

            asm = frame.GetMethod()?.DeclaringType?.Assembly;

            if (asm == null) continue;

            if (IsSelf()) continue;

            name = asm.GetName()?.Name;

            if (name == null) continue;

            if (IsBlacklisted()) continue;

            break;
        }

        if (i > 12)
        {
            asm = Assembly.GetEntryAssembly();
        }

        return asm;
    }

    static string ResolveNamespace(Type[] types)
    {
        var ns = types
            .Select(t => t.Namespace)
            .FirstOrDefault();

        if (ns.IsNot())
            return Current.GetName().Name;

        return ns;
    }

    static string[] ResolveAreaNamespaces(Type[] types)
    {
        if (types == null || types.Length == 0) return null;

        var list = new List<string>();

        foreach (var type in types)
        {
            var ns = type.Namespace.Replace(RootNamespace, "");

            if (ns.Length > 1)
            {
                ns = ns.TrimStart('.');

                if (!list.Contains(ns))
                    list.Add(ns);
            }
        }
        return list.ToArray();
    }

    static Type[] GetTypes()
    {
        return Current
            .GetTypes()
            .Where(t => (t.IsClass || t.IsEnum)
                && t.IsPublic
                && !t.IsSpecialName // anonymous types
                && t.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
            .ToArray();
    }

    internal static void AddViewNamespace(string ns)
    {
        if (ns.IsNot()) return;

        if (!ns.StartsWith(RootNamespace)) return;

        var tmp = ns.Replace(RootNamespace, "").Replace(".", "/");

        if (tmp.IsNot()) return;

        if (ViewNamespaces.Contains(tmp)) return;

        ViewNamespaces.Add(tmp);
    }
}