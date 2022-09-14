using DecompressTool.Utilities;
using System;

namespace DecompressTool.Logging
{
    public interface ILogger
    {
        void Log(string message, Exception ex = null, LogType logType = LogType.Info);
    }
}
