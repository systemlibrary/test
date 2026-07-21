using System.Globalization;

namespace SystemLibrary.Common.Framework;

partial class StringExtensions
{
    /// <summary>
    /// Converts a string to <c>DateTimeOffset</c> by trying multiple formats and cultures until one succeeds.
    /// Returns <c>DateTimeOffset.MinValue</c> for null or strings shorter than 4 chars.
    /// Unix timestamps (seconds and milliseconds) are also supported.
    /// </summary>
    /// <remarks>
    /// Throws if no format matches — pass a <c>format</c> argument to prioritize a known format and reduce fallback attempts.
    /// </remarks>
    /// <example>
    /// <code>
    /// "2000-12-24".ToDateTimeOffset();
    /// "1640390400000".ToDateTimeOffset(); // unix ms
    /// "2000-12-24".ToDateTimeOffset("yyyy-MM-dd");
    /// </code>
    /// </example>
    public static DateTimeOffset ToDateTimeOffset(this string date, string format = null)
    {
        if (date == null)
            return DateTimeOffset.MinValue;

        var l = date.Length;

        if (l < 4)
            return DateTimeOffset.MinValue;

        if (l == 4)
            return new DateTimeOffset(new DateTime(Convert.ToInt32(date), 1, 1));

        DateTimeOffset dt;

        if (DateTimeOffset.TryParse(date, out dt))
            return dt;

        if (format.Is())
        {
            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.RoundtripKind, out dt))
                return dt;

            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                return dt;

            if (DateTimeOffset.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
        }

        if (DateTimeOffset.TryParse(date, null, DateTimeStyles.RoundtripKind, out dt) ||
            DateTimeOffset.TryParse(date, null, DateTimeStyles.AssumeUniversal, out dt))
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
                return new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimestamp));
            else
                return new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp));
        }

        foreach (var culture in Cultures)
        {
            if (DateTimeOffset.TryParse(date, culture, DateTimeStyles.None, out dt))
                return dt;

            foreach (var cultureFormat in DateTimeFormatsCulture)
            {
                if (DateTimeOffset.TryParseExact(date, cultureFormat, culture, DateTimeStyles.RoundtripKind, out dt))
                    return dt;
            }
        }

        if (TryParseWithFormats(date, AllCultureFormats, out dt))
            return dt;

        foreach (var f in AllDateTimeFormats)
        {
            if (DateTimeOffset.TryParseExact(date, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
        }


        throw new Exception("Invalid date " + date + ". Input was not recognized as a valid DateTime. No matching format provided. You sent in format: '" + format + "'");
    }
}
