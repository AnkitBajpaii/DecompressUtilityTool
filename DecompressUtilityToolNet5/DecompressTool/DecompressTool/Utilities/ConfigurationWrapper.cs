using System;
using System.Configuration;

namespace DecompressTool.Utilities
{
    public sealed class ConfigurationWrapper
    {
        public string From { get; private set; } = String.Empty;
        public string To { get; private set; } = String.Empty;
        public string ApplicationLogDir { get; private set; } = String.Empty;
        public string ApplicationLogFileName { get; private set; } = String.Empty;
        public string ProcessedFilesLogDir { get; private set; } = String.Empty;
        public string ProcessedFileName { get; private set; } = String.Empty;

        private static object _lock = new object();

        private ConfigurationWrapper()
        {

        }

        private static ConfigurationWrapper? _instance;
        public static ConfigurationWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = ConfigurationWrapper.GetConfiguration();
                        }
                    }
                }

                return _instance;
            }
        }
        private static ConfigurationWrapper GetConfiguration()
        {
            string from = ConfigurationManager.AppSettings["sourceDir"]!;
            string to = ConfigurationManager.AppSettings["targetDir"]!;
            string applicationLogDir = ConfigurationManager.AppSettings["applicationLogDir"]!;
            string applicationLogFileName = ConfigurationManager.AppSettings["applicationLogFileName"]!;
            string processedFilesLogDir = ConfigurationManager.AppSettings["processedFileLogDir"]!;
            string processedFileName = ConfigurationManager.AppSettings["processedFileName"]!;

            return new ConfigurationWrapper
            {
                From = from,
                To = to,
                ApplicationLogDir = applicationLogDir,
                ApplicationLogFileName = applicationLogFileName,
                ProcessedFilesLogDir = processedFilesLogDir,
                ProcessedFileName = processedFileName
            };
        }
    }
}
