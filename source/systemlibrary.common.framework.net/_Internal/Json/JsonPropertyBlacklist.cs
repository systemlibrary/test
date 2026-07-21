using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal static class JsonPropertyBlacklist
{
    internal static HashSet<string> MemberNames;

    internal static HashSet<string> ClassNames;

    static JsonPropertyBlacklist()
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
    }
}