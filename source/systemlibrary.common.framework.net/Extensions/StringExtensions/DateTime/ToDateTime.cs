using System.Globalization;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Converts a string to <c>DateTime</c> by trying multiple formats and cultures until one succeeds.
    /// Returns <c>DateTime.MinValue</c> for null or strings shorter than 4 chars.
    /// Unix timestamps (seconds and milliseconds) are also supported.
    /// </summary>
    /// <remarks>
    /// Throws if no format matches — pass a <c>format</c> argument to prioritize a known format and reduce fallback attempts.
    /// </remarks>
    /// <example>
    /// <code>
    /// "2000-12-24".ToDateTime();
    /// "1640390400000".ToDateTime(); // unix ms
    /// "24 Dec 2000".ToDateTime();
    /// "2000-12-24".ToDateTime("yyyy-MM-dd");
    /// </code>
    /// </example>
    public static DateTime ToDateTime(this string date, string format = null)
    {
        // TODO: Optimize greatly as this is not friendly, or completely recreate into either:
        //A) normalize date into a standard format, string parsing... converting only once
        //B) read the date char by char, and increase either ms, s, m, H, day, month or year by the value, ending up in a perfect datetime (all ints)
        //C) create a cache on either last successful format used, and always try that initially, as most apps use that one format throughout
        //D) completely rethink the format, their order, and based on date (string) parsing, we can skip a lot of formats, group formats better and try all in a group
        if (date == null)
            return DateTime.MinValue;

        var l = date.Length;

        if (l < 4)
            return DateTime.MinValue;

        if (l == 4)
            return new DateTime(Convert.ToInt32(date), 1, 1);

        DateTime dt;

        if (DateTime.TryParse(date, out dt))
            return dt;

        if (format.Is())
        {
            if (DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.RoundtripKind, out dt))
                return dt;

            if (DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                return dt;

            if (DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
        }

        if (DateTime.TryParse(date, null, DateTimeStyles.RoundtripKind, out dt) ||
            DateTime.TryParse(date, null, DateTimeStyles.AssumeUniversal, out dt))
        {
            return dt;
        }

        var monthName = char.IsAsciiLetter(date[0]) || char.IsAsciiLetter(date[4]);

        if (monthName)
        {
            if (TryParseWithFormats(date, MonthlyNameDateTimeFormats, out dt))
                return dt;
        }

        var z = date[l - 1] == 'Z' || date[l - 1] == 'z';

        if (l <= 12)
        {
            if (z)
            {
                if (TryParseWithFormats(date, ShortDateTimeFormatsEndsInZ, out dt))
                    return dt;
            }
            else
            {
                if (TryParseWithFormats(date, ShortDateTimeFormats, out dt))
                    return dt;
            }
        }
        else
        {
            if (z)
            {
                if (TryParseWithFormats(date, LongDateTimeFormatsEndsInZ, out dt))
                    return dt;
            }
            else
            {
                var plus = date[l - 6] == '+';

                if (plus)
                {
                    if (TryParseWithFormats(date, LongDateTimeFormatsWithPlus, out dt))
                        return dt;
                }
                else
                {
                    if (TryParseWithFormats(date, LongDateTimeFormats, out dt))
                        return dt;
                }
            }
        }

        if (long.TryParse(date, out long unixTimestamp))
        {
            if (unixTimestamp > 99999999999)
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimestamp);
            else
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp);
        }

        foreach (var culture in Cultures)
        {
            if (DateTime.TryParse(date, culture, DateTimeStyles.None, out dt))
                return dt;

            foreach (var cultureFormat in DateTimeFormatsCulture)
            {
                if (DateTime.TryParseExact(date, cultureFormat, culture, DateTimeStyles.RoundtripKind, out dt))
                    return dt;
            }
        }

        if (TryParseWithFormats(date, AllCultureFormats, out dt))
            return dt;

     foreach (var culture in Cultures)
        {
            if (DateTime.TryParse(date, culture, DateTimeStyles.None, out dt))
                return dt;

            foreach (var cultureFormat in DateTimeFormatsCulture)
            {
                if (DateTime.TryParseExact(date, cultureFormat, culture, DateTimeStyles.RoundtripKind, out dt))
                    return dt;
            }
        }

        if (TryParseWithFormats(date, AllCultureFormats, out dt))
            return dt;

        foreach(var f in AllDateTimeFormats)
        {
            if (DateTime.TryParseExact(date, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
        }

        throw new Exception("Invalid date " + date + ". Input was not recognized as a valid DateTime. No matching format provided. You sent in format: '" + format + "'");
    }
}
