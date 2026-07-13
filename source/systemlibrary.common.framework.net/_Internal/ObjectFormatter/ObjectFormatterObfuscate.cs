namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectFormatterObfuscate
{
    internal static HashSet<string> MemberNames;

    static ObjectFormatterObfuscate()
    {
        MemberNames = new();

        MemberNames.Add("Ssn");
        MemberNames.Add("SocialSecurityNumber");

        MemberNames.Add("LastName");
        MemberNames.Add("Surname");

        MemberNames.Add("FullName");

        MemberNames.Add("Authorization");
    }
}
