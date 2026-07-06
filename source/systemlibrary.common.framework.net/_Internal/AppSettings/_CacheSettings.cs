
namespace SystemLibrary.Common.Framework.Boostrap;

internal class CacheSettings
{
    public int Duration { get; set; } = 180; // 3 minutes
    public int ContainerSizeLimit { get; set; } = 40000;
    public int FallbackDuration { get; set; } = 180; // 3 minutes
}
