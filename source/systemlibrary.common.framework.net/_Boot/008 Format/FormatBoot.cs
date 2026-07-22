namespace SystemLibrary.Common.Framework.Boostrap;

internal static class FormatBoot
{
    internal static void Strap() { }

    static FormatBoot()
    {
        FormatInstance.IsoDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";
        FormatInstance.IsoDateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";

        if (EnvironmentInstance.IsDev)
        {
            FormatInstance.ObjectTextFormatterMemberOrder = [
                nameof(LogMessage.Level),
                nameof(LogMessage.Timestamp)
            ];
        }
    }
}
