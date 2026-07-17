using System.Reflection.Metadata;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class FormatInstance
{
    internal static string DateTimeFormat;
    internal static string DateTimeOffsetFormat;
    internal static string[] ObjectTextFormatterMemberOrder;

    static FormatInstance()
    {
        Boot.Strap();
    }
}
