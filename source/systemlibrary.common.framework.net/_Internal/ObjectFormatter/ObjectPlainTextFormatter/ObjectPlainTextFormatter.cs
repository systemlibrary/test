using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// TODO: Total refactor, restructure into proper methods and reusable methods, and lists of configurations that makes this extensible
/// Fix: Maximum amount of 'message.Len' to be, say, 1024K chars.
/// Fix: Maximum amount of printed items in IEnumerables per IEnumerable to be 128K.
/// </summary>
internal static class ObjectPlainTextFormatter
{
    internal static ConcurrentDictionary<int, PropertyInfo[]> TypeProperties = new ConcurrentDictionary<int, PropertyInfo[]>();
    internal static ConcurrentDictionary<int, FieldInfo[]> TypeFields = new ConcurrentDictionary<int, FieldInfo[]>();

    internal static StringBuilder Format(object obj, ObjectFormatterOptions options)
    {
        var message = new StringBuilder(224);

        var visited = new List<int>();

        var level = options.StartLevel;

        Append(message, obj, level, visited, options);

        visited.Clear();

        visited = null;

        if (message.EndsWith("\n"))
            message.TrimEnd();

        return message;
    }

    static void Append(StringBuilder message, object obj, int level, List<int> visited, ObjectFormatterOptions options)
    {
        if (AppendVariable(message, obj, level, options)) return;

        if (AppendType(message, obj, level)) return;

        if (level > options.MaxLevel)
        {
            Add(message, "Skipped as max depth reached, continuing...", level);
            return;
        }

        if (AppendEnumerable(message, obj, level, visited, options)) return;

        if (AppendClass(message, obj, level, visited, options)) return;

        if (AppendKeyValuePair(message, obj, level, visited, options)) return;

        Add(message, obj.ToString(), level);
    }

    static bool AppendVariable(StringBuilder message, object obj, int level, ObjectFormatterOptions options)
    {
        var value = GetVariableValue(obj);

        if (value != null)
        {
            if (level > 0)
            {
                if (options.ExcludeNullMembers && value == "(null)")
                {
                    return true;
                }
            }

            Add(message, value, level);
            return true;
        }
        return false;
    }

    static bool AppendType(StringBuilder message, object obj, int level)
    {
        string PrintBool(string n, bool b)
        {
            if (b)
                return n + ", ";
            return "";
        }

        if (obj is Type t)
        {
            // TODO: Optimize with a string builder, so many small strings...
            var name = t.FullName;
            var index = name.IndexOf(',');
            if (index > 0)
            {
                var tmp = name.Substring(0, index);
                var countBrackets = 0;
                foreach (var a in name)
                    if (a == '[')
                        countBrackets++;

                foreach (var a in tmp)
                    if (a == ']')
                        countBrackets--;

                while (countBrackets > 0)
                {
                    tmp += "]";
                    countBrackets--;
                }
                name = tmp;
            }

            name = name.Replace("System.Collections.Generic.", "")
                .Replace("System.Collections.", "")
                .Replace("System.", "");

            var value = "Type " + name + ": "
                + (PrintBool("IsClass", t.IsClass)
                + PrintBool("IsValueType", t.IsValueType)
                + PrintBool("IsInterface", t.IsInterface)
                + PrintBool("IsEnum", t.IsEnum)
                + PrintBool("IsGenericType", t.IsGenericType)
                + PrintBool("IsPrimitive", t.IsPrimitive)
                + PrintBool("IsArray", t.IsArray)
                + PrintBool("IsAbstract", t.IsAbstract)
                + PrintBool("IsAutoClass", t.IsAutoClass)
                + PrintBool("IsPointer", t.IsPointer)
                + PrintBool("IsGenericParameter", t.IsGenericParameter)).TrimEnd(", ");

            Add(message, value, level);

            return true;
        }
        return false;
    }

