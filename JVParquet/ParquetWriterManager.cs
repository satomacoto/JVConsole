using Parquet;
using Parquet.Data;
using Parquet.Schema;
using Parquet.File;
using JVParquet.Core;
using JVParquet.Interfaces;
using JVParquet.Exceptions;

namespace JVParquet
{
    public class ParquetWriterManager : IParquetWriter
    {
        private readonly string _outputDir;
        private readonly Dictionary<string, WriterContext> _writers;
        private readonly string _filePrefix;
        private readonly TypeDefinitionLoader _typeLoader;

        public ParquetWriterManager(string outputDir, string filePrefix = "data")
        {
            _outputDir = outputDir;
            _writers = new Dictionary<string, WriterContext>();
            _filePrefix = filePrefix;
            _typeLoader = new TypeDefinitionLoader();
            _typeLoader.LoadHardcodedDefinitions();
        }

        public async Task<Result> WriteRecordsAsync(string recordSpec, IEnumerable<Dictionary<string, object?>> records)
        {
            try
            {
                var recordList = records.ToList();
                if (recordList.Count == 0)
                {
                    return Result.Success();
                }

                // WriterContextを取得または作成
                var context = await GetOrCreateWriterContextAsync(recordSpec, recordList[0]);

            // データを列ごとに整理
            var columnData = new Dictionary<string, List<object?>>();
            foreach (var field in context.Schema.Fields)
            {
                columnData[field.Name] = new List<object?>();
            }

                foreach (var record in recordList)
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

            // データカラムの作成（型定義を使用）
            var dataColumns = new List<DataColumn>();
            foreach (var field in context.Schema.Fields)
            {
                var dataField = (DataField)field;
                var values = columnData[field.Name];
                
                // 型に応じて値を変換（JRA-VAN仕様に準拠）
                if (dataField.ClrType == typeof(int))
                {
                    var intValues = values.Select(v => 
                    {
                        if (v is int intVal) return intVal;
                        var converted = _typeLoader.ConvertValue(v?.ToString(), typeof(int));
                        return converted is int ? (int)converted : 0;
                    }).ToArray();
                    dataColumns.Add(new DataColumn(dataField, intValues));
                }
                else if (dataField.ClrType == typeof(long))
                {
                    var longValues = values.Select(v => 
                    {
                        if (v is long longVal) return longVal;
                        var converted = _typeLoader.ConvertValue(v?.ToString(), typeof(long));
                        return converted is long ? (long)converted : 0L;
                    }).ToArray();
                    dataColumns.Add(new DataColumn(dataField, longValues));
                }
                else if (dataField.ClrType == typeof(decimal))
                {
                    var decimalValues = values.Select(v => 
                    {
                        if (v is decimal decVal) return decVal;
                        var converted = _typeLoader.ConvertValue(v?.ToString(), typeof(decimal));
                        return converted is decimal ? (decimal)converted : 0m;
                    }).ToArray();
                    dataColumns.Add(new DataColumn(dataField, decimalValues));
                }
                else
                {
                    // string型（空白はtrimされて空文字列になる）
                    var stringValues = values.Select(v => 
                    {
                        var str = v?.ToString();
                        return string.IsNullOrEmpty(str) ? string.Empty : str.Trim();
                    }).ToArray();
                    dataColumns.Add(new DataColumn(dataField, stringValues));
                }
            }

            // Parquetファイルへの書き込み
            using (var rowGroup = context.Writer.CreateRowGroup())
            {
                for (int i = 0; i < dataColumns.Count; i++)
                {
                    await rowGroup.WriteColumnAsync(dataColumns[i]);
                }
                }

                context.RecordCount += recordList.Count;
                Console.WriteLine($"Written {recordList.Count} records to {recordSpec} (Total: {context.RecordCount})");
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new ParquetWriteException(
                    $"Failed to write records for {recordSpec}", 
                    $"{_outputDir}/{recordSpec}", 
                    ex));
            }
        }

        private async Task<WriterContext> GetOrCreateWriterContextAsync(string recordSpec, Dictionary<string, object?> sampleRecord)
        {
            if (!_writers.ContainsKey(recordSpec))
            {
                // スキーマの作成（型定義を使用）
                var fields = new List<Field>();
                foreach (var kvp in sampleRecord.OrderBy(k => k.Key))
                {
                    // 型定義から適切な型を取得
                    var columnType = _typeLoader.GetColumnType(recordSpec, kvp.Key);
                    fields.Add(new DataField(kvp.Key, columnType));
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

            // head_MakeDate_Year/Month/Dayを取得（新しい命名規則）
            if (record.TryGetValue(Constants.FieldNames.HeadMakeDateYear, out var year))
            {
                info.Year = year?.ToString() ?? Constants.PartitionKeys.Unknown;
            }
            if (record.TryGetValue(Constants.FieldNames.HeadMakeDateMonth, out var month))
            {
                info.Month = month?.ToString() ?? Constants.PartitionKeys.Unknown;
            }
            if (record.TryGetValue(Constants.FieldNames.HeadMakeDateDay, out var day))
            {
                info.Day = day?.ToString() ?? Constants.PartitionKeys.Unknown;
            }

            return info;
        }

        private string GetOutputFilePath(string recordSpec, PartitionInfo partitionInfo)
        {
            // パーティションディレクトリの作成
            var partitionPath = Path.Combine(
                _outputDir,
                recordSpec,
                $"{Constants.PartitionKeys.Year}={partitionInfo.Year}",
                $"{Constants.PartitionKeys.Month}={partitionInfo.Month}",
                $"{Constants.PartitionKeys.Day}={partitionInfo.Day}"
            );

            // ファイル名（プレフィックスを使用）
            var fileName = $"{_filePrefix}{Constants.FileExtensions.Parquet}";

            return Path.Combine(partitionPath, fileName);
        }

        public Task<Result> CloseAsync()
        {
            try
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

                // 未定義カラムのサマリーを出力
                _typeLoader.PrintUndefinedColumnsSummary();
                
                return Task.FromResult(Result.Success());
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure(ex));
            }
        }

        public void Dispose()
        {
            CloseAsync().GetAwaiter().GetResult();
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
            public string Year { get; set; } = Constants.PartitionKeys.Unknown;
            public string Month { get; set; } = Constants.PartitionKeys.Unknown;
            public string Day { get; set; } = Constants.PartitionKeys.Unknown;
        }
    }
}