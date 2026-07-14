using System.Text;

using SystemLibrary.Common.Framework.Boostrap;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

internal interface ILogFormatter
{
    static abstract string Format(LogMessage message);
}