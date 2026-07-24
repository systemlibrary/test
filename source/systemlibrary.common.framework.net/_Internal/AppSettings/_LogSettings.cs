
namespace SystemLibrary.Common.Framework.Bootstrap;

internal class LogSettings
{
    public string FilePath { get; set; } = null;
    public LogLevel Level { get; set; } = LogLevel.All;
    public LogFormat Format { get; set; } = LogFormat.Text;
    public LogType LogType { get; set; } = LogType.Std;
    public StdForward StdForward { get; set; } = StdForward.None;
}

