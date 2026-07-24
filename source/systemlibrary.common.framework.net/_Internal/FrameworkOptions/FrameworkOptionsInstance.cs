namespace SystemLibrary.Common.Framework;

/// <summary>
/// The 'global' and only way to read 'FrameworkOptions' is through this instance throughout app lifetime
/// </summary>
internal static class FrameworkOptionsInstance
{
    internal static FrameworkOptions Current = new FrameworkOptions();

    internal static void Configure(FrameworkOptions options)
    {
        if (options != null)
            Current = options;
    }
}
