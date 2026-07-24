using Microsoft.Extensions.Logging;


namespace SystemLibrary.Common.Framework.App;

internal class StdToLogProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new StdToLogILogger();
    }

    public void Dispose()
    {
    }


}
