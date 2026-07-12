
namespace SystemLibrary.Common.Framework.Boostrap;

internal class LogSettings
{
    public string FullFilePath { get; set; } = null;
    public LogLevel Level { get; set; } = LogLevel.Debug;
    public LogFormat Format { get; set; } = LogFormat.Text;
    public LogForward LogForward { get; set; } = LogForward.LogToStd;
    public StdForward StdForward { get; set; } = StdForward.None;
}

