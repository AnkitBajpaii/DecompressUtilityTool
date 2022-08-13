namespace DecompressTool.Logging
{
    public class AppLogger : AbstractLogger, ILogger
    {
        public AppLogger(string logDirPath, string logFileName) : base(logDirPath, logFileName)
        {
        }

        protected override void LogToFile(string message)
        {
            var openToWriteTo = new FileStreamOptions
            {
                Mode = FileMode.Append, // open existing file then seek to end, otherwise create new file                
                Access = FileAccess.Write,
            };

            using var outputFileStream = new FileStream(logFilePath, openToWriteTo);

            using var outputStreamWriter = new StreamWriter(outputFileStream);

            outputStreamWriter.Write(message);
        }
    }
}
