namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectSanitizedObfuscate
{
    internal static HashSet<string> MemberNames;

    static ObjectSanitizedObfuscate()
    {
        MemberNames = new();

        MemberNames.Add("Ssn");
        MemberNames.Add("SocialSecurityNumber");

        MemberNames.Add("FirstName");
        MemberNames.Add("GivenName");

        MemberNames.Add("LastName");
        MemberNames.Add("Surname");

        MemberNames.Add("FullName");

        MemberNames.Add("Authorization");
    }
}