    static bool AppendKeyValuePair(StringBuilder message, object obj, int level, List<int> visited, ObjectFormatterOptions options)
    {
        var objType = obj.GetType();
        if (objType.IsGenericType &&
           objType.GetGenericTypeDefinition() == SystemType.KeyValuePairType)
        {
            var value = objType.GetProperty("Value").GetValue(obj);

            if (value is string txt)
                Add(message, txt, level);

            else if (value is IEnumerable enumerable)
            {
                var key = objType.GetProperty("Key").GetValue(obj);

                if (value is Array arr)
                {
                    var array = arr.Cast<object>();
                    var joined = string.Join(", ", array);
                    if (array.Count() > 1)
                    {
                        Add(message, "[" + key + ", [" + joined + "]]", level);
                    }
                    else
                    {
                        Add(message, "[" + key + ", " + joined + "]", level);
                    }
                }
                else if (value is IDictionary dictionary)
                {
                    // TODO: Create a decent way to print any dictionary (nested dictionary, etc)
                    var ident = new string('\t', level);
                    var joined = "{\n" + ident;
                    foreach (var dk in dictionary.Keys)
                    {
                        joined += dk + ": ";
                        var v = dictionary[dk];
                        if (v != null)
                        {
                            var sb = new StringBuilder("");

                            Append(sb, v, 0, visited, options);

                            joined += sb.ToString();
                        }
                        joined += "\n" + ident;
                    }
                    Add(message, "[" + key + ", " + joined + "}]", level);
                }
                else if (value is IList list)
                {
                    var joined = string.Join(", ", list.Cast<object>());

                    Add(message, "[" + key + ", " + joined + "]", level);
                }
            }
            else
            {
                Add(message, obj.ToString(), level);
            }
            return true;
        }

        return false;
    }

    static bool AppendEnumerable(StringBuilder message, object obj, int level, List<int> visited, ObjectFormatterOptions options)
    {
        var isList = obj is IEnumerable && obj is not string;

        if (!isList) return false;

        if (obj is IEnumerable enumerable)
        {
            var originalType = obj.GetType();

            var type = (Type)null;
            var type2 = (Type)null;
            var args = (Type[])null;

            if (originalType.IsArray)
            {
                type = originalType.GetElementType();
            }
            else
            {
                args = originalType.GetGenericArguments();
                if (args?.Length > 0) type = args[0];
                if (args?.Length > 1) type2 = args[1];
            }
            if (type == null) type = originalType;

            int count = GetEnumerableCount(enumerable);

            AppendEnumerableTypeName(message, enumerable, level, type, type2, args, count);

            if (count == 0) return true;

            var printAsOneLine = IsListTypePrintableAsOneLine(type);

            var printAsMatrix = !printAsOneLine && type == typeof(int[,]) || type == typeof(long[,]) || type == typeof(string[,]);

            if (printAsOneLine)
            {
                var tmp = new StringBuilder(": ");

                if (type.IsEnum)
                {
                    foreach (var item in enumerable)
                        tmp.Append(PrintEnum(item) + ", ");
                }
                else
                {
                    foreach (var item in enumerable)
                        tmp.Append(item + ", ");
                }

                Add(message, tmp.ToString().TrimEnd(", "), level);
            }
            else if (printAsMatrix)
            {
                var arr = enumerable as Array;

                message.Append("\n" + new string('\t', level + 1));

                for (int j = 0; j < arr.GetLength(0); j++)
                {
                    for (int k = 0; k < arr.GetLength(1); k++)
                    {
                        var val = arr.GetValue(j, k);
                        message.Append(val + " ");
                    }

                    if (j + 1 < arr.GetLength(0))
                        message.Append("\n" + new string('\t', level + 1));
                }
            }
            else
            {
                Add(message, "\n", level);
                int curr = 1;

                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    var tmp = new StringBuilder();

                    Append(tmp, item, level + 1, visited, options);

                    if (tmp.Length == 0)
                        Add(tmp, item.ToString(), level + 1);

                    if (curr < count)
                    {
                        curr++;
                        Add(message, tmp.ToString(), 0);
                        Add(message, "\n", level + 1);
                    }
                    else
                    {
                        Add(message, tmp.ToString(), 0);
                    }
                }
            }
        }

