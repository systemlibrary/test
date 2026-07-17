using System.Runtime.Serialization;

using Microsoft.Extensions.Options;

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
            FormatInstance.ObjectTextFormatterMemberOrder = [
                nameof(LogMessage.Level),
                nameof(LogMessage.Timestamp)
            ];
        }
        else
        {
            FormatInstance.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";
            FormatInstance.DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";
        }

    }
}
