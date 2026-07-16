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

    /// <summary>
    /// Format of the text
    /// </summary>
    public ObjectFormatterFormat Format = ObjectFormatterFormat.Plain;

    /// <summary>
    /// Order members, first as the top member, if you need to override the way attribute order doesnt please the environment
    /// In dev you might want "Level" first, while in other env you want timestamp, for a "class LogMessage" for instance"
    /// </summary>
    public string[] MemberOrder;
}