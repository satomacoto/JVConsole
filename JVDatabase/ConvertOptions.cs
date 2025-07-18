namespace JVDatabase
{
    public class ConvertOptions
    {
        public string InputPath { get; set; } = "";
        public string OutputPath { get; set; } = "";
        public IEnumerable<string> SkipRecordSpecs { get; set; } = Enumerable.Empty<string>();
        public int BatchSize { get; set; } = 10000;
        public string DatabasePath { get; set; } = ":memory:";
        public IEnumerable<string> FilterRecordSpecs { get; set; } = Enumerable.Empty<string>();
        public bool Deduplicate { get; set; } = true;
        public bool Verbose { get; set; } = false;
    }
}