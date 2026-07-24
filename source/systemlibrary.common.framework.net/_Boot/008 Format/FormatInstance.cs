using System.Reflection.Metadata;

namespace SystemLibrary.Common.Framework.Bootstrap;

internal static class FormatInstance
{
    internal static string IsoDateTimeFormat;
    internal static string IsoDateTimeOffsetFormat;
    internal static string[] ObjectTextFormatterMemberOrder;

    static FormatInstance()
    {
        Boot.Strap();
    }
}
