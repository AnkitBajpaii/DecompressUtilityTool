using DecompressTool.Logging;
using DecompressTool.Utilities;
using System.IO.Compression;

namespace DecompressTool.Services
{
    public class DecompressFileProcessor
    {
        private readonly ILogger appLogger;
        private readonly string unzipDirectoryPath;
        private readonly ProcessedFilesLogger processedFileLogger;
        Dictionary<string, IList<string>> zipToExtractedFilesPathMap = new Dictionary<string, IList<string>>();

        public DecompressFileProcessor(ILogger _appLogger, ProcessedFilesLogger _processedFileLogger, string unzipDirectoryPath)
        {
            this.appLogger = _appLogger;

            this.unzipDirectoryPath = unzipDirectoryPath;
            this.processedFileLogger = _processedFileLogger;
        }

        public void ProcessMultiple(IList<string> zippedFilePaths)
        {
            foreach (var zippedFilePath in zippedFilePaths)
            {
                try
                {
                    ProcessSingle(zippedFilePath);
                }
                catch (Exception ex)
                {
                    appLogger.Log($"Error Processing file {zippedFilePath}", ex, Utilities.LogType.Error);
                }
            }

            if (zipToExtractedFilesPathMap.Any())
            {
                List<string> extractedFilePaths = new List<string>();

                foreach (var kvp in zipToExtractedFilesPathMap)
                {
                    extractedFilePaths.AddRange(zipToExtractedFilesPathMap[kvp.Key]);
                }

                processedFileLogger.Log(extractedFilePaths);
            }
        }

        public bool ProcessSingle(string inputFilePath)
        {
            // check if file exists
            if (!File.Exists(inputFilePath))
            {
                appLogger.Log($"File {inputFilePath} does not exist.");
                return false;
            }

            string extension = Path.GetExtension(inputFilePath);
            bool isProcessed;
            switch (extension)
            {
                case ".zip":
                case ".gz":
                    {
                        isProcessed = DecompressZipFile(inputFilePath);
                        break;
                    }

                default:
                    appLogger.Log($"{extension} is unsupported file type.");
                    isProcessed = false;
                    break;
            }

            if (!isProcessed)
            {
                appLogger.Log($"File {inputFilePath} is not processed.", logType: LogType.Error);
            }

            return isProcessed;
        }

        private bool DecompressZipFile(string zipFilePath)
        {
            bool status;
            try
            {
                appLogger.Log($"Start decompressing {zipFilePath}");

                var extractPath = Path.GetFullPath(unzipDirectoryPath);

                // Ensures that the last character on the extraction path
                // is the directory separator char.
                // Without this, a malicious zip file could try to traverse outside of the expected
                // extraction path.
                if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    extractPath += Path.DirectorySeparatorChar;

                List<string> extractedFilePaths = new List<string>();

                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string fileName = RemoveTimeStamp(entry.FullName);

                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, fileName));

                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        {
                            entry.ExtractToFile(destinationPath, true);

                            if (!zipToExtractedFilesPathMap.ContainsKey(zipFilePath))
                            {
                                zipToExtractedFilesPathMap.Add(zipFilePath, new List<string>());
                            }

                            zipToExtractedFilesPathMap[zipFilePath].Add(destinationPath);

                            extractedFilePaths.Add(destinationPath);
                        }
                    }
                }

                appLogger.Log($"Successfully decompressed");

                status = true;
            }
            catch (IOException ex)
            {
                appLogger.Log($"IO Exception for {zipFilePath}", ex, LogType.Error);

                status = false;
            }

            return status;
        }

        private string RemoveTimeStamp(string fileName)
        {
            string delimiterStr = "View_";

            if (fileName.Contains("View_"))
            {
                int indexTo = fileName.LastIndexOf(".");

                int indexOfDelimiterStr = fileName.LastIndexOf(delimiterStr);

                int indexStart = indexOfDelimiterStr + delimiterStr.Length;

                string res = fileName.Remove(indexStart, indexTo - indexStart);

                return res;
            }

            return fileName;

        }
    }
}
