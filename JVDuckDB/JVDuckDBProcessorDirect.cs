using System.Text;
using System.Text.Json;
using Parquet;
using Parquet.Data;
using Parquet.Schema;
using JVDuckDB.TypeMapping;

namespace JVDuckDB
{
    public class JVDuckDBProcessorDirect
    {
        private readonly ConvertOptions _options;
        private readonly HashSet<string> _skipRecordSpecs;
        private readonly Dictionary<string, int> _recordCounts = new();
        private readonly Dictionary<string, List<(string path, string recordSpec, DateTime makeDate, int recordCount)>> _indexData = new();
        private int _totalRecords = 0;
        private readonly JVDuckDBProcessorBase_UTF8 _parser;
        private readonly TypeMappingManager _typeMappingManager;

        public JVDuckDBProcessorDirect(ConvertOptions options)
        {
            _options = options;
            _skipRecordSpecs = new HashSet<string>(options.SkipRecordSpecs);
            _parser = new JVDuckDBProcessorBase_UTF8();
            _typeMappingManager = new TypeMappingManager();
        }

        public async Task ProcessAsync()
        {
            // 1. 入力ファイルの取得
            var inputFiles = GetInputFiles();

            // 2. 各ファイルを直接Parquetに変換
            foreach (var (file, index) in inputFiles.Select((f, i) => (f, i)))
            {
                await ProcessFileDirectAsync(file);
            }

            // 3. インデックステーブルの作成
            await CreateIndexTableAsync();

            // 結果サマリー
            PrintSummary();
        }

        private string[] GetInputFiles()
        {
            if (File.Exists(_options.InputPath))
            {
                return new[] { _options.InputPath };
            }
            else if (Directory.Exists(_options.InputPath))
            {
                return Directory.GetFiles(_options.InputPath, "JV-*.txt", SearchOption.AllDirectories)
                    .OrderBy(f => f)
                    .ToArray();
            }
            else
            {
                throw new FileNotFoundException($"入力パスが見つかりません: {_options.InputPath}");
            }
        }

        private async Task ProcessFileDirectAsync(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            // JVDownloaderがUTF-8で保存しているため、UTF-8として読み込む
            using var reader = new StreamReader(filePath, Encoding.UTF8);
            
            // レコード種別ごとにグループ化
            var recordGroups = new Dictionary<string, List<Dictionary<string, object?>>>();
            string? line;
            int lineCount = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineCount++;
                if (string.IsNullOrWhiteSpace(line) || line.Length < 2)
                    continue;

                var recordSpec = line.Substring(0, 2);
                
                // スキップ対象のレコード種別をチェック
                if (_skipRecordSpecs.Contains(recordSpec))
                    continue;

                // フィルター対象のレコード種別をチェック
                if (_options.FilterRecordSpecs.Any() && !_options.FilterRecordSpecs.Contains(recordSpec))
                    continue;

                try
                {
                    var parsed = _parser.ParseRecord(line);
                    if (parsed != null)
                    {
                        if (!recordGroups.ContainsKey(recordSpec))
                            recordGroups[recordSpec] = new List<Dictionary<string, object?>>();

                        recordGroups[recordSpec].Add(parsed);
                        _totalRecords++;
                    }
                }
                catch (Exception ex)
                {
                    if (_options.Verbose)
                    {
                        // パースエラーは無視
                    }
                }
            }

