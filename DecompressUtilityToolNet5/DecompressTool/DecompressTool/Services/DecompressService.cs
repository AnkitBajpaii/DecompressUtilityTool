using DecompressTool.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DecompressTool.Services
{
    public class DecompressService
    {
        private readonly ILogger appLogger;
        private readonly ProcessedFilesLogger processedFileLogger;

        public DecompressService(ILogger _appLogger, ProcessedFilesLogger _processedFileLogger)
        {
            this.appLogger = _appLogger;
            processedFileLogger = _processedFileLogger;
        }

        public void Decompress(IList<string> filePrefixes, string? from, string? to)
        {
            if (String.IsNullOrEmpty(from))
            {
                this.appLogger.Log($"From Directory is Empty");
                return;
            }

            if (String.IsNullOrEmpty(to))
            {
                this.appLogger.Log($"To Directory is Empty");
                return;
            }

            if (!Directory.Exists(from))
            {
                this.appLogger.Log($"Directory doesn't exist {from}");
                return;
            }

            if (!Directory.Exists(to))
            {
                this.appLogger.Log($"Directory doesn't exist {to}");
                return;
            }

            IList<string> latestModifiedZippedFilePaths = GetLatestModifiedFiles(filePrefixes, from).Select(x => x.FullName).ToList();

            DecompressFileProcessor fileProcessor = new DecompressFileProcessor(appLogger, processedFileLogger, to);

            fileProcessor.ProcessMultiple(latestModifiedZippedFilePaths);
        }

        private static IList<FileInfo> GetLatestModifiedFiles(IList<string> filePrefixes, string? from)
        {
            DirectoryInfo di = new DirectoryInfo(from);

            var fileToProcess = di.GetFiles().Where(x => x.Name.EndsWith(".zip") || x.Name.EndsWith(".gz"));

            Dictionary<string, IList<FileInfo>> allFilesMap = new Dictionary<string, IList<FileInfo>>();

            foreach (var filePrefix in filePrefixes)
            {
                foreach (var file in fileToProcess)
                {
                    if (file.Name.StartsWith(filePrefix))
                    {
                        if (!allFilesMap.ContainsKey(filePrefix))
                        {
                            allFilesMap.Add(filePrefix, new List<FileInfo>());
                        }

                        allFilesMap[filePrefix].Add(file);
                    }
                }
            }

            IList<FileInfo> latestModifiedFiles = new List<FileInfo>();

            foreach (var kvp in allFilesMap)
            {
                IList<FileInfo> fileInfos = kvp.Value;

                latestModifiedFiles.Add(fileInfos.OrderByDescending(x => x.LastWriteTime).First());
            }

            return latestModifiedFiles;
        }
    }
}
