using DecompressTool.Utilities;
using System;
using System.IO;
using System.Text;

namespace DecompressTool.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        protected readonly string logDirPath;
        protected string logFilePath;

        public AbstractLogger(string logDirPath, string logFileName)
        {
            logFilePath = Path.Combine(logDirPath, logFileName);
            this.logDirPath = logDirPath;
        }

        protected string ExtractErrorInfo(Exception ex)
        {
            if (ex is null) return String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Exception details:");
            sb.AppendLine($"Error Message {ex.Message}");
            sb.AppendLine($"Stack Trace {ex.StackTrace}");

            Exception? innerEx = ex.InnerException;
            while (innerEx != null)
            {
                sb.AppendLine($"\tInner exception details:");
                sb.AppendLine($"\tError Message {ex.Message}");
                sb.AppendLine($"\tStack Trace {ex.StackTrace}");

                innerEx = innerEx.InnerException;
            }

            return sb.ToString();
        }

        public void Log(string message, Exception? ex = null, LogType logType = LogType.Info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{logType}: {message}");
            sb.AppendLine(ExtractErrorInfo(ex));

            Log(sb.ToString());
        }

        protected virtual void Log(string message)
        {
            if (!Directory.Exists(logDirPath))
            {
                Directory.CreateDirectory(logDirPath);
            }

            LogToFile(message);

            // Suggestion: The log file will grow with time, you need to manually clean up.
            // Or we could do something like this:
            // get file size in MB
            // if file size exceed specified size say 12 MB, we will rename our log file by appending a time stamp and add to an archieved folder.
        }

        protected abstract void LogToFile(string message);
    }
}
