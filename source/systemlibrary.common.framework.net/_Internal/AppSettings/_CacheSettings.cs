
namespace SystemLibrary.Common.Framework.Bootstrap;

internal class CacheSettings
{
    public int Duration { get; set; } = 180; // 3 minutes
    public int ContainerSizeLimit { get; set; } = 40000;
    public int FallbackDuration { get; set; } = 180; // 3 minutes

    public string Hello { get; set; }
}

/*
 * appsetings.json
 * 
 * {
 *   "systemLibraryCommonFramework": {
 *         "app": {
 *         
 *         }
 *         "CACHE": {
 *           "heLLo": "world"
 *         }
 *   
 *   }
 *  
 * }
 * 
 * */