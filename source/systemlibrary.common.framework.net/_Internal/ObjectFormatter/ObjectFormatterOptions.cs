namespace SystemLibrary.Common.Framework.Extensions;

public class ObjectFormatterOptions
{
    public bool ExcludeNullMembers = false;
    /// <summary>
    /// Start level, depth, of members of objects
    /// </summary>
    public int StartLevel = 0;

    /// <summary>
    /// Skipping members further down/deeper than this level (nested members at level 6 is never printed by default)
    /// </summary>
    public int MaxLevel = 5;
}