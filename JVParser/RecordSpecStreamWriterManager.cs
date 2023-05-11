namespace JVParser
{
    // Class to manage mutliple stream writers
    class RecordSpecStreamWriterManager
    {
        // List of stream writers
        private Dictionary<string, StreamWriter> streamWriters;

        // Output directory
        private string outputDir;

        // File name prefix
        private string fileNamePrefix;

        // Constructor
        public RecordSpecStreamWriterManager(string outputDirectory, string fileNamePrefix)
        {
            this.streamWriters = new Dictionary<string, StreamWriter>();
            this.outputDir = outputDirectory;
            this.fileNamePrefix = fileNamePrefix;
        }

        public void Close()
        {
            foreach (KeyValuePair<string, StreamWriter> streamWriter in streamWriters)
            {
                streamWriter.Value.Close();
            }
        }

        // Ouput path
        private string GetOutputPath(string recordSpecName)
        {
            string[] paths = { outputDir, fileNamePrefix + "-" + recordSpecName + ".jsonl" };
            return Path.Combine(paths);
        }

        // Add a steam writer with file name if not exists and get the stream writer
        private StreamWriter GetStreamWriter(string recordSpecName)
        {
            if (!streamWriters.ContainsKey(recordSpecName))
            {
                var outputPath = GetOutputPath(recordSpecName);
                if (File.Exists(outputPath))
                {
                    throw new OutputFileAlreadyExistsException();
                }
                streamWriters.Add(recordSpecName, new StreamWriter(outputPath));
            }
            return streamWriters[recordSpecName];
        }

        // Write a string to a stream writer
        public void WriteToStreamWriter(string recordSpecName, string text)
        {
            GetStreamWriter(recordSpecName).Write(text);
        }

        // Write a line to a stream writer
        public void WriteLineToStreamWriter(string recordSpecName, string text)
        {
            GetStreamWriter(recordSpecName).WriteLine(text);
        }

        public void PrintOutputPaths()
        {
            foreach (var writer in streamWriters.Values)
            {
                Console.WriteLine(((FileStream)writer.BaseStream).Name);
            }
        }
    }
}

