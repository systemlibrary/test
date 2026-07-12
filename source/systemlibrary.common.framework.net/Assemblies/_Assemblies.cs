using Asm = System.Reflection.Assembly;

namespace SystemLibrary.Common.Framework;

partial class Assemblies
{
    static Assemblies()
    {
        var assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();

        var whiteListedAssemblies = new List<Asm>();

        foreach (var asm in assembliesLoaded)
        {
            if (!asm.FullName.StartsWithAny(BlacklistedAssemblyNames) || asm.GetName()?.Name?.EndsWith(".Tests") == true)
            {
                whiteListedAssemblies.Add(asm);
            }
        }

        Types = whiteListedAssemblies.SelectMany(asm => asm.GetTypes())
            .Where(x =>
                !x.Name.StartsWith("<>f__") &&
                !x.Name.StartsWith("<>c__") &&
                !x.Name.StartsWith("<>o__"))
            .ToArray();
    }
}