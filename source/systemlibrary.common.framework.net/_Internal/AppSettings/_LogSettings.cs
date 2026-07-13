
namespace SystemLibrary.Common.Framework.Boostrap;

internal class LogSettings
{
    public string FullFilePath { get; set; } = null;
    public LogLevel Level { get; set; } = LogLevel.All;
    public LogFormat Format { get; set; } = LogFormat.Text;
    public LogWriterType LogForward { get; set; } = LogWriterType.Std;
    public StdForward StdForward { get; set; } = StdForward.None;
}

