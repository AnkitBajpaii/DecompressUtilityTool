using System.IO;

namespace DecompressTool.Logging
{
    public class AppLogger : AbstractLogger, ILogger
    {
        public AppLogger(string logDirPath, string logFileName) : base(logDirPath, logFileName)
        {
        }

        protected override void LogToFile(string message)
        {
            using var outputFileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);

            using var outputStreamWriter = new StreamWriter(outputFileStream);

            outputStreamWriter.Write(message);
        }
    }
}
