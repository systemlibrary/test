using System.Text;

using SystemLibrary.Common.Framework.Bootstrap;

namespace SystemLibrary.Common.Framework;

internal static class Diagnosis
{
    static StringBuilder Diag = new StringBuilder();

    internal static void Emit()
    {
        if (!AppInstance.Diagnostics) return;

        Emit("Environment", EnvironmentInstance.EnvironmentName + " (" + EnvironmentInstance.EnvironmentType + ")");

        // EnvironmentInstance environment,
        // AppInstance app,
        // ConfigInstance config,
        // FrameworkSettingsInstance framework,
        // CryptationInstance cryptation,
        // LogInstance log
        // AppRootInstance ContentRootPath and SettingsRootPath 
        // Flush bootstrap timer - elapsed duration on bootstrapped framework
    }

    static void Emit(string name, object arg1, object arg2 = null, object arg3 = null)
    {
        if (arg1 == null)
            arg1 = "(null)";

        var args = new[] { arg1, arg2, arg3 }.Where(x => x != null).ToArray();

        var newLine = LogInstance.Format == LogFormat.HTML ? "<br>" : "\n";

        if (LogInstance.Format == LogFormat.NDJson) 
            newLine = "";

        var quote = "";
        if (LogInstance.Format == LogFormat.Json || LogInstance.Format == LogFormat.NDJson)
            quote = "\"";

        var delimiter = ": ";

        if (LogInstance.Format == LogFormat.Json || LogInstance.Format == LogFormat.NDJson)
            delimiter = ", ";

        var line = $"{quote}{name}{quote}{delimiter}{quote}{string.Join(", ", args)}{quote}" + newLine;

        Diag.Append(line);
    }
}
