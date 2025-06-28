using JVParquet.Configuration;
using JVParquet.Core;
using JVParquet.Interfaces;
using JVParquet.Services;

namespace JVParquet
{
    public class JVDataParquetConverter : IAsyncDisposable, IDisposable
    {
        private readonly IRecordParser _recordParser;
        private readonly IBufferManager<Dictionary<string, object?>> _bufferManager;
        private readonly IParquetWriter _parquetWriter;
        private readonly ParquetSettings _settings;

        public JVDataParquetConverter(string outputDir, string filePrefix = "data") 
            : this(new ParquetSettings { OutputDirectory = outputDir, FilePrefix = filePrefix })
        {
        }

        public JVDataParquetConverter(ParquetSettings settings)
        {
            _settings = settings;
            _recordParser = new RecordParser();
            _parquetWriter = new ParquetWriterManager(settings.OutputDirectory, settings.FilePrefix);
            
            Func<string, IEnumerable<Dictionary<string, object?>>, Task<Result>> flushAction = 
                async (key, items) => await _parquetWriter.WriteRecordsAsync(key, items);
            
            _bufferManager = new BufferManager<Dictionary<string, object?>>(settings, flushAction);
        }

        public async Task<Result> ProcessRecordAsync(string line)
        {
            var parseResult = _recordParser.ParseRecord(line);
            
            if (!parseResult.IsSuccess)
            {
                return Result.Failure($"Failed to parse record: {parseResult.Error}");
            }

            var parsedRecord = parseResult.Value!;
            return await _bufferManager.AddAsync(parsedRecord.RecordSpec, parsedRecord.Data);
        }

        public async ValueTask DisposeAsync()
        {
            await _bufferManager.FlushAllAsync();
            await _parquetWriter.CloseAsync();
            
            _bufferManager?.Dispose();
            _parquetWriter?.Dispose();
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}