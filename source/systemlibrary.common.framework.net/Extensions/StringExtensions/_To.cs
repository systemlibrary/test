using System.Diagnostics;
using System.Reflection;
using System.Text;

using SystemLibrary.Common.Framework.Attributes;
using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Converts a string to <c>TEnum</c>, matching case-insensitively against key name, <c>EnumValue</c> and <c>EnumText</c> attributes.
    /// Returns the default enum value if no match is found.
    /// </summary>
    /// <example>
    /// <code>
    /// enum Color { None, [EnumText("White")] [EnumValue("BlackAndWhite")] Black, Pink }
    ///
    /// "black".ToEnum&lt;Color&gt;();        // Color.Black — key name match
    /// "white".ToEnum&lt;Color&gt;();        // Color.Black — EnumText match
    /// "blackAndWhite".ToEnum&lt;Color&gt;(); // Color.Black — EnumValue match
    /// "brown".ToEnum&lt;Color&gt;();        // Color.None — no match, returns default
    /// </code>
    /// </example>
    public static T ToEnum<T>(this string text) where T : struct, IComparable, IFormattable, IConvertible
    {
        if (text == null) return default(T);

        return (T)text.ToEnum(typeof(T));
    }

    /// <summary>
    /// Converts a string to the specified enum type returned as <c>object</c>.
    /// Matches case-insensitively against key name, <c>EnumValue</c> and <c>EnumText</c> attributes.
    /// Returns the first enum key if no match is found.
    /// </summary>
    public static object ToEnum(this string text, Type enumType)
    {
        object result;

        if (!enumType.IsEnum)
            return Activator.CreateInstance(enumType);

        MemberInfo[] members = null;

        if (text != null && text.Length > 0 && char.IsDigit(text[0]))
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
            {
                members = EnumMembersCached.Cache(enumType, () => enumType.GetMembers(BindingFlags.Public | BindingFlags.Static));
                var name = result.ToString();
                for (int i = 0; i < members.Length; i++)
                    if (members[i].Name == name)
                        return result;
            }
        }
        else if (text != null)
        {
            if (Enum.TryParse(enumType, text, false, out result) || Enum.TryParse(enumType, text, true, out result))
                return result;
        }
        else
        {
            // TODO: Add cache here... 

            Enum.TryParse(enumType, text, out result);
        }

        var checkUnderscore = text?.Length > 1;

        if (members == null)
            members = EnumMembersCached.Cache(enumType, () => enumType.GetMembers(BindingFlags.Public | BindingFlags.Static));

        for (int i = 0; i < members.Length; i++)
        {
            var member = members[i];

            // TODO: Cache GetCustomAttribute to avoid that lookup over n over
            if (member.GetCustomAttribute(SystemType.EnumValueAttributeType) is EnumValueAttribute valueAttr)
            {
                if (Equals(valueAttr.Value, text) || (valueAttr.Value + "").Equals(text, StringComparison.OrdinalIgnoreCase))
                    if (Enum.TryParse(enumType, member.Name, out result))
                        return result;
            }

            if (member.GetCustomAttribute(SystemType.EnumTextAttributeType) is EnumTextAttribute textAttr)
            {
                if (textAttr.Text?.Equals(text, StringComparison.OrdinalIgnoreCase) == true)
                    if (Enum.TryParse(enumType, member.Name, out result))
                        return result;
            }

            if (checkUnderscore == true &&
                member.Name.Length == text.Length + 1 &&
                member.Name[0] == '_' &&
                string.Compare(member.Name, 1, text, 0, text.Length, true) == 0)
            {
                if (Enum.TryParse(enumType, member.Name, out result))
                    return result;
            }
        }

        // TODO: Consider if "enum 0" is always the one thing we want to default return as, instead of the "first member", whatever that is...
        // TODO: Cache the instance, return the same always
        if (result == null || result == "")
            return Activator.CreateInstance(enumType);

        return result;
    }

    /// <summary>
    /// Returns a SHA256 hash string of the text.
    /// </summary>
    /// <example>
    /// <code>
    /// var hash = "Hello world".ToSha256Hash();
    /// </code>
    /// </example>
    public static string ToSha256Hash(this string text)
    {
        return Sha256.Compute(text.GetBytes());
    }

    /// <summary>
    /// Returns a camel cased string — first letter lowercased, first letter of each subsequent word uppercased.
    /// Words are delimited by space or dash.
    /// </summary>
    /// <example>
    /// <code>
    /// "abC deF".toCamelCase(); // "abc Def"
    /// </code>
    /// </example>
    public static string toCamelCase(this string text)
    {
        if (text.IsNot()) return text;

        if (char.IsUpper(text[0]))
        {
            if (!text.Contains(" ") && !text.Contains("-"))
            {
                return char.ToLower(text[0]) + text.Substring(1);
            }
        }
        else
        {
            if (!text.Contains(" ") && !text.Contains("-"))
            {
                return text;
            }
        }

        var length = text.Length;

        var sb = new StringBuilder(length);

        sb.Append(char.ToLower(text[0]));

        var c = '0';

        for (int i = 1; i < length; i++)
        {
            c = text[i];
            if (c == ' ')
            {
                i++;
                sb.Append(" " + char.ToUpper(text[i]));
            }
            else if (c == '-')
            {
                i++;
                sb.Append("-" + char.ToUpper(text[i]));
            }
            else
            {
                sb.Append(char.ToLower(c));
            }
        }

        return sb.ToString();
    }

    static string[] excludedNamespaces = new[] {
        "System.Runtime",
        "System.Threading",
        "System.Reflection",
        "Microsoft.",
        "Windows.",
        "MSBuild.",
        "MSTest.",
        "VSTest.",
        "testhost.",
        "netstandard.",
    };

    static string GetNonNoiseFrameName(MethodBase m)
    {
        var t = m.DeclaringType;
        if (t == null) return null;

        var ns = t.Namespace;
        if (ns == null) return null;

        if (ns == "System") return null;

        if (ns.StartsWithAny(excludedNamespaces))
            return null;

        return ns + "." + t.Name + "." + m.Name;
    }

    const int MaxFriendlyStackFrames = 30;

    /// <summary>
    /// Returns a display-friendly stack trace filtered to remove framework and test runner noise.
    /// </summary>
    public static string ToFriendlyStackTrace(this StackTrace stackTrace)
    {
        //return stackTrace.ToString();

        if (stackTrace == null) return null;

        var frames = stackTrace.GetFrames();

        var sb = new StringBuilder(300);

        var methodEnding = "()" + Environment.NewLine;

        var writtenFrames = 0;

        for (int i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];

            var methodBase = frame.GetMethod();

            var name = GetNonNoiseFrameName(methodBase);

            if (name == null)
            {
                if (methodBase.Name == "InvokeMethod" &&
                    methodBase.DeclaringType?.Namespace == "System")
                {
                    break;
                }

                continue;
            }


            if (sb.Length > 1)
            {
                sb.Append(" at ");
            }

            sb.Append(name);
            sb.Append("()");

            if (EnvironmentInstance.IsDev)
            {
                var file = frame.GetFileName();
                var line = frame.GetFileLineNumber();

                if (file != null && line > 0)
                {
                    sb.Append(" in ");
                    sb.Append(file);
                    sb.Append(":line ");
                    sb.Append(line);
                }
            }

            sb.AppendLine();

            writtenFrames++;

            if (writtenFrames > MaxFriendlyStackFrames)
                break;
        }

        sb.TrimEnd(Environment.NewLine);

        return sb.ToString();
    }

    /// <summary>
    /// Returns a display-friendly stack trace string filtered to remove framework and test runner noise.
    /// </summary>
    /// <example>
    /// <code>
    /// var trace = Environment.StackTrace.ToFriendlyStackTrace();
    /// </code>
    /// </example>
    public static string ToFriendlyStackTrace(this string stackTrace)
    {
        if (stackTrace == null || stackTrace.Length == 0) return stackTrace;

        try
        {
            var traces = stackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            if (traces.Length == 0) return stackTrace;

            var stackTraceBuilder = new StringBuilder("");

            var end = Math.Min(traces.Length, 9);

            for (int i = 0; i < end; i++)
            {
                if (traces[i].StartsWithAny(
                    "   at System.RuntimeMethodHandle.",
                    "   at System.Reflection.RuntimeMethodInfo",
                    "   at Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.MSTestExecutor.<>c__DisplayClass",
                    "   at lambda_method"))
                {
                    break;
                }

                if (traces[i].StartsWithAny(
                    "   at SystemLibrary.Common.Framework.LogFormatter",
                    "   at LoggerRateLimiter.Flush",
                    "   at System.Environment.get_"))
                {
                    continue;
                }

                if (i < Math.Min(traces.Length, 9) - 1)
                    stackTraceBuilder.Append("\t" + traces[i].TrimStart() + "\n");
            }
            return stackTraceBuilder.ToString().TrimStart('\t').TrimEnd("\n");
        }
        catch
        {
            return "unknown";
        }
    }
}
