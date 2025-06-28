namespace JVParquet.Configuration
{
    public class ParquetSettings
    {
        public int BatchSize { get; set; } = Constants.DefaultBatchSize;
        public string OutputDirectory { get; set; } = string.Empty;
        public string FilePrefix { get; set; } = "data";
        public bool EnableCompression { get; set; } = true;
        public string CompressionType { get; set; } = "Snappy";
        public int MaxParallelism { get; set; } = Environment.ProcessorCount;
        public bool EnableStatistics { get; set; } = true;
    }
}