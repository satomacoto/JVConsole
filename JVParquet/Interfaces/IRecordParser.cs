using JVParquet.Core;

namespace JVParquet.Interfaces
{
    public interface IRecordParser
    {
        Result<ParsedRecord> ParseRecord(string line);
    }

    public class ParsedRecord
    {
        public string RecordSpec { get; set; } = string.Empty;
        public Dictionary<string, object?> Data { get; set; } = new();
        public DateTime? MakeDate { get; set; }
    }
}