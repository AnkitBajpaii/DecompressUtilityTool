using DecompressTool.Logging;
using DecompressTool.Services;
using DecompressTool.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DecompressTool
{
    public class DecompressApp
    {
        public void Run()
        {
            ConfigurationWrapper configurationWrapper = ConfigurationWrapper.Instance;

            ILogger? appLogger = new AppLogger(configurationWrapper.ApplicationLogDir, configurationWrapper.ApplicationLogFileName);

            ProcessedFilesLogger processedLogger = new ProcessedFilesLogger(configurationWrapper.ProcessedFilesLogDir, configurationWrapper.ProcessedFileName);

            try
            {
                appLogger.Log("Decompress Process Started");
                
                string fileNamesJson = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\files.json"));
                
                IList<string>? fileNamesList = JsonConvert.DeserializeObject<List<string>>(fileNamesJson);
                
                if (fileNamesList is null || fileNamesList.Count == 0)
                {
                    appLogger.Log("File name prefixes not found.");

                    return;
                }

                DecompressService service = new(appLogger, processedLogger);

                service.Decompress(fileNamesList, configurationWrapper.From, configurationWrapper.To);
            }
            catch (Exception ex)
            {
                appLogger.Log("Application failure", ex, LogType.Error);
            }
            finally
            {
                appLogger.Log("Decompress Process Ended");
                appLogger.Log("***************************************************************************************************************************************");
            }
        }
    }
}
