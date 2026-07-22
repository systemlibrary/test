using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace SystemLibrary.Common.Framework.Extensions;

internal static class ObjectSanitizedBlacklist
{
    internal static HashSet<string> MemberNames;

    internal static HashSet<string> ClassNames;

    static ObjectSanitizedBlacklist()
    {
        ClassNames = new();
        ClassNames.Add(typeof(RuntimeTypeHandle).Name);
        ClassNames.Add(typeof(ModelBindingMessageProvider).Name);
        ClassNames.Add(typeof(RuntimeWrappedException).Name);

        ClassNames.Add("RuntimeAssembly");
        ClassNames.Add("RuntimeModule");
        ClassNames.Add("RuntimeMethodHandle");
        ClassNames.Add("ControllerContext");
        ClassNames.Add("RuntimeMethodInfo");

        MemberNames = new();

        MemberNames.Add("Constructor");
        MemberNames.Add("ReturnTypeCustomAttributes");
        MemberNames.Add("ReturnParameter");
        MemberNames.Add("MethodImplementationFlags");
        MemberNames.Add("CallingConvention");

        MemberNames.Add("Password");
        MemberNames.Add("PasswordSalt");
        MemberNames.Add("Salt");
        MemberNames.Add("JWT");
        MemberNames.Add("SessionId");
        MemberNames.Add("ClientSecret");
        MemberNames.Add("Secret");
    }
}