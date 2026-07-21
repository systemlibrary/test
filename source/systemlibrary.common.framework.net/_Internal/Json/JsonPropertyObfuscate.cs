using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal static class JsonPropertyObfuscate
{
    internal static HashSet<string> MemberNames;

    static JsonPropertyObfuscate()
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