        return true;
    }

    static void AppendEnumerableTypeName(StringBuilder message, IEnumerable enumerable, int level, Type type, Type type2, Type[] args, int count)
    {
        // Enumearble types of Object, we do not print that as a prefix
        if (type == SystemType.ObjectType) return;

        if (args == null || args.Length == 0)
            Add(message, PrintTypeName(type) + "[] (" + count + ")", level);
        else
        {
            if (enumerable is IDictionary)
            {
                Add(message, "IDictionary<" + PrintTypeName(type) + ", " + PrintTypeName(type2) + "> (" + count + ")", level);
            }
            else if (enumerable is Array)
            {
                Add(message, PrintTypeName(type) + "[] (" + count + ")", level);
            }
            else if (enumerable is IList)
            {
                Add(message, "List<" + PrintTypeName(type) + "> (" + count + ")", level);
            }
            else if (enumerable is ICollection)
            {
                if (args.Length == 1)
                {
                    Add(message, "ICollection<" + PrintTypeName(type) + "> (" + count + ")", level);
                }
                else
                {
                    Add(message, "ICollection<" + PrintTypeName(type) + ", " + PrintTypeName(type2) + "> (" + count + ")", level);
                }
            }
            else
                Add(message, PrintTypeName(type) + " (" + count + ")", level);
        }
    }

    static string PrintTypeName(Type type)
    {
        if (type == null) return "";
        if (type == SystemType.IntType) return "int";
        if (type == SystemType.StringType) return "string";
        if (type == SystemType.CharType) return "char";
        if (type == SystemType.Int64Type) return "long";
        if (type == SystemType.Int16Type) return "short";
        if (type == SystemType.BoolType) return "bool";
        if (type == SystemType.DoubleType) return "double";
        return type.Name;
    }

    static bool AppendClass(StringBuilder message, object obj, int level, List<int> visited, ObjectFormatterOptions options)
    {
        var type = obj.GetType();

        if (!type.IsClassType()) return false;

        if (NamesBlacklisted.ClassNames.Contains(type.Name)) return true;

        var reference = RuntimeHelpers.GetHashCode(obj);

        if (IsVisited(type, reference, visited))
        {
            Add(message, obj.GetType().Name + " ref: " + reference + " already printed, continue...", level);

            return true;
        }

        var properties = TypeProperties.Cache(type, () =>
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(p =>
                    p.CanRead &&
                    !NamesBlacklisted.MemberNames.Contains(p.Name) &&
                    !NamesBlacklisted.ClassNames.Contains(p.PropertyType.Name) &&
                    (p.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true)
                )
                .OrderBy(p =>
                {
                    var index = FormatInstance.ObjectTextFormatterMemberOrder?.IndexOf(p.Name) ?? -1;

                    if (index >= 0) return index;

                    return 1000 + (p.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? int.MaxValue);
                })
                .ToArray();
        });

        var fields = TypeFields.Cache(type, () =>
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f =>
                    !NamesBlacklisted.MemberNames.Contains(f.Name) &&
                    !NamesBlacklisted.ClassNames.Contains(f.FieldType.Name) &&
                    (f.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? true)
                )
                .OrderBy(p =>
                {
                    var index = FormatInstance.ObjectTextFormatterMemberOrder?.IndexOf(p.Name) ?? -1;

                    if (index >= 0) return index;

                    return 1000 + (p.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? int.MaxValue);
                })
                .ToArray();
        });

        var args = type.GetGenericArguments();

        var genericType = (Type)null;

        if (args?.Length > 0)
            genericType = args[0];

        var typeName = type.Name;

        if (genericType != null)
            typeName = typeName + "<" + genericType?.Name + ">";

        if (level > 0)
            Add(message, typeName + " (ref: " + reference + ")", level);

        if (message.Length > 0)
            Add(message, "\n", 0);

        level += 1;

        if (!fields.Any() && !properties.Any())
        {
            Add(message, "Variables: (none)", level);
            return true;
        }

        if (properties.Any())
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (property?.PropertyType == null) continue;

                if (!property.CanRead) continue;

                try
                {
                    var value = property.GetValue(obj);

                    if (options.ExcludeNullMembers && value == null) continue;

                    Add(message, property.Name + ": ", level - 1);

                    var sb = new StringBuilder(128);

                    if (value is string text && NamesObfuscated.MemberNames.Contains(property.Name))
                    {
                        Append(sb, text.Obfuscate(), level, visited, options);
                    }
                    else
                    {
                        Append(sb, value, level, visited, options);
                    }

                    // NOTE: Cannot remember what this was good for again
                    //int index = level;
                    //if (index > 0 && sb.Length > index)
                    //    sb.Remove(0, index);

                    Add(message, sb.ToString(), 0);
                }
                catch
                {
                    if (!message.EndsWith(": "))
                        Add(message, property.Name + ": ", level);

                    Add(message, "(error reading property, continue...)", level);
                }
                if (i < properties.Length - 1)
                    Add(message, "\n", 0);
            }
        }

        if (fields.Any())
        {
            if (properties.Any())
                Add(message, "\n", 0);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (field?.FieldType == null) continue;

                if (field.IsPrivate) continue;

                try
                {
                    var value = field.GetValue(obj);

                    if (options.ExcludeNullMembers && value == null) continue;

                    var sb = new StringBuilder(128);

                    //if (field.Name[0] == 'M' &&
                    //   type.Name == "LogMessage" &&
                    //   (field.Name == "Messages" || field.Name == "Message"))
                    //{
                    //}
                    //else
                    //{
                    Add(message, field.Name + ": ", level - 1);
                    //}

                    if (value is string text && NamesObfuscated.MemberNames.Contains(field.Name))
                    {
                        Append(sb, text.Obfuscate(), level, visited, options);
                    }
                    else
                    {
                        Append(sb, value, level, visited, options);
                    }

                    int index = level;
                    if (index > 0)
                        sb.TrimStart();

                    Add(message, sb.ToString(), 0);
                }
                catch
                {
                    if (!message.EndsWith(": "))
                        Add(message, field.Name + ": ", level);

                    Add(message, "(error reading field, continue...)", level);
                }
                if (i < fields.Length - 1)
                    Add(message, "\n", 0);
            }
        }
        return true;
    }

    static void AppendSkippedObject(StringBuilder message, object obj)
    {
        if (message.Length == 0)
            message.Append(obj.GetType().Name + " skipped");
    }

    static void Add(StringBuilder message, string value, int level)
    {
        if (message.Length != 0)
            message.Append(new string(' ', level * 2));

        if (value != null)
            message.Append(value);
    }

    static bool IsVisited(Type type, int hash, List<int> visit)
    {
        if (type.BaseType == typeof(ValueType)) return false;

        if (hash == 0) return false;

        if (visit.Contains(hash)) return true;

        visit.Add(hash);

        return false;
    }

    static bool IsListTypePrintableAsOneLine(Type listTypeArg)
    {
        return listTypeArg.IsEnum ||
            listTypeArg == SystemType.Int16Type ||
            listTypeArg == SystemType.IntType ||
            listTypeArg == SystemType.Int64Type ||

            listTypeArg == SystemType.UInt16Type ||
            listTypeArg == SystemType.UIntType ||
            listTypeArg == SystemType.UInt64Type ||

            listTypeArg == SystemType.HalfType ||
            listTypeArg == SystemType.BigIntegerType ||

            listTypeArg == SystemType.ByteType ||
            listTypeArg == SystemType.DecimalType ||
            listTypeArg == SystemType.DoubleType ||
            listTypeArg == SystemType.BoolType ||

            listTypeArg == SystemType.FloatType ||
            listTypeArg == SystemType.HalfType ||
            listTypeArg == SystemType.BigIntegerType;
    }

    static int GetEnumerableCount(IEnumerable enumerable)
    {
        int c = 0;

        if (enumerable is IDictionary d) c = d.Count;

        else if (enumerable is Array a) c = a.Length;

        else if (enumerable is IList l) c = l.Count;

        else if (enumerable is ICollection ic) c = ic.Count;

        else foreach (var item in enumerable) c++;

        return c;
    }

    static string GetVariableValue(object obj)
    {
        if (obj == null) return "(null)";

        if (obj is Exception e)
        {
            if (e is AggregateException agg)
            {
                return agg.Flatten().ToString().Replace("\n", "\n\t");
            }
            return e.ToString().Replace("\n", "\n\t");
        }

        if (obj is string str)
            return PrintString(str);

        if (obj is StringBuilder sb) return PrintString(sb.ToString());

        if (obj is DateTime dt) return dt.ToString(FormatInstance.DateTimeFormat);

        if (obj is DateTimeOffset dto) return dto.ToString(FormatInstance.DateTimeOffsetFormat);

        if (obj is Enum enu) return PrintEnum(enu);

        if (obj is ValueType &&
           !(obj is IEnumerable) &&
           !(obj is ReadOnlyMemory<int>) &&
           !(obj is ReadOnlyMemory<string>))
        {
            var objType = obj.GetType();
            if (objType.IsGenericType &&
               objType.GetGenericTypeDefinition() == SystemType.KeyValuePairType)
            {
                return null;
            }

            return obj.ToString();
        }

        if (obj is CultureInfo cult)
        {
            return $"CultureInfo: {cult.Name}, TwoLetterISO: {cult.TwoLetterISOLanguageName}, ThreeLetterISO: {cult.ThreeLetterISOLanguageName}";
        }

        if (obj is Uri uri)
        {
            return $"{uri.OriginalString} | Scheme: {uri.Scheme} | Host: {uri.Host} | Path: {uri.AbsolutePath} | Query: {(uri.Query.IsNot() ? "(empty)" : uri.Query)} | IsAbsolute: {uri.IsAbsoluteUri} | IsFile: {uri.IsFile} | Authority: {uri.Authority}";
        }

        if (obj is Version v)
        {
            return v.ToString();
        }
        if (obj is HttpMethod m)
        {
            return m.ToString();
        }

        if (obj is bool?)
            return (obj as bool?).Value + "";

        if (obj is int?)
            return (obj as int?).Value + "";

        if (obj is double?)
            return (obj as double?).Value + "";

        if (obj is short?)
            return (obj as short?).Value + "";

        if (obj is long?)
            return (obj as long?).Value + "";

        if (obj is decimal?)
            return (obj as decimal?).Value + "";

        if (obj is Memory<string> memString)
            return "Memory<string> " + memString.Span.ToString();

        if (obj is Memory<bool> memBool)
            return "Memory<bool> " + memBool.Span.ToString();

        if (obj is Memory<int> memInt)
            return "Memory<int> " + memInt.Span.ToString();

        if (obj is Memory<DateTime> memDateTime)
            return "Memory<DateTime> " + memDateTime.Span.ToString();

        if (obj is ReadOnlyMemory<string> romString)
            return "ReadOnlyMemory<string> " + romString.Span.ToString();

        if (obj is ReadOnlyMemory<int> romInt)
        {
            var tmp = "ReadOnlyMemory<int> ";
            foreach (var number in romInt.Span)
            {
                tmp += number + ", ";
            }
            return tmp;
        }

        if (obj is ReadOnlyMemory<DateTime> romDateTime) return romDateTime.Span.ToString();

        if (obj is StackTrace stackTrace)
        {
            return stackTrace.ToFriendlyStackTrace();
        }

        if (obj is StackFrame stackFrame)
        {
            var method = stackFrame.GetMethod();
            var declaringType = method?.DeclaringType;
            var ns = declaringType?.Namespace;

            var nssuffix = ns.Is() ? "." : "";

            return $"StackFrame {stackFrame.GetFileName()} {ns}{nssuffix}{declaringType?.Name}.{method?.Name}".Trim();
        }

        var type = obj.GetType();

        if (type.Name.Contains("Task`") && obj is Task task)
        {
            return $"{type.Name} | Message: {task.Exception?.Message ?? "none"} | " +
                   $"Completed: {task.IsCompleted} | Faulted: {task.IsFaulted} | " +
                   $"Successfully: {task.IsCompletedSuccessfully} | Cancelled: {task.IsCanceled}";
        }

        if (type.Name.Contains("Action`")) return "";

        if (type.Name == "DBNull") return "(null)";

        if (obj is char[] charArray)
        {
            var tmp = new char[(charArray.Length * 2) - 1];
            for (int i = 0; i < charArray.Length; i++)
            {
                tmp[i * 2] = charArray[i];
                if (i < charArray.Length - 1)
                    tmp[i * 2 + 1] = ' ';
            }

            return new string(tmp);
        }

        if (obj is object[] objects)
        {
            if (objects.Length == 1 && objects[0] is string txt)
            {
                return PrintString(txt);
            }
        }

        return null;
    }

    static string PrintEnum(object item)
    {
        var value = (item as Enum).ToValue();

        var text = (item as Enum).ToText();
        if (value.Is() && text != value)
            return text + " (" + value + ")";

        return (item as Enum).ToText();
    }

    static string PrintString(string str)
    {
        if (str == "") return "(empty)";

        var strLength = str.Length;
        if (strLength > 16384)
        {
            var lastWords = new StringBuilder();
            int spaceCount = 0;

            for (int i = strLength - 1; i >= 0 && spaceCount < 3; i--)
            {
                if (str[i] == ' ') spaceCount++;

                lastWords.Insert(0, str[i]);

                if (lastWords.Length > 192) break;
            }

            var l = 16192 + 4 + lastWords.Length;

            return $"{str.Substring(0, 16192)}...{lastWords} (length: {l}/{strLength})";
        }
        return strLength > 32 ? $"{str} (length: {strLength})" : str;
    }
}
