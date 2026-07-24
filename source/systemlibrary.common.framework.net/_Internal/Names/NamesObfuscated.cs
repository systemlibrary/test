using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

internal static class NamesObfuscated
{
    internal static HashSet<string> MemberNames;

    static NamesObfuscated()
    {
        MemberNames = new();

        if (!EnvironmentInstance.IsDev)
        {
            MemberNames.Add("Ssn");
            MemberNames.Add("SocialSecurityNumber");

            MemberNames.Add("Password");
            MemberNames.Add("PasswordSalt");
            MemberNames.Add("ClientSecret");

            MemberNames.Add("LastName");
            MemberNames.Add("Surname");

            MemberNames.Add("FullName");

            MemberNames.Add("Authorization");
        }
    }
}
