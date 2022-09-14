using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DecompressTool.Logging
{
    public class ProcessedFilesLogger
    {
        protected readonly string logDirPath;
        protected string logFilePath;
        public ProcessedFilesLogger(string logDirPath, string logFileName)
        {
            logFilePath = Path.Combine(logDirPath, logFileName);
            this.logDirPath = logDirPath;
        }

        public void Log(IList<string> extractedFilePaths)
        {
            if (!Directory.Exists(logDirPath))
            {
                Directory.CreateDirectory(logDirPath);
            }

            if (!File.Exists(logFilePath))
            {
                using var outputFileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write);

                using var outputStreamWriter = new StreamWriter(outputFileStream);

                outputStreamWriter.Write($"{nameof(ProcessedFile.FileName)},{nameof(ProcessedFile.LastModifiedDateTime)}");
            }

            IList<ProcessedFile> records;

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Comment = '@',
                AllowComments = true,
                TrimOptions = TrimOptions.InsideQuotes,
                Delimiter = "," // Default
            };

            try
            {
                using StreamReader inputReader = File.OpenText(logFilePath);

                using CsvReader csvReader = new CsvReader(inputReader, csvConfiguration);

                records = csvReader.GetRecords<ProcessedFile>().ToList();
            }
            catch (Exception)
            {
                return;
            }

            foreach (string inputFilePath in extractedFilePaths)
            {
                if (!File.Exists(inputFilePath))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(inputFilePath);

                string fileName = fileInfo.Name;

                DateTime lastModifiedDateTime = fileInfo.LastWriteTime;

                var matchedFile = records.FirstOrDefault(x => x.FileName == fileName);

                if (matchedFile != null)
                {
                    if (lastModifiedDateTime > DateTime.Parse(matchedFile.LastModifiedDateTime))
                    {
                        matchedFile.LastModifiedDateTime = lastModifiedDateTime.ToString("dd-MMM-yyyy hh:mm:ss");
                    }
                }
                else
                {
                    //append
                    records.Add(new ProcessedFile { FileName = fileName, LastModifiedDateTime = lastModifiedDateTime.ToString("dd-MMM-yyyy hh:mm:ss") });
                }
            }

            records.OrderBy(x => DateTime.Parse(x.LastModifiedDateTime));

            using StreamWriter outputWriter = File.CreateText(logFilePath);

            using CsvWriter csvWriter = new CsvWriter(outputWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(records);
        }
    }
}
