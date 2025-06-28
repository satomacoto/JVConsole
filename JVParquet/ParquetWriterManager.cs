using Parquet;
using Parquet.Data;
using Parquet.Schema;
using Parquet.File;

namespace JVParquet
{
    public class ParquetWriterManager : IDisposable
    {
        private readonly string _outputDir;
        private readonly Dictionary<string, WriterContext> _writers;
        private readonly string _filePrefix;

        public ParquetWriterManager(string outputDir, string filePrefix = "data")
        {
            _outputDir = outputDir;
            _writers = new Dictionary<string, WriterContext>();
            _filePrefix = filePrefix;
        }

        public async Task WriteRecordsAsync(string recordSpec, List<Dictionary<string, object?>> records)
        {
            if (records.Count == 0)
            {
                return;
            }

            // WriterContextを取得または作成
            var context = await GetOrCreateWriterContextAsync(recordSpec, records[0]);

            // データを列ごとに整理
            var columnData = new Dictionary<string, List<object?>>();
            foreach (var field in context.Schema.Fields)
            {
                columnData[field.Name] = new List<object?>();
            }

            foreach (var record in records)
            {
                foreach (var field in context.Schema.Fields)
                {
                    if (record.TryGetValue(field.Name, out var value))
                    {
                        columnData[field.Name].Add(value);
                    }
                    else
                    {
                        columnData[field.Name].Add(null);
                    }
                }
            }

            // データカラムの作成
            var dataColumns = new List<DataColumn>();
            foreach (var field in context.Schema.Fields)
            {
                var dataField = (DataField)field;
                var values = columnData[field.Name];
                
                // 文字列型として処理（Phase 1）
                var stringValues = values.Select(v => v?.ToString()).ToArray();
                var column = new DataColumn(dataField, stringValues);
                dataColumns.Add(column);
            }

            // Parquetファイルへの書き込み
            using (var rowGroup = context.Writer.CreateRowGroup())
            {
                for (int i = 0; i < dataColumns.Count; i++)
                {
                    await rowGroup.WriteColumnAsync(dataColumns[i]);
                }
            }

            context.RecordCount += records.Count;
            Console.WriteLine($"Written {records.Count} records to {recordSpec} (Total: {context.RecordCount})");
        }

        private async Task<WriterContext> GetOrCreateWriterContextAsync(string recordSpec, Dictionary<string, object?> sampleRecord)
        {
            if (!_writers.ContainsKey(recordSpec))
            {
                // スキーマの作成
                var fields = new List<Field>();
                foreach (var kvp in sampleRecord.OrderBy(k => k.Key))
                {
                    // Phase 1: すべてstring型として定義
                    fields.Add(new DataField(kvp.Key, typeof(string)));
                }

                var schema = new ParquetSchema(fields);

                // パーティション情報の取得
                var partitionInfo = GetPartitionInfo(sampleRecord);
                
                // 出力ファイルパスの作成
                var filePath = GetOutputFilePath(recordSpec, partitionInfo);

                // ディレクトリの作成
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // ParquetWriterの作成
                var stream = File.OpenWrite(filePath);
                var writer = await ParquetWriter.CreateAsync(schema, stream);

                _writers[recordSpec] = new WriterContext
                {
                    Schema = schema,
                    Writer = writer,
                    Stream = stream,
                    FilePath = filePath,
                    RecordCount = 0
                };
            }

            return _writers[recordSpec];
        }

        private PartitionInfo GetPartitionInfo(Dictionary<string, object?> record)
        {
            var info = new PartitionInfo();

            // head.MakeDate.Year/Month/Dayを取得
            if (record.TryGetValue("head.MakeDate.Year", out var year))
            {
                info.Year = year?.ToString() ?? "unknown";
            }
            if (record.TryGetValue("head.MakeDate.Month", out var month))
            {
                info.Month = month?.ToString() ?? "unknown";
            }
            if (record.TryGetValue("head.MakeDate.Day", out var day))
            {
                info.Day = day?.ToString() ?? "unknown";
            }

            return info;
        }

        private string GetOutputFilePath(string recordSpec, PartitionInfo partitionInfo)
        {
            // パーティションディレクトリの作成
            var partitionPath = Path.Combine(
                _outputDir,
                recordSpec,
                $"year={partitionInfo.Year}",
                $"month={partitionInfo.Month}",
                $"day={partitionInfo.Day}"
            );

            // ファイル名（プレフィックスを使用）
            var fileName = $"{_filePrefix}.parquet";

            return Path.Combine(partitionPath, fileName);
        }

        public void Dispose()
        {
            foreach (var context in _writers.Values)
            {
                try
                {
                    context.Writer?.Dispose();
                    context.Stream?.Dispose();
                    Console.WriteLine($"Closed {context.FilePath} with {context.RecordCount} records");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing writer for {context.FilePath}: {ex.Message}");
                }
            }

            _writers.Clear();
        }

        private class WriterContext
        {
            public required ParquetSchema Schema { get; set; }
            public required ParquetWriter Writer { get; set; }
            public required Stream Stream { get; set; }
            public required string FilePath { get; set; }
            public int RecordCount { get; set; }
        }

        private class PartitionInfo
        {
            public string Year { get; set; } = "unknown";
            public string Month { get; set; } = "unknown";
            public string Day { get; set; } = "unknown";
        }
    }
}