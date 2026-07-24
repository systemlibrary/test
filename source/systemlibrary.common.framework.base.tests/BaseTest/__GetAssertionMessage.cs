using System.Collections;
using System.Diagnostics;
using System.Text.Json;

namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    static string GetAssertionMessage(object value, object matchBegins, string message = null)
    {
        // TODO: Create a "DisplayText" extension on object
        // Use reflection, Logger.Dump or LogBuild might have almost what we want...
        // A decent way to display "value" with a message basically, showing type name, or the value, and if a class the properties and their value at depth 1 only
        // If a list we take the list, array or ienumerable count, and print the Type Name of first item or null
        // NOTE: Gives little ROI currently, moving on...
        var originalType = value?.GetType();

        var type = originalType;
        if (originalType?.IsGenericType == true)
            type = originalType.GetGenericArguments()[0];

        var ownerType = type?.Name ?? "";
        var stackTrace = new StackTrace();
        var callingMethod = stackTrace.GetFrame(2)?.GetMethod()?.Name ?? "UnknownMethod";

        string valueDisplayText;

        var appendFirst64Chars = false;
        if (value == null)
        {
            valueDisplayText = "null";
        }
        else if (value is Array arr)
        {
            valueDisplayText = $"Array[] (len: {arr.Length})";
            appendFirst64Chars = true;
        }
        else if (value is IList ilist)
        {
            valueDisplayText = $"IList<> (len: {ilist.Count})";
            appendFirst64Chars = true;
        }
        else if (value is IEnumerable enumerable &&
            value is not string)
        {
            int count = 0;
            foreach (var e in enumerable)
            {
                count++;
            }
            valueDisplayText = $"IEnumerable<> (len: {count})";
            appendFirst64Chars = true;
        }
        else if (value is HttpResponseMessage httpResponseMessage)
        {
            valueDisplayText = httpResponseMessage.ReasonPhrase + " " + httpResponseMessage.Content?.ReadAsStringAsync().Result;
        }
        else
        {
            valueDisplayText = value.ToString();

            if(valueDisplayText == "")
                valueDisplayText = "\"\"";
        }

        if (appendFirst64Chars)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = false, MaxDepth = 2, IgnoreReadOnlyFields = true, AllowTrailingCommas = true, UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip };

                var json = JsonSerializer.Serialize(value, options);

                valueDisplayText += " " + json.MaxLength(64);
            }
            catch
            {
                // Swallow, most likely types that we do not want to print, Linq expression, System.Type, etc...
            }
        }

        if (message.Is())
            message = message + " ";

        if (matchBegins != null)
        {
            message = message + "Expected " + matchBegins;
            if (matchBegins == "")
                message = "Expected \"\"";
        }
        else
        {
            message = message + "Expected valid value";
        }

        return $"{message} got -> {valueDisplayText} ({ownerType}) at {callingMethod}"
            .Replace("()", "")
            .Replace("  ", " ");
    }
}
