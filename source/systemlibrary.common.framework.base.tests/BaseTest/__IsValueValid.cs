using System.Collections;
using System.Net;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

partial class BaseTest
{
    static bool IsValueValid(object value, object matchBegins)
    {
        if (value == null)
        {
            return false;
        }

        if (value == matchBegins) return true;

        if (value is byte byt)
            return byt > 0;

        if (value is bool flag)
        {
            if (flag)
                if (matchBegins == null)
                    return true;

            if (matchBegins is bool bm && bm == flag)
                return true;

            if (matchBegins is string matchBeginsStr && matchBeginsStr?.Length > 0)
                throw new Exception("Validating a bool, cannot use a 'matchBegins' with parameter as a string. You probably want to pass that string as the third argument named 'message': " + matchBegins);

            return flag;
        }

        if (value is HttpResponseMessage responseMessage)
        {
            return responseMessage?.IsSuccessStatusCode == true;
        }

        if (value is HttpStatusCode statusCode)
        {
            if (matchBegins != null)
            {
                if (matchBegins is int intStatusCode)
                {
                    return (int)statusCode == intStatusCode;
                }
                if (matchBegins is HttpStatusCode statusCodeStatusCode)
                {
                    return statusCode == statusCodeStatusCode;
                }
            }
            return
                statusCode == HttpStatusCode.OK ||
                statusCode == HttpStatusCode.Accepted;
        }

        if (value is string s)
        {
            if (s == matchBegins?.ToString()) return true;

            if (s == "") return false;

            var enumKeys = Enum.GetValues<HttpStatusCode>()
                    .Cast<HttpStatusCode>()
                    .Where(status => (int)status >= 400 && (int)status != 500);

            foreach (var enumKey in enumKeys)
                if (s.Contains(enumKey.ToString(), StringComparison.Ordinal))
                    return false;


            if (s.ContainsAny(" Exception ", "\nException ", " Error ", "\nError ", " Invalid ", "\nInvalid "))
                return false;

            if (s.ContainsAny(StringComparison.OrdinalIgnoreCase, "Invalid:", "Error:", "Exception:", "status\": 403", "status\": 404"))
                return false;

            if (matchBegins != null)
            {
                if (s.Is() && matchBegins == "") return false;

                if (s.Equals(matchBegins.ToString()) == false &&
                    s.StartsWith(matchBegins.ToString()) == false)
                    return false;
            }

            // The string is valid
            // does not contain invalid text
            // does not contain invalid httpstatuscodes
            // does equal or start with 'match' if match was passed
            return true;
        }

        if (value is Array arr)
            return arr.Length > 0;

        if (value is IList ilist)
            return ilist.Count > 0;

        if (value is IEnumerable ienumerable)
        {
            var count = 0;
            foreach (var enumerableItem in ienumerable)
            {
                count++;
                break;
            }

            return count > 0;
        }

        if (value.ToString() == matchBegins?.ToString()) return true;

        var originalType = value?.GetType();

        if (originalType.IsClassType()) return true;

        switch (value)
        {
            case int i1 when matchBegins is int mi1: return i1 == mi1;
            case short s1 when matchBegins is short ms1: return s1 == ms1;
            case long l1 when matchBegins is long ml1: return l1 == ml1;

            case uint ui1 when matchBegins is uint mui1: return ui1 == mui1;
            case ushort us1 when matchBegins is ushort mus1: return us1 == mus1;
            case ulong ul1 when matchBegins is ulong mul1: return ul1 == mul1;

            case float f1 when matchBegins is float mf1: return f1 == mf1;
            case double d1 when matchBegins is double md1: return d1 == md1;

            case decimal m1 when matchBegins is decimal mm1: return m1 == mm1;

            case DateTime dt1: return dt1 > DateTime.MinValue && dt1 < DateTime.MaxValue;
            case DateTimeOffset dto1: return dto1 > DateTimeOffset.MinValue && dto1 < DateTimeOffset.MaxValue;

            default:
                break;
        }

        return
               (value is Int16 i16 && i16 > 0) ||
               (value is int i && i > 0) ||
               (value is Int64 i64 && i64 > 0) ||

               (value is UInt16 ui16 && ui16 > 0) ||
               (value is uint ui && ui > 0) ||
               (value is UInt64 ui64 && ui64 > 0) ||

               (value is double d && d > 0) ||
               (value is float f && f > 0) ||

               (value is DateTime dt && (dt > DateTime.MinValue && dt < DateTime.MaxValue)) ||
               (value is DateTimeOffset dto && (dto > DateTimeOffset.MinValue && dto < DateTimeOffset.MaxValue));
    }
}
