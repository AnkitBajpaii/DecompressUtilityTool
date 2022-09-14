using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecompressTool.Sample
{
    internal class TextFileProcessor
    {
        public string InputFilepath { get; set; }
        public string OutputFilePath { get; set; }

        public TextFileProcessor(string inputFilepath, string outputFilePath)
        {
            InputFilepath = inputFilepath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            try
            {
                // read all text at once
                String originaltext = File.ReadAllText(InputFilepath);

                string processedText = originaltext.ToUpperInvariant();

                // output entire text at once to a file
                File.WriteAllText(OutputFilePath, processedText);

                // read all lines
                string[] lines = File.ReadAllLines(InputFilepath);

                // you can process individual lines
                lines[1] = lines[1].ToUpperInvariant();

                // write all lines to file
                File.WriteAllLines(OutputFilePath, lines);

                // appending text content
                File.AppendAllText(OutputFilePath, "test data");

                //appending all lines
                File.AppendAllLines(OutputFilePath, lines);

                using var inputFileStream = new FileStream(InputFilepath, FileMode.Open);

                using var outputFileStream = new FileStream(OutputFilePath, FileMode.CreateNew, FileAccess.Write);

                //FileMode.Truncate to delete all file conents
                using var inputStreamReader = new StreamReader(inputFileStream);
                using var outputStreamWriter = new StreamWriter(outputFileStream);

                int currentLineNumber = 1;
                while (!inputStreamReader.EndOfStream)
                {
                    string inputLine = inputStreamReader.ReadLine();
                    string processedLine = inputLine.ToUpperInvariant();

                    bool isLastLine = inputStreamReader.EndOfStream;

                    if (isLastLine)
                    {
                        outputStreamWriter.WriteLine(processedLine);
                    }
                    else
                        outputStreamWriter.WriteLine(processedLine);
                }

                currentLineNumber++;
            }
            catch (IOException ex)
            {
                throw;
            }

        }
    }
}
