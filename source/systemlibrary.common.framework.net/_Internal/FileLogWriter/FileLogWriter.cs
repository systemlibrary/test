using SystemLibrary.Common.Framework.Boostrap;

namespace SystemLibrary.Common.Framework;

internal class FileLogWriter : ILogWriter
{
    public void Write(LogMessage message)
    {
        BootstrapLog.Write("FILE WRITER LOL");
    }
}
