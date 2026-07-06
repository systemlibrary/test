
namespace SystemLibrary.Common.Framework.Boostrap;

internal class LogSettings
{
    string _FullFilePath;
    public string FullFilePath
    {
        get
        {
            return _FullFilePath;
        }
        set
        {
            _FullFilePath = value._ToOsFriendlyPath();
        }
    }

    public LogLevel Level { get; set; } = LogLevel.Warning;
    //public LogFormat Format { get; set; } = LogFormat.Text;
    //public LogForward Forward { get; set; } = LogForward.Default;

    public bool AddTimestamp { get; set; } = false;
    public bool AddAuthenticatedState { get; set; } = false;
    public bool AddUrl { get; set; } = true;
    public bool AddBrowserName { get; set; } = false;
    public bool AddStacktrace { get; set; } = false;
    public bool AddCorrelationId { get; set; } = true;
}

