using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Framework.App.Extensions;

partial class IServiceCollectionExtensions
{
    static IMvcBuilder UseApplicationParts(this IMvcBuilder builder)
    {
        if (builder == null) return builder;

        var stack = new StackTrace(2);
        var amount = 7;
        var addedAssemblies = new List<string>();

        foreach (var frame in stack.GetFrames())
        {
            if (amount == -1) break;

            amount--;

            var asm = frame.GetMethod()?.DeclaringType?.Assembly;

            if (asm != null)
            {
                //var name = asm.GetName().Name;

                //if (name != "testhost" && !name.StartsWith("Microsoft.") && !name.StartsWith("System.") && !name.EndsWith("Base.Tests"))
                //{
                //    var fullName = asm.FullName;

                //    if (name.EndsWith(".Tests") || !fullName.StartsWithAny(Assemblies.BlacklistedAssemblyNames))
                //    {
                //        if (!addedAssemblies.Contains(fullName))
                //        {
                //            addedAssemblies.Add(fullName);
                //            builder.AddApplicationPart(asm);
                //        }
                //    }
                //}
                var name = asm.GetName().Name;
                if (name != "testhost" && !name.StartsWith("Microsoft.") && !name.StartsWith("System."))
                {
                    var fullName = asm.FullName;
                    if (!addedAssemblies.Contains(fullName))
                    {
                        addedAssemblies.Add(fullName);
                        builder.AddApplicationPart(asm);
                    }
                }
            }
        }

        var executingAssembliy = Assembly.GetExecutingAssembly();
        var entryAssembly = Assembly.GetEntryAssembly();
        var callingAssembly = Assembly.GetCallingAssembly();

        if (addedAssemblies.Count > 0)
        {
            if (addedAssemblies.Contains(executingAssembliy.FullName))
                executingAssembliy = null;

            if (addedAssemblies.Contains(entryAssembly.FullName))
                entryAssembly = null;

            if (addedAssemblies.Contains(callingAssembly.FullName))
                callingAssembly = null;
        }

        builder = AddApplicationPart(builder, executingAssembliy, entryAssembly, callingAssembly);

        if (FrameworkOptionsInstance.Current.ApplicationParts != null)
        {
            foreach (var part in FrameworkOptionsInstance.Current.ApplicationParts)
            {
                if (part != null &&
                    part != executingAssembliy &&
                    part != entryAssembly &&
                    part != callingAssembly &&
                    !addedAssemblies.Contains(part.FullName))
                {
                    builder = builder.AddApplicationPart(part);
                }
            }
        }

        return builder;
    }

    static IMvcBuilder AddApplicationPart(IMvcBuilder builder, Assembly executing, Assembly entry, Assembly calling)
    {
        if (builder != null)
        {
            if (executing != null)
                builder = builder.AddApplicationPart(executing);

            if (entry != null && executing?.FullName != entry.FullName)
                builder = builder.AddApplicationPart(entry);

            if (calling != null && executing?.FullName != calling.FullName && entry?.FullName != calling.FullName)
                builder = builder.AddApplicationPart(calling);
        }

        return builder;
    }
}