            // レコード種別ごとにParquetファイルに書き込み
            foreach (var kvp in recordGroups)
            {
                var recordSpec = kvp.Key;
                var records = kvp.Value;
                
                if (records.Count == 0) continue;

                // MakeDateでグループ化
                var dateGroups = records.GroupBy(r => 
                {
                    var year = r["head_MakeDate_Year"]?.ToString() ?? "0000";
                    var month = r["head_MakeDate_Month"]?.ToString() ?? "00";
                    var day = r["head_MakeDate_Day"]?.ToString() ?? "00";
                    return new { Year = year, Month = month, Day = day };
                });

                foreach (var dateGroup in dateGroups)
                {
                    var partitionPath = Path.Combine(
                        _options.OutputPath,
                        recordSpec,
                        $"year={dateGroup.Key.Year}",
                        $"month={dateGroup.Key.Month}",
                        $"day={dateGroup.Key.Day}"
                    );

                    Directory.CreateDirectory(partitionPath);

                    // ファイル名にタイムスタンプを含める（Append対応）
                    var outputFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}.parquet";
                    var outputPath = Path.Combine(partitionPath, outputFileName);

                    // Parquetファイルに書き込み
                    await WriteParquetFileAsync(outputPath, recordSpec, dateGroup.ToList());

                    // インデックス情報を記録
                    var makeDate = new DateTime(
                        int.Parse(dateGroup.Key.Year),
                        int.Parse(dateGroup.Key.Month),
                        int.Parse(dateGroup.Key.Day)
                    );
                    
                    if (!_indexData.ContainsKey(recordSpec))
                        _indexData[recordSpec] = new List<(string, string, DateTime, int)>();
                    
                    _indexData[recordSpec].Add((outputPath, recordSpec, makeDate, dateGroup.Count()));

                    // レコード数を更新
                    if (!_recordCounts.ContainsKey(recordSpec))
                        _recordCounts[recordSpec] = 0;
                    _recordCounts[recordSpec] += dateGroup.Count();
                }
            }
        }

        private async Task WriteParquetFileAsync(string outputPath, string recordSpec, List<Dictionary<string, object?>> records)
        {
            if (records.Count == 0) return;

            // カラムを動的に生成
            var firstRecord = records.First();
            var fields = new List<DataField>();
            
            foreach (var kvp in firstRecord)
            {
                // 型を推定
                var value = kvp.Value;
                DataField field;
                
                // TypeMappingManagerから型を取得
                var key = kvp.Key;
                var fieldType = _typeMappingManager.GetFieldType(recordSpec, key);
                field = new DataField(kvp.Key, fieldType);
                
                // デバッグ: 型マッピングの確認（最初の数フィールドのみ）
                if (_options.Verbose && fields.Count < 10)
                {
                    // 型マッピング情報は無視
                }
                
                fields.Add(field);
            }

            var schema = new ParquetSchema(fields);

            using var file = File.Create(outputPath);
            using var writer = await ParquetWriter.CreateAsync(schema, file);
            
            // データを列形式に変換
            var columns = new Dictionary<string, List<object?>>();
            foreach (var field in fields)
            {
                columns[field.Name] = new List<object?>();
            }

            foreach (var record in records)
            {
                foreach (var field in fields)
                {
                    var value = record.ContainsKey(field.Name) ? record[field.Name] : null;
                    columns[field.Name].Add(value);
                }
            }

            // 行グループを作成
            using var rowGroup = writer.CreateRowGroup();
            
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var columnData = columns[field.Name];
                
                if (field.ClrType == typeof(string))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => v?.ToString()).ToArray()));
                }
                else if (field.ClrType == typeof(long))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => 
                    {
                        if (v == null) return 0L;
                        var str = v.ToString();
                        if (string.IsNullOrWhiteSpace(str)) return 0L;
                        return long.TryParse(str, out var result) ? result : 0L;
                    }).ToArray()));
                }
                else if (field.ClrType == typeof(int))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => 
                    {
                        if (v == null) return 0;
                        var str = v.ToString();
                        if (string.IsNullOrWhiteSpace(str)) return 0;
                        return int.TryParse(str, out var result) ? result : 0;
                    }).ToArray()));
                }
                else if (field.ClrType == typeof(double))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => 
                    {
                        if (v == null) return 0.0;
                        var str = v.ToString();
                        if (string.IsNullOrWhiteSpace(str)) return 0.0;
                        return double.TryParse(str, out var result) ? result : 0.0;
                    }).ToArray()));
                }
                else if (field.ClrType == typeof(bool))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => v != null ? Convert.ToBoolean(v) : false).ToArray()));
                }
                else if (field.ClrType == typeof(DateTime))
                {
                    await rowGroup.WriteColumnAsync(new DataColumn(field, columnData.Select(v => 
                    {
                        if (v == null) return DateTime.MinValue;
                        if (v is DateTime dt) return dt;
                        return DateTime.MinValue;
                    }).ToArray()));
                }
            }

            if (_options.Verbose)
            {
                // 書き込み完了メッセージは無視
            }
        }

        private async Task CreateIndexTableAsync()
        {
            var indexPath = Path.Combine(_options.OutputPath, "_index");
            Directory.CreateDirectory(indexPath);

            var indexFile = Path.Combine(indexPath, "jv_partition_index.parquet");

            // インデックスデータを準備
            var indexRecords = new List<Dictionary<string, object?>>();

            foreach (var kvp in _indexData)
            {
                foreach (var (path, recordSpec, makeDate, recordCount) in kvp.Value)
                {
                    indexRecords.Add(new Dictionary<string, object?>
                    {
                        ["file_path"] = Path.GetRelativePath(_options.OutputPath, path),
                        ["record_spec"] = recordSpec,
                        ["make_date"] = makeDate.ToString("yyyy-MM-dd"),
                        ["year"] = makeDate.Year,
                        ["month"] = makeDate.Month,
                        ["day"] = makeDate.Day,
                        ["record_count"] = recordCount,
                        ["created_at"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }

            if (indexRecords.Count > 0)
            {
                await WriteIndexParquetFileAsync(indexFile, indexRecords);
            }

        }

        private async Task WriteIndexParquetFileAsync(string outputPath, List<Dictionary<string, object?>> records)
        {
            var schema = new ParquetSchema(
                new DataField("file_path", typeof(string)),
                new DataField("record_spec", typeof(string)),
                new DataField("make_date", typeof(string)),
                new DataField("year", typeof(int)),
                new DataField("month", typeof(int)),
                new DataField("day", typeof(int)),
                new DataField("record_count", typeof(int)),
                new DataField("created_at", typeof(string))
            );

            using var file = File.Create(outputPath);
            using var writer = await ParquetWriter.CreateAsync(schema, file);
            using var rowGroup = writer.CreateRowGroup();

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[0],
                records.Select(r => r["file_path"]?.ToString() ?? "").ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[1],
                records.Select(r => r["record_spec"]?.ToString() ?? "").ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[2],
                records.Select(r => r["make_date"]?.ToString() ?? "").ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[3],
                records.Select(r => Convert.ToInt32(r["year"])).ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[4],
                records.Select(r => Convert.ToInt32(r["month"])).ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[5],
                records.Select(r => Convert.ToInt32(r["day"])).ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[6],
                records.Select(r => Convert.ToInt32(r["record_count"])).ToArray()
            ));

            await rowGroup.WriteColumnAsync(new DataColumn(
                schema.DataFields[7],
                records.Select(r => r["created_at"]?.ToString() ?? "").ToArray()
            ));
        }

        private void PrintSummary()
        {
            // サマリー表示は無視
        }
    }
}