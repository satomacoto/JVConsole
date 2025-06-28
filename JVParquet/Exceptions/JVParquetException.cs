namespace JVParquet.Exceptions
{
    public class JVParquetException : Exception
    {
        public string RecordSpec { get; }
        public int? LineNumber { get; }

        public JVParquetException(string message) : base(message)
        {
            RecordSpec = string.Empty;
        }

        public JVParquetException(string message, Exception innerException) 
            : base(message, innerException)
        {
            RecordSpec = string.Empty;
        }

        public JVParquetException(string message, string recordSpec, int? lineNumber = null) 
            : base(message)
        {
            RecordSpec = recordSpec;
            LineNumber = lineNumber;
        }

        public JVParquetException(string message, string recordSpec, Exception innerException, int? lineNumber = null) 
            : base(message, innerException)
        {
            RecordSpec = recordSpec;
            LineNumber = lineNumber;
        }
    }

    public class RecordParsingException : JVParquetException
    {
        public string RawData { get; }

        public RecordParsingException(string message, string recordSpec, string rawData, int? lineNumber = null)
            : base($"Failed to parse record type '{recordSpec}' at line {lineNumber}: {message}", recordSpec, lineNumber)
        {
            RawData = rawData;
        }
    }

    public class ParquetWriteException : JVParquetException
    {
        public string FilePath { get; }

        public ParquetWriteException(string message, string filePath, Exception innerException)
            : base($"Failed to write Parquet file '{filePath}': {message}", innerException)
        {
            FilePath = filePath;
        }
    }
}