using System.Runtime.Serialization;

namespace SystemLibrary.Common.Framework.Boostrap;

internal static class FormatBoot
{
    internal static void Strap() { }

    static FormatBoot()
    {
        if (EnvironmentInstance.IsDev)
        {
            FormatInstance.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            FormatInstance.DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fff";
        }
        else
        {
            FormatInstance.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";
            FormatInstance.DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";
        }
    }
}
