using System.Reflection.Metadata;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class FormatInstance
{
    internal static string DateTimeFormat;
    internal static string DateTimeOffsetFormat;

    static FormatInstance()
    {
        Boot.Strap();
    }
}
