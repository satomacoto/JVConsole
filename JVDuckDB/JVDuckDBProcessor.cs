using DuckDB.NET.Data;
using System.Text;
using System.Text.Json;
using static JVData_Struct;

namespace JVDuckDB
{
    public class JVDuckDBProcessor
    {
        private readonly ConvertOptions _options;
        private readonly Dictionary<string, List<string>> _pkMapping;
        private readonly HashSet<string> _skipRecordSpecs;
        private readonly Dictionary<string, int> _recordCounts = new();
        private int _totalRecords = 0;

        public JVDuckDBProcessor(ConvertOptions options)
        {
            _options = options;
            _pkMapping = RecordIndexMapping.IndexColumns;
            _skipRecordSpecs = new HashSet<string>(options.SkipRecordSpecs);
        }

        public async Task ProcessAsync()
        {
            using var connection = new DuckDBConnection($"DataSource={_options.DatabasePath}");
            await connection.OpenAsync();

            try
            {
                // 1. 入力ファイルの取得
                var inputFiles = GetInputFiles();

                // 2. 各ファイルを処理
                foreach (var (file, index) in inputFiles.Select((f, i) => (f, i)))
                {
                    await ProcessFileAsync(connection, file);
                }

                // 3. 重複排除とParquet出力
                await DeduplicateAndExportAsync(connection);

                // 4. インデックステーブルの作成
                await CreateIndexTableAsync(connection);

                // 結果サマリー
                PrintSummary();
            }
            finally
            {
                await connection.CloseAsync();
            }
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

        private async Task ProcessFileAsync(DuckDBConnection connection, string filePath)
        {
            using var reader = new StreamReader(filePath, Encoding.GetEncoding(932));
            var records = new Dictionary<string, List<Dictionary<string, object?>>>();
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
                    var parsed = ParseRecord(line);
                    if (parsed != null)
                    {
                        if (!records.ContainsKey(recordSpec))
                            records[recordSpec] = new List<Dictionary<string, object?>>();

                        records[recordSpec].Add(parsed);
                        _totalRecords++;

                        // バッチサイズに達したら書き込み
                        if (records[recordSpec].Count >= _options.BatchSize)
                        {
                            await WriteBatchToTableAsync(connection, recordSpec, records[recordSpec]);
                            records[recordSpec].Clear();
                        }
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

            // 残りのレコードを書き込み
            foreach (var kvp in records.Where(r => r.Value.Count > 0))
            {
                await WriteBatchToTableAsync(connection, kvp.Key, kvp.Value);
            }
        }

        private Dictionary<string, object?>? ParseRecord(string line)
        {
            var bytes = Encoding.GetEncoding(932).GetBytes(line);
            var recordSpec = line.Substring(0, 2);

            // 共通ヘッダーの解析
            var record = new Dictionary<string, object?>
            {
                ["record_spec"] = recordSpec,
                ["data_kubun"] = line.Substring(2, 1),
                ["head_MakeDate_Year"] = line.Substring(3, 4),
                ["head_MakeDate_Month"] = line.Substring(7, 2),
                ["head_MakeDate_Day"] = line.Substring(9, 2),
                ["raw_data"] = line // 元データも保存（デバッグ用）
            };

            // レコード種別ごとの解析
            switch (recordSpec)
            {
                case "RA":
                    ParseRARecord(bytes, record);
                    break;
                case "SE":
                    ParseSERecord(bytes, record);
                    break;
                case "UM":
                    ParseUMRecord(bytes, record);
                    break;
                case "KS":
                    ParseKSRecord(bytes, record);
                    break;
                case "CH":
                    ParseCHRecord(bytes, record);
                    break;
                case "HN":
                    ParseHNRecord(bytes, record);
                    break;
                case "SK":
                    ParseSKRecord(bytes, record);
                    break;
                case "JG":
                    ParseJGRecord(bytes, record);
                    break;
                case "H1":
                    ParseH1Record(bytes, record);
                    break;
                case "H6":
                    ParseH6Record(bytes, record);
                    break;
                case "AV":
                    ParseAVRecord(bytes, record);
                    break;
                case "BN":
                    ParseBNRecord(bytes, record);
                    break;
                case "BR":
                    ParseBRRecord(bytes, record);
                    break;
                case "BT":
                    ParseBTRecord(bytes, record);
                    break;
                case "CC":
                    ParseCCRecord(bytes, record);
                    break;
                case "CK":
                    ParseCKRecord(bytes, record);
                    break;
                case "CS":
                    ParseCSRecord(bytes, record);
                    break;
                case "DM":
                    ParseDMRecord(bytes, record);
                    break;
                case "HC":
                    ParseHCRecord(bytes, record);
                    break;
                case "HR":
                    ParseHRRecord(bytes, record);
                    break;
                case "HS":
                    ParseHSRecord(bytes, record);
                    break;
                case "HY":
                    ParseHYRecord(bytes, record);
                    break;
                case "JC":
                    ParseJCRecord(bytes, record);
                    break;
                case "O1":
                    ParseO1Record(bytes, record);
                    break;
                case "O2":
                    ParseO2Record(bytes, record);
                    break;
                case "O3":
                    ParseO3Record(bytes, record);
                    break;
                case "O4":
                    ParseO4Record(bytes, record);
                    break;
                case "O5":
                    ParseO5Record(bytes, record);
                    break;
                case "O6":
                    ParseO6Record(bytes, record);
                    break;
                case "RC":
                    ParseRCRecord(bytes, record);
                    break;
                case "TC":
                    ParseTCRecord(bytes, record);
                    break;
                case "TK":
                    ParseTKRecord(bytes, record);
                    break;
                case "TM":
                    ParseTMRecord(bytes, record);
                    break;
                case "WC":
                    ParseWCRecord(bytes, record);
                    break;
                case "WE":
                    ParseWERecord(bytes, record);
                    break;
                case "WF":
                    ParseWFRecord(bytes, record);
                    break;
                case "WH":
                    ParseWHRecord(bytes, record);
                    break;
                case "YS":
                    ParseYSRecord(bytes, record);
                    break;
                // 他のレコード種別も必要に応じて実装
                default:
                    // 未実装のレコード種別は基本情報のみ
                    if (_options.Verbose)
                    {
                        // 未実装メッセージは無視
                    }
                    break;
            }

            return record;
        }

        private void ParseRARecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);

            // レース情報
            record["YoubiCD"] = MidB2S(ref bytes, 27, 1);
            record["TokuNum"] = MidB2S(ref bytes, 28, 4);
            record["Hondai"] = MidB2S(ref bytes, 32, 60).Trim();
            record["Fukudai"] = MidB2S(ref bytes, 92, 60).Trim();
            record["Kakko"] = MidB2S(ref bytes, 152, 60).Trim();
            record["HondaiEng"] = MidB2S(ref bytes, 212, 120).Trim();
            record["FukudaiEng"] = MidB2S(ref bytes, 332, 120).Trim();
            record["KakkoEng"] = MidB2S(ref bytes, 452, 120).Trim();
            record["Ryakusyo10"] = MidB2S(ref bytes, 572, 20).Trim();
            record["Ryakusyo6"] = MidB2S(ref bytes, 592, 12).Trim();
            record["Ryakusyo3"] = MidB2S(ref bytes, 604, 6).Trim();
            record["Kubun"] = MidB2S(ref bytes, 610, 1);
            record["Nkai"] = MidB2S(ref bytes, 611, 3).Trim();

            // 競走条件
            record["GradeCD"] = MidB2S(ref bytes, 614, 1);
            record["GradeCDBefore"] = MidB2S(ref bytes, 615, 1);
            record["SyubetuCD"] = MidB2S(ref bytes, 616, 2);
            record["KigoCD"] = MidB2S(ref bytes, 618, 3);
            record["JyuryoCD"] = MidB2S(ref bytes, 621, 1);
            record["JyokenCD1"] = MidB2S(ref bytes, 622, 3);
            record["JyokenCD2"] = MidB2S(ref bytes, 625, 3);
            record["JyokenCD3"] = MidB2S(ref bytes, 628, 3);
            record["JyokenCD4"] = MidB2S(ref bytes, 631, 3);
            record["JyokenCD5"] = MidB2S(ref bytes, 634, 3);
            record["JyokenName"] = MidB2S(ref bytes, 637, 60).Trim();
            record["Kyori"] = MidB2S(ref bytes, 697, 4);
            record["KyoriBefore"] = MidB2S(ref bytes, 701, 4);
            record["TrackCD"] = MidB2S(ref bytes, 705, 2);
            record["TrackCDBefore"] = MidB2S(ref bytes, 707, 2);
            record["CourseKubunCD"] = MidB2S(ref bytes, 709, 2);
            record["CourseKubunCDBefore"] = MidB2S(ref bytes, 711, 2);

            // 本賞金
            record["Honsyokin1"] = MidB2S(ref bytes, 713, 8);
            record["Honsyokin2"] = MidB2S(ref bytes, 721, 8);
            record["Honsyokin3"] = MidB2S(ref bytes, 729, 8);
            record["Honsyokin4"] = MidB2S(ref bytes, 737, 8);
            record["Honsyokin5"] = MidB2S(ref bytes, 745, 8);
            record["HonsyokinBefore1"] = MidB2S(ref bytes, 753, 8);
            record["HonsyokinBefore2"] = MidB2S(ref bytes, 761, 8);
            record["HonsyokinBefore3"] = MidB2S(ref bytes, 769, 8);
            record["HonsyokinBefore4"] = MidB2S(ref bytes, 777, 8);
            record["HonsyokinBefore5"] = MidB2S(ref bytes, 785, 8);

            // その他
            record["HassoTime"] = MidB2S(ref bytes, 793, 4);
            record["HassoTimeBefore"] = MidB2S(ref bytes, 797, 4);
            record["TorokuTosu"] = MidB2S(ref bytes, 801, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 803, 2);
            record["NyusenTosu"] = MidB2S(ref bytes, 805, 2);
            record["TenkoCD"] = MidB2S(ref bytes, 807, 1);
            record["SibaBabaCD"] = MidB2S(ref bytes, 808, 1);
            record["DirtBabaCD"] = MidB2S(ref bytes, 809, 1);

            // データ区分
            record["DataKubun"] = MidB2S(ref bytes, 823, 1);
            record["SakkujoTosu"] = MidB2S(ref bytes, 824, 2);
            record["ChuteiTosu"] = MidB2S(ref bytes, 826, 2);
        }

        private void ParseSERecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);

            // 馬情報
            record["Wakuban"] = MidB2S(ref bytes, 27, 1);
            record["Umaban"] = MidB2S(ref bytes, 28, 2);
            record["KettoNum"] = MidB2S(ref bytes, 30, 10);
            record["Bamei"] = MidB2S(ref bytes, 40, 36).Trim();
            record["UmaKigoCD"] = MidB2S(ref bytes, 76, 2);
            record["SexCD"] = MidB2S(ref bytes, 78, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 79, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 80, 2);

            // 馬の情報
            record["Barei"] = MidB2S(ref bytes, 82, 2);
            record["TozaiCD"] = MidB2S(ref bytes, 84, 1);
            record["ChokyosiCode"] = MidB2S(ref bytes, 85, 5);
            record["ChokyosiName"] = MidB2S(ref bytes, 90, 34).Trim();
            record["BanusiCode"] = MidB2S(ref bytes, 124, 6);
            record["BanusiName"] = MidB2S(ref bytes, 130, 64).Trim();
            record["Fukusyoku"] = MidB2S(ref bytes, 194, 60).Trim();
            record["reserved1"] = MidB2S(ref bytes, 254, 60);
            record["Futan"] = MidB2S(ref bytes, 314, 3);
            record["FutanBefore"] = MidB2S(ref bytes, 317, 3);
            record["Blinker"] = MidB2S(ref bytes, 320, 1);
            record["reserved2"] = MidB2S(ref bytes, 321, 1);

            // 騎手情報
            record["KisyuCode"] = MidB2S(ref bytes, 322, 5);
            record["KisyuCodeBefore"] = MidB2S(ref bytes, 327, 5);
            record["KisyuName"] = MidB2S(ref bytes, 332, 34).Trim();
            record["KisyuNameBefore"] = MidB2S(ref bytes, 366, 34).Trim();
            record["MinaraiCD"] = MidB2S(ref bytes, 400, 1);
            record["MinaraiCDBefore"] = MidB2S(ref bytes, 401, 1);
            record["BaTaijyu"] = MidB2S(ref bytes, 402, 3);
            record["ZogenFugo"] = MidB2S(ref bytes, 405, 1);
            record["ZogenSa"] = MidB2S(ref bytes, 406, 3);

            // 成績情報
            record["IJyoCD"] = MidB2S(ref bytes, 409, 1);
            record["NyusenJyuni"] = MidB2S(ref bytes, 410, 2);
            record["KakuteiJyuni"] = MidB2S(ref bytes, 412, 2);
            record["DochakuKubun"] = MidB2S(ref bytes, 414, 1);
            record["DochakuTosu"] = MidB2S(ref bytes, 415, 1);
            record["Time"] = MidB2S(ref bytes, 416, 4);
            record["ChakusaCD"] = MidB2S(ref bytes, 420, 3);
            record["ChakusaCDP"] = MidB2S(ref bytes, 423, 3);
            record["ChakusaCDPP"] = MidB2S(ref bytes, 426, 3);
            record["Jyuni1c"] = MidB2S(ref bytes, 429, 2);
            record["Jyuni2c"] = MidB2S(ref bytes, 431, 2);
            record["Jyuni3c"] = MidB2S(ref bytes, 433, 2);
            record["Jyuni4c"] = MidB2S(ref bytes, 435, 2);
            record["Odds"] = MidB2S(ref bytes, 437, 4);
            record["Ninki"] = MidB2S(ref bytes, 441, 2);
            record["Honsyokin"] = MidB2S(ref bytes, 443, 8);
            record["Fukasyokin"] = MidB2S(ref bytes, 451, 8);
        }

        private void ParseUMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["KettoNum"] = MidB2S(ref bytes, 11, 10);
            record["DelKubun"] = MidB2S(ref bytes, 21, 1);
            record["RegDate"] = MidB2S(ref bytes, 22, 8);
            record["DelDate"] = MidB2S(ref bytes, 30, 8);
            record["BirthDate"] = MidB2S(ref bytes, 38, 8);
            record["Bamei"] = MidB2S(ref bytes, 46, 36).Trim();
            record["BameiKana"] = MidB2S(ref bytes, 82, 40).Trim();
            record["BameiEng"] = MidB2S(ref bytes, 122, 80).Trim();
            record["ZaikyuFlag"] = MidB2S(ref bytes, 202, 1);
            record["Reserved"] = MidB2S(ref bytes, 203, 19);
            record["UmaKigoCD"] = MidB2S(ref bytes, 222, 2);
            record["SexCD"] = MidB2S(ref bytes, 224, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 225, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 226, 2);
        }

        private void ParseKSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["KisyuCode"] = MidB2S(ref bytes, 11, 5);
            record["DelKubun"] = MidB2S(ref bytes, 16, 1);
            record["IssueDate"] = MidB2S(ref bytes, 17, 8);
            record["DelDate"] = MidB2S(ref bytes, 25, 8);
            record["BirthDate"] = MidB2S(ref bytes, 33, 8);
            record["KisyuName"] = MidB2S(ref bytes, 41, 34).Trim();
            record["KisyuNameKana"] = MidB2S(ref bytes, 75, 30).Trim();
            record["KisyuRyakusyo"] = MidB2S(ref bytes, 105, 8).Trim();
            record["KisyuNameEng"] = MidB2S(ref bytes, 113, 70).Trim();
            record["SexCD"] = MidB2S(ref bytes, 183, 1);
            record["SikakuCD"] = MidB2S(ref bytes, 184, 1);
        }

        private void ParseCHRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["ChokyosiCode"] = MidB2S(ref bytes, 11, 5);
            record["DelKubun"] = MidB2S(ref bytes, 16, 1);
            record["IssueDate"] = MidB2S(ref bytes, 17, 8);
            record["DelDate"] = MidB2S(ref bytes, 25, 8);
            record["BirthDate"] = MidB2S(ref bytes, 33, 8);
            record["ChokyosiName"] = MidB2S(ref bytes, 41, 34).Trim();
            record["ChokyosiNameKana"] = MidB2S(ref bytes, 75, 30).Trim();
            record["ChokyosiRyakusyo"] = MidB2S(ref bytes, 105, 8).Trim();
            record["ChokyosiNameEng"] = MidB2S(ref bytes, 113, 70).Trim();
            record["SexCD"] = MidB2S(ref bytes, 183, 1);
            record["TozaiCD"] = MidB2S(ref bytes, 184, 1);
            record["Syotai"] = MidB2S(ref bytes, 185, 3);
        }

        private async Task WriteBatchToTableAsync(DuckDBConnection connection, string recordSpec, List<Dictionary<string, object?>> records)
        {
            if (records.Count == 0) return;

            var tableName = $"{recordSpec}_staging";
            
            // テーブルが存在しない場合は作成
            if (!await TableExistsAsync(connection, tableName))
            {
                await CreateStagingTableAsync(connection, tableName, records[0]);
            }

            // データを挿入
            var columns = records[0].Keys.ToList();
            var placeholders = string.Join(", ", columns.Select(_ => "?"));
            var insertSql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({placeholders})";

            if (_options.Verbose)
            {
                // SQLログは無視
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = insertSql;

            foreach (var record in records)
            {
                cmd.Parameters.Clear();
                foreach (var column in columns)
                {
                    var param = cmd.CreateParameter();
                    param.Value = record[column] ?? DBNull.Value;
                    cmd.Parameters.Add(param);
                }

                await cmd.ExecuteNonQueryAsync();
            }

            // カウントを更新
            if (!_recordCounts.ContainsKey(recordSpec))
                _recordCounts[recordSpec] = 0;
            _recordCounts[recordSpec] += records.Count;

            if (_options.Verbose)
            {
                // 書き込み結果は無視
            }
        }

        private async Task<bool> TableExistsAsync(DuckDBConnection connection, string tableName)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{tableName}'";
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        private async Task CreateStagingTableAsync(DuckDBConnection connection, string tableName, Dictionary<string, object?> sampleRecord)
        {
            var columns = sampleRecord.Select(kvp => $"{kvp.Key} VARCHAR").ToList();
            var createTableSql = $"CREATE TABLE {tableName} ({string.Join(", ", columns)})";
            
            using var cmd = connection.CreateCommand();
            cmd.CommandText = createTableSql;
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task DeduplicateAndExportAsync(DuckDBConnection connection)
        {
            Directory.CreateDirectory(_options.OutputPath);

            foreach (var recordSpec in _recordCounts.Keys)
            {
                var stagingTable = $"{recordSpec}_staging";
                var dedupedTable = $"{recordSpec}_deduped";

                // PKカラムが定義されているかチェック
                if (_pkMapping.TryGetValue(recordSpec, out var pkColumns) && _options.Deduplicate)
                {
                    // 重複排除
                    var partitionBy = string.Join(", ", pkColumns);
                    var orderBy = "head_MakeDate_Year DESC, head_MakeDate_Month DESC, head_MakeDate_Day DESC";

                    var dedupSql = $@"
                        CREATE TABLE {dedupedTable} AS
                        SELECT * EXCLUDE (rn) FROM (
                            SELECT *,
                                ROW_NUMBER() OVER (
                                    PARTITION BY {partitionBy}
                                    ORDER BY {orderBy}
                                ) as rn
                            FROM {stagingTable}
                        ) WHERE rn = 1";

                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = dedupSql;
                    await cmd.ExecuteNonQueryAsync();

                }
                else
                {
                    // 重複排除しない場合はそのまま
                    dedupedTable = stagingTable;
                }

                // Parquetファイルとして出力
                var outputPath = Path.Combine(_options.OutputPath, recordSpec);
                Directory.CreateDirectory(outputPath);

                var exportSql = $@"
                    COPY (SELECT * FROM {dedupedTable})
                    TO '{outputPath}/'
                    (FORMAT PARQUET,
                     PARTITION_BY (head_MakeDate_Year, head_MakeDate_Month, head_MakeDate_Day),
                     OVERWRITE_OR_IGNORE true,
                     COMPRESSION 'SNAPPY')";

                using var cmd2 = connection.CreateCommand();
                cmd2.CommandText = exportSql;
                await cmd2.ExecuteNonQueryAsync();

                // 出力件数を取得
                using var countCmd = connection.CreateCommand();
                countCmd.CommandText = $"SELECT COUNT(*) FROM {dedupedTable}";
                var outputCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                
            }
        }

        private async Task CreateIndexTableAsync(DuckDBConnection connection)
        {
            // インデックステーブルの作成
            var createIndexSql = @"
                CREATE TABLE IF NOT EXISTS jv_partition_index (
                    record_spec VARCHAR,
                    id_Year VARCHAR,
                    id_MonthDay VARCHAR,
                    ChokyoDate VARCHAR,
                    BataijuDate VARCHAR,
                    id_JyoCD VARCHAR,
                    KettoNum VARCHAR,
                    head_MakeDate_Year VARCHAR,
                    head_MakeDate_Month VARCHAR,
                    head_MakeDate_Day VARCHAR,
                    partition_path VARCHAR,
                    record_count INTEGER,
                    last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = createIndexSql;
            await cmd.ExecuteNonQueryAsync();

            // 各レコード種別のインデックスを作成
            foreach (var recordSpec in _recordCounts.Keys)
            {
                var indexSql = recordSpec switch
                {
                    "RA" or "SE" or "HR" => $@"
                        INSERT INTO jv_partition_index
                        SELECT 
                            '{recordSpec}' as record_spec,
                            id_Year,
                            id_MonthDay,
                            NULL as ChokyoDate,
                            NULL as BataijuDate,
                            id_JyoCD,
                            NULL as KettoNum,
                            head_MakeDate_Year,
                            head_MakeDate_Month,
                            head_MakeDate_Day,
                            'year=' || head_MakeDate_Year || '/month=' || head_MakeDate_Month || '/day=' || head_MakeDate_Day as partition_path,
                            COUNT(*) as record_count,
                            CURRENT_TIMESTAMP
                        FROM {recordSpec}_deduped
                        GROUP BY id_Year, id_MonthDay, id_JyoCD, 
                                 head_MakeDate_Year, head_MakeDate_Month, head_MakeDate_Day",
                    
                    "UM" or "SK" => $@"
                        INSERT INTO jv_partition_index
                        SELECT 
                            '{recordSpec}' as record_spec,
                            NULL as id_Year,
                            NULL as id_MonthDay,
                            NULL as ChokyoDate,
                            NULL as BataijuDate,
                            NULL as id_JyoCD,
                            KettoNum,
                            head_MakeDate_Year,
                            head_MakeDate_Month,
                            head_MakeDate_Day,
                            'year=' || head_MakeDate_Year || '/month=' || head_MakeDate_Month || '/day=' || head_MakeDate_Day as partition_path,
                            COUNT(*) as record_count,
                            CURRENT_TIMESTAMP
                        FROM {recordSpec}_staging
                        GROUP BY KettoNum, head_MakeDate_Year, head_MakeDate_Month, head_MakeDate_Day",
                    
                    _ => null
                };

                if (indexSql != null)
                {
                    using var indexCmd = connection.CreateCommand();
                    indexCmd.CommandText = indexSql;
                    await indexCmd.ExecuteNonQueryAsync();
                }
            }

            // インデックステーブルをParquetとして出力
            var indexOutputPath = Path.Combine(_options.OutputPath, "_index");
            Directory.CreateDirectory(indexOutputPath);

            var exportIndexSql = $@"
                COPY (SELECT * FROM jv_partition_index)
                TO '{Path.Combine(indexOutputPath, "jv_partition_index.parquet")}'
                (FORMAT PARQUET, COMPRESSION 'SNAPPY')";

            using var exportCmd = connection.CreateCommand();
            exportCmd.CommandText = exportIndexSql;
            await exportCmd.ExecuteNonQueryAsync();
        }

        private void PrintSummary()
        {
            // サマリー表示は無視
        }

        private void ParseHNRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 繁殖馬マスタ (JV_HN_HANSYOKU)
            record["HansyokuNum"] = MidB2S(ref bytes, 12, 10);
            record["reserved"] = MidB2S(ref bytes, 22, 8);
            record["KettoNum"] = MidB2S(ref bytes, 30, 10);
            record["DelKubun"] = MidB2S(ref bytes, 40, 1);
            record["Bamei"] = MidB2S(ref bytes, 41, 36).Trim();
            record["BameiKana"] = MidB2S(ref bytes, 77, 40).Trim();
            record["BameiEng"] = MidB2S(ref bytes, 117, 80).Trim();
            record["BirthYear"] = MidB2S(ref bytes, 197, 4);
            record["SexCD"] = MidB2S(ref bytes, 201, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 202, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 203, 2);
            record["HansyokuMochiKubun"] = MidB2S(ref bytes, 205, 1);
            record["ImportYear"] = MidB2S(ref bytes, 206, 4);
            record["SanchiName"] = MidB2S(ref bytes, 210, 20).Trim();
            record["HansyokuFNum"] = MidB2S(ref bytes, 230, 10);
            record["HansyokuMNum"] = MidB2S(ref bytes, 240, 10);
        }

        private void ParseSKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 産駒マスタ (JV_SK_SANKU)
            record["KettoNum"] = MidB2S(ref bytes, 12, 10);
            record["BirthDate"] = MidB2S(ref bytes, 22, 8);
            record["SexCD"] = MidB2S(ref bytes, 30, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 31, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 32, 2);
            record["SankuMochiKubun"] = MidB2S(ref bytes, 34, 1);
            record["ImportYear"] = MidB2S(ref bytes, 35, 4);
            record["BreederCode"] = MidB2S(ref bytes, 39, 8);
            record["SanchiName"] = MidB2S(ref bytes, 47, 20).Trim();

            // 3代血統 繁殖登録番号（14個）
            for (int i = 0; i < 14; i++)
            {
                record[$"HansyokuNum_{i:00}"] = MidB2S(ref bytes, 67 + (10 * i), 10);
            }
        }

        private static string MidB2S(ref byte[] bytes, int start, int length)
        {
            return JVData_Struct.MidB2S(ref bytes, start, length);
        }

        private void ParseJGRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 除外馬（JG: JV_JG_JOGAIBA）
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            // 除外馬情報
            record["KettoNum"] = MidB2S(ref bytes, 27, 10);
            record["Bamei"] = MidB2S(ref bytes, 37, 36).Trim();
            record["ShutsubaTohyoJun"] = MidB2S(ref bytes, 73, 3);
            record["ShussoKubun"] = MidB2S(ref bytes, 76, 1);
            record["JogaiJotaiKubun"] = MidB2S(ref bytes, 77, 1);
        }

        private void ParseH1Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 票数（全掛） (H1: JV_H1_HYOSU_ZENKAKE)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 27, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 29, 2);
            
            // 発売フラグ
            for (int i = 0; i < 7; i++)
            {
                record[$"HatubaiFlag_{i}"] = MidB2S(ref bytes, 31 + i, 1);
            }
            
            record["FukuChakuBaraiKey"] = MidB2S(ref bytes, 38, 1);
            
            // 返還馬番情報(馬番01～28)
            for (int i = 0; i < 28; i++)
            {
                record[$"HenkanUma_{i:00}"] = MidB2S(ref bytes, 39 + i, 1);
            }
            
            // 返還枠番情報(枠番1～8)
            for (int i = 0; i < 8; i++)
            {
                record[$"HenkanWaku_{i}"] = MidB2S(ref bytes, 67 + i, 1);
            }
            
            // 返還同枠情報(枠番1～8)
            for (int i = 0; i < 8; i++)
            {
                record[$"HenkanDoWaku_{i}"] = MidB2S(ref bytes, 75 + i, 1);
            }
        }

        private void ParseH6Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 票数（3連単） (H6: JV_H6_HYOSU_SANRENTAN)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 27, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 29, 2);
            record["HatubaiFlag"] = MidB2S(ref bytes, 31, 1);
            
            // 返還馬番情報(馬番01～18)
            for (int i = 0; i < 18; i++)
            {
                record[$"HenkanUma_{i:00}"] = MidB2S(ref bytes, 32 + i, 1);
            }
        }

        private void ParseAVRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // AV統計情報 (AV: JV_AV_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["JyusyoKaisuu"] = MidB2S(ref bytes, 27, 4);
            record["AveTime"] = MidB2S(ref bytes, 31, 5);
            record["Ave3F"] = MidB2S(ref bytes, 36, 3);
        }

        private void ParseBNRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬主マスタ (BN: JV_BN_BANUSI)
            record["BanusiCode"] = MidB2S(ref bytes, 11, 6);
            record["BanusiName_Co"] = MidB2S(ref bytes, 17, 64).Trim();
            record["BanusiName_Kana"] = MidB2S(ref bytes, 81, 50).Trim();
            record["BanusiName"] = MidB2S(ref bytes, 131, 18).Trim();
            record["RuikeiHonsyoHeiti"] = MidB2S(ref bytes, 149, 10);
            record["RuikeiHonsyoSyogai"] = MidB2S(ref bytes, 159, 10);
            record["RuikeiFukaHeichi"] = MidB2S(ref bytes, 169, 10);
            record["RuikeiFukaSyogai"] = MidB2S(ref bytes, 179, 10);
        }

        private void ParseBRRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 生産者マスタ (BR: JV_BR_BREEDER)
            record["BreederCode"] = MidB2S(ref bytes, 11, 8);
            record["BreederName_Co"] = MidB2S(ref bytes, 19, 72).Trim();
            record["BreederName"] = MidB2S(ref bytes, 91, 18).Trim();
            record["Syutoku"] = MidB2S(ref bytes, 109, 20).Trim();
        }

        private void ParseBTRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 系統マスタ (BT: JV_BT_KEITO)
            record["HansyokuNum"] = MidB2S(ref bytes, 11, 10);
            record["KeitoId"] = MidB2S(ref bytes, 21, 30);
            record["KeitoName"] = MidB2S(ref bytes, 51, 36).Trim();
        }

        private void ParseCCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // コメント情報 (CC: JV_CC_INFO)
            // RACE_ID2の解析（一部フィールドが異なる）
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 15, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 17, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 19, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 21, 2);
            
            record["HappyoTime_Week"] = MidB2S(ref bytes, 23, 1);
            record["HappyoTime_Month"] = MidB2S(ref bytes, 24, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 26, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 28, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 30, 2);
        }

        private void ParseCKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 出走取消・競走除外 (CK: JV_CK_UMA)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["Umaban"] = MidB2S(ref bytes, 27, 2);
            record["KettoNum"] = MidB2S(ref bytes, 29, 10);
            record["Bamei"] = MidB2S(ref bytes, 39, 36).Trim();
            record["Jiyuu"] = MidB2S(ref bytes, 75, 1);
        }

        private void ParseCSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // コース情報 (CS: JV_CS_COURSE)
            record["id_JyoCD"] = MidB2S(ref bytes, 11, 2);
            record["TrackCD"] = MidB2S(ref bytes, 13, 2);
            record["CourseKubun"] = MidB2S(ref bytes, 15, 2);
            record["Kyori"] = MidB2S(ref bytes, 17, 5);
            record["Kaisuu"] = MidB2S(ref bytes, 22, 3);
            record["UpDown"] = MidB2S(ref bytes, 25, 39);
        }

        private void ParseDMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 騎手変更 (DM: JV_DM_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["Umaban"] = MidB2S(ref bytes, 35, 2);
            record["KettoNum"] = MidB2S(ref bytes, 37, 10);
            record["Bamei"] = MidB2S(ref bytes, 47, 36).Trim();
            record["KisyuCodeBefore"] = MidB2S(ref bytes, 83, 5);
            record["KisyuNameBefore"] = MidB2S(ref bytes, 88, 34).Trim();
            record["MinaraiCDBefore"] = MidB2S(ref bytes, 122, 1);
            record["Date"] = MidB2S(ref bytes, 123, 8);
            record["KisyuCodeAfter"] = MidB2S(ref bytes, 131, 5);
            record["KisyuNameAfter"] = MidB2S(ref bytes, 136, 34).Trim();
            record["MinaraiCDAfter"] = MidB2S(ref bytes, 170, 1);
            record["JiyuuCD"] = MidB2S(ref bytes, 171, 1);
        }

        private void ParseHCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 坂路調教 (HC: JV_HC_HANRO)
            record["TresenKubun"] = MidB2S(ref bytes, 11, 1);
            record["ChokyoDate_Year"] = MidB2S(ref bytes, 12, 4);
            record["ChokyoDate_Month"] = MidB2S(ref bytes, 16, 2);
            record["ChokyoDate_Day"] = MidB2S(ref bytes, 18, 2);
            record["ChokyoTime"] = MidB2S(ref bytes, 20, 4);
            record["KettoNum"] = MidB2S(ref bytes, 24, 10);
            record["HaronTime4"] = MidB2S(ref bytes, 34, 4);
            record["LapTime4"] = MidB2S(ref bytes, 38, 3);
            record["HaronTime3"] = MidB2S(ref bytes, 41, 4);
            record["LapTime3"] = MidB2S(ref bytes, 45, 3);
            record["HaronTime2"] = MidB2S(ref bytes, 48, 4);
            record["LapTime2"] = MidB2S(ref bytes, 52, 3);
            record["LapTime1"] = MidB2S(ref bytes, 55, 3);
        }

        private void ParseHRRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 払戻 (HR: JV_HR_PAY)
            // ParseHRRecordは既に実装済み（ParseRARecordと同じRACE_IDを持つ）
            // ここでは基本的な情報のみ追加
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 27, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 29, 2);
            
            // 発売フラグ
            for (int i = 0; i < 9; i++)
            {
                record[$"HatubaiFlag_{i}"] = MidB2S(ref bytes, 31 + i, 1);
            }
            
            // 返還馬番情報(馬番01～28)
            for (int i = 0; i < 28; i++)
            {
                record[$"HenkanUma_{i:00}"] = MidB2S(ref bytes, 40 + i, 1);
            }
            
            // 返還枠番情報(枠番1～8)
            for (int i = 0; i < 8; i++)
            {
                record[$"HenkanWaku_{i}"] = MidB2S(ref bytes, 68 + i, 1);
            }
            
            // 返還同枠情報(枠番1～8)
            for (int i = 0; i < 8; i++)
            {
                record[$"HenkanDoWaku_{i}"] = MidB2S(ref bytes, 76 + i, 1);
            }
        }

        private void ParseHSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬市場取引価格 (HS: JV_HS_SALE)
            record["KettoNum"] = MidB2S(ref bytes, 11, 10);
            record["HansyokuNum"] = MidB2S(ref bytes, 21, 10);
            record["Bamei"] = MidB2S(ref bytes, 31, 36).Trim();
            record["MarketCD"] = MidB2S(ref bytes, 67, 1);
            record["SaleDate_Year"] = MidB2S(ref bytes, 68, 4);
            record["SaleDate_Month"] = MidB2S(ref bytes, 72, 2);
            record["SaleDate_Day"] = MidB2S(ref bytes, 74, 2);
            record["SaleKingaku"] = MidB2S(ref bytes, 76, 10);
        }

        private void ParseHYRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬名意味由来情報 (HY: JV_HY_BAMEIORIGIN)
            record["KettoNum"] = MidB2S(ref bytes, 11, 10);
            record["Bamei"] = MidB2S(ref bytes, 21, 36).Trim();
            record["Origin"] = MidB2S(ref bytes, 57, 64).Trim();
        }

        private void ParseJCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // JC統計情報 (JC: JV_JC_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["Hondai"] = MidB2S(ref bytes, 27, 60).Trim();
            record["Ryakusyo10"] = MidB2S(ref bytes, 87, 20).Trim();
            record["Ryakusyo6"] = MidB2S(ref bytes, 107, 12).Trim();
            record["Ryakusyo3"] = MidB2S(ref bytes, 119, 6).Trim();
            record["GradeCD"] = MidB2S(ref bytes, 125, 1);
            record["GradeCDBefore"] = MidB2S(ref bytes, 126, 1);
            record["SyubetuCD"] = MidB2S(ref bytes, 127, 2);
            record["JyokenName"] = MidB2S(ref bytes, 129, 60).Trim();
            record["Kyori"] = MidB2S(ref bytes, 189, 4);
            record["TrackCD"] = MidB2S(ref bytes, 193, 2);
        }

        private void ParseO1Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（単複枠） (O1: JV_O1_ODDS_TANFUKUWAKU)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["TansyoFlag"] = MidB2S(ref bytes, 39, 1);
            record["FukusyoFlag"] = MidB2S(ref bytes, 40, 1);
            record["WakurenFlag"] = MidB2S(ref bytes, 41, 1);
            record["FukuChakuBaraiKey"] = MidB2S(ref bytes, 42, 1);
        }

        private void ParseO2Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（馬連） (O2: JV_O2_ODDS_UMAREN)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["UmarenFlag"] = MidB2S(ref bytes, 39, 1);
        }

        private void ParseO3Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（ワイド） (O3: JV_O3_ODDS_WIDE)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["WideFlag"] = MidB2S(ref bytes, 39, 1);
        }

        private void ParseO4Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（馬単） (O4: JV_O4_ODDS_UMATAN)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["UmatanFlag"] = MidB2S(ref bytes, 39, 1);
        }

        private void ParseO5Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（3連複） (O5: JV_O5_ODDS_SANREN)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["SanrenpukuFlag"] = MidB2S(ref bytes, 39, 1);
        }

        private void ParseO6Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // オッズ（3連単） (O6: JV_O6_ODDS_SANRENTAN)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Month"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Day"] = MidB2S(ref bytes, 29, 2);
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 31, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 33, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 35, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 37, 2);
            record["SanrentanFlag"] = MidB2S(ref bytes, 39, 1);
        }

        private void ParseRCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // レースレコード (RC: JV_RC_RECORD)
            record["RecordSpec"] = MidB2S(ref bytes, 0, 2);
            record["DataKubun"] = MidB2S(ref bytes, 2, 1);
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            record["TokuNum"] = MidB2S(ref bytes, 27, 4);
            record["Hondai"] = MidB2S(ref bytes, 31, 60).Trim();
            record["GradeCD"] = MidB2S(ref bytes, 91, 1);
            record["SyubetuCD"] = MidB2S(ref bytes, 92, 2);
            record["Kyori"] = MidB2S(ref bytes, 94, 4);
            record["TrackCD"] = MidB2S(ref bytes, 98, 2);
            
            record["RecKubun"] = MidB2S(ref bytes, 100, 1);
            record["RecTime"] = MidB2S(ref bytes, 101, 4);
            record["TenkoBaba_TenkoCD"] = MidB2S(ref bytes, 105, 1);
            record["TenkoBaba_SibaBabaCD"] = MidB2S(ref bytes, 106, 1);
            record["TenkoBaba_DirtBabaCD"] = MidB2S(ref bytes, 107, 1);
            
            record["rec1_RecKubun"] = MidB2S(ref bytes, 108, 1);
            record["rec1_RecTime"] = MidB2S(ref bytes, 109, 4);
            record["rec1_KettoNum"] = MidB2S(ref bytes, 113, 10);
            record["rec1_Bamei"] = MidB2S(ref bytes, 123, 36).Trim();
            record["rec1_UmaKigoCD"] = MidB2S(ref bytes, 159, 2);
            record["rec1_SexCD"] = MidB2S(ref bytes, 161, 1);
            record["rec1_BreedCD"] = MidB2S(ref bytes, 162, 2);
            record["rec1_KeiroCD"] = MidB2S(ref bytes, 164, 2);
            record["rec1_Barei"] = MidB2S(ref bytes, 166, 2);
            record["rec1_ChokyosiCode"] = MidB2S(ref bytes, 168, 5);
            record["rec1_ChokyosiName"] = MidB2S(ref bytes, 173, 34).Trim();
            record["rec1_Futan"] = MidB2S(ref bytes, 207, 3);
            record["rec1_KisyuCode"] = MidB2S(ref bytes, 210, 5);
            record["rec1_KisyuName"] = MidB2S(ref bytes, 215, 34).Trim();
        }

        private void ParseTCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 時計別成績 (TC: JV_TC_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["TrackCD"] = MidB2S(ref bytes, 27, 2);
            record["Kyori"] = MidB2S(ref bytes, 29, 4);
            record["TenkoBaba_TenkoCD"] = MidB2S(ref bytes, 33, 1);
            record["TenkoBaba_SibaBabaCD"] = MidB2S(ref bytes, 34, 1);
            record["TenkoBaba_DirtBabaCD"] = MidB2S(ref bytes, 35, 1);
            
            // ラップタイム
            for (int i = 0; i < 25; i++)
            {
                record[$"LapTime_{i:00}"] = MidB2S(ref bytes, 36 + (i * 3), 3);
            }
            
            record["Haron4Time"] = MidB2S(ref bytes, 111, 4);
            record["Haron3Time"] = MidB2S(ref bytes, 115, 4);
        }

        private void ParseTKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 特別登録馬 (TK: JV_TK_TOKUUMA)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            // レース情報
            record["RaceInfo_YoubiCD"] = MidB2S(ref bytes, 27, 1);
            record["RaceInfo_TokuNum"] = MidB2S(ref bytes, 28, 4);
            record["RaceInfo_Hondai"] = MidB2S(ref bytes, 32, 60).Trim();
            record["RaceInfo_Fukudai"] = MidB2S(ref bytes, 92, 60).Trim();
            record["RaceInfo_Kakko"] = MidB2S(ref bytes, 152, 60).Trim();
            record["RaceInfo_HondaiEng"] = MidB2S(ref bytes, 212, 120).Trim();
            record["RaceInfo_FukudaiEng"] = MidB2S(ref bytes, 332, 120).Trim();
            record["RaceInfo_KakkoEng"] = MidB2S(ref bytes, 452, 120).Trim();
            record["RaceInfo_Ryakusyo10"] = MidB2S(ref bytes, 572, 20).Trim();
            record["RaceInfo_Ryakusyo6"] = MidB2S(ref bytes, 592, 12).Trim();
            record["RaceInfo_Ryakusyo3"] = MidB2S(ref bytes, 604, 6).Trim();
            record["RaceInfo_Kubun"] = MidB2S(ref bytes, 610, 1);
            record["RaceInfo_Nkai"] = MidB2S(ref bytes, 611, 3).Trim();
            
            record["GradeCD"] = MidB2S(ref bytes, 614, 1);
            
            // 条件情報
            record["JyokenInfo_SyubetuCD"] = MidB2S(ref bytes, 615, 2);
            record["JyokenInfo_KigoCD"] = MidB2S(ref bytes, 617, 3);
            record["JyokenInfo_JyuryoCD"] = MidB2S(ref bytes, 620, 1);
            record["JyokenInfo_JyokenCD1"] = MidB2S(ref bytes, 621, 3);
            record["JyokenInfo_JyokenCD2"] = MidB2S(ref bytes, 624, 3);
            record["JyokenInfo_JyokenCD3"] = MidB2S(ref bytes, 627, 3);
            record["JyokenInfo_JyokenCD4"] = MidB2S(ref bytes, 630, 3);
            record["JyokenInfo_JyokenCD5"] = MidB2S(ref bytes, 633, 3);
            
            record["Kyori"] = MidB2S(ref bytes, 636, 4);
            record["TrackCD"] = MidB2S(ref bytes, 640, 2);
            record["CourseKubunCD"] = MidB2S(ref bytes, 642, 2);
            
            record["HandiDate_Year"] = MidB2S(ref bytes, 644, 4);
            record["HandiDate_Month"] = MidB2S(ref bytes, 648, 2);
            record["HandiDate_Day"] = MidB2S(ref bytes, 650, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 652, 3);
        }

        private void ParseTMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // タイム型データマイニング予想 (TM: JV_TM_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            // 出走馬情報 (18頭分)
            for (int i = 0; i < 18; i++)
            {
                int offset = 27 + (i * 13);
                record[$"DeM_{i:00}_Umaban"] = MidB2S(ref bytes, offset, 2);
                record[$"DeM_{i:00}_KettoNum"] = MidB2S(ref bytes, offset + 2, 10);
                record[$"DeM_{i:00}_TMScore"] = MidB2S(ref bytes, offset + 12, 1);
            }
        }

        private void ParseWCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // ウッドチップ調教 (WC: JV_WC_WOOD)
            record["TresenKubun"] = MidB2S(ref bytes, 11, 1);
            
            record["ChokyoDate_Year"] = MidB2S(ref bytes, 12, 4);
            record["ChokyoDate_Month"] = MidB2S(ref bytes, 16, 2);
            record["ChokyoDate_Day"] = MidB2S(ref bytes, 18, 2);
            
            record["ChokyoTime"] = MidB2S(ref bytes, 20, 4);
            record["KettoNum"] = MidB2S(ref bytes, 24, 10);
            record["Course"] = MidB2S(ref bytes, 34, 1);
            record["BabaAround"] = MidB2S(ref bytes, 35, 1);
            record["reserved"] = MidB2S(ref bytes, 36, 1);
            
            record["HaronTime10"] = MidB2S(ref bytes, 37, 4);
            record["LapTime10"] = MidB2S(ref bytes, 41, 3);
            record["HaronTime9"] = MidB2S(ref bytes, 44, 4);
            record["LapTime9"] = MidB2S(ref bytes, 48, 3);
            record["HaronTime8"] = MidB2S(ref bytes, 51, 4);
            record["LapTime8"] = MidB2S(ref bytes, 55, 3);
            record["HaronTime7"] = MidB2S(ref bytes, 58, 4);
            record["LapTime7"] = MidB2S(ref bytes, 62, 3);
            record["HaronTime6"] = MidB2S(ref bytes, 65, 4);
            record["LapTime6"] = MidB2S(ref bytes, 69, 3);
            record["HaronTime5"] = MidB2S(ref bytes, 72, 4);
            record["LapTime5"] = MidB2S(ref bytes, 76, 3);
            record["HaronTime4"] = MidB2S(ref bytes, 79, 4);
            record["LapTime4"] = MidB2S(ref bytes, 83, 3);
            record["HaronTime3"] = MidB2S(ref bytes, 86, 4);
            record["LapTime3"] = MidB2S(ref bytes, 90, 3);
            record["HaronTime2"] = MidB2S(ref bytes, 93, 4);
            record["LapTime2"] = MidB2S(ref bytes, 97, 3);
            record["LapTime1"] = MidB2S(ref bytes, 100, 3);
        }

        private void ParseWERecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 天候・馬場状態変化 (WE: JV_WE_WEATHER)
            // RACE_ID2の解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 15, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 17, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 19, 2);
            
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 21, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 23, 2);
            
            record["HenkoID"] = MidB2S(ref bytes, 25, 1);
            record["TenkoCD"] = MidB2S(ref bytes, 26, 1);
            record["SibaBabaCD"] = MidB2S(ref bytes, 27, 1);
            record["DirtBabaCD"] = MidB2S(ref bytes, 28, 1);
        }

        private void ParseWFRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // Win5 (WF: JV_WF_INFO)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            
            record["KaisaiFlag"] = MidB2S(ref bytes, 21, 1);
            
            // 5レース分の対象レース情報
            for (int i = 0; i < 5; i++)
            {
                int offset = 22 + (i * 2);
                record[$"JyoCD_{i}"] = MidB2S(ref bytes, offset, 2);
            }
            
            for (int i = 0; i < 5; i++)
            {
                int offset = 32 + (i * 2);
                record[$"RaceNum_{i}"] = MidB2S(ref bytes, offset, 2);
            }
            
            record["SyussoTosu"] = MidB2S(ref bytes, 42, 18);
            record["HatubaiFlag"] = MidB2S(ref bytes, 60, 1);
            record["Odds"] = MidB2S(ref bytes, 61, 7);
            record["Ninki"] = MidB2S(ref bytes, 68, 3);
            
            // 5レース分の馬番情報
            for (int i = 0; i < 5; i++)
            {
                int offset = 71 + (i * 2);
                record[$"Umaban_{i}"] = MidB2S(ref bytes, offset, 2);
            }
            
            record["Pay"] = MidB2S(ref bytes, 81, 9);
            record["Tekichunashi"] = MidB2S(ref bytes, 90, 3);
            record["Carry"] = MidB2S(ref bytes, 93, 15);
        }

        private void ParseWHRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬体重 (WH: JV_WH_BATAIJYU)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["HappyoTime_Hour"] = MidB2S(ref bytes, 27, 2);
            record["HappyoTime_Minute"] = MidB2S(ref bytes, 29, 2);
            
            // 28頭分の馬体重情報
            for (int i = 0; i < 28; i++)
            {
                int offset = 31 + (i * 8);
                record[$"BaTaijyu_{i:00}_Umaban"] = MidB2S(ref bytes, offset, 2);
                record[$"BaTaijyu_{i:00}_Bamei"] = MidB2S(ref bytes, offset + 2, 36).Trim();
                record[$"BaTaijyu_{i:00}_BaTaijyu"] = MidB2S(ref bytes, offset + 38, 3);
                record[$"BaTaijyu_{i:00}_ZogenFugo"] = MidB2S(ref bytes, offset + 41, 1);
                record[$"BaTaijyu_{i:00}_ZogenSa"] = MidB2S(ref bytes, offset + 42, 3);
            }
        }

        private void ParseYSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 予想スケジュール (YS: JV_YS_SCHEDULE)
            // RACE_ID2の解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 15, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 17, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 19, 2);
            
            // 12レース分の情報
            for (int i = 0; i < 12; i++)
            {
                int offset = 21 + (i * 146);
                record[$"ScheduleInfo_{i:00}_RaceNum"] = MidB2S(ref bytes, offset, 2);
                record[$"ScheduleInfo_{i:00}_YoubiCD"] = MidB2S(ref bytes, offset + 2, 1);
                record[$"ScheduleInfo_{i:00}_TokuNum"] = MidB2S(ref bytes, offset + 3, 4);
                record[$"ScheduleInfo_{i:00}_Hondai"] = MidB2S(ref bytes, offset + 7, 60).Trim();
                record[$"ScheduleInfo_{i:00}_Ryakusyo10"] = MidB2S(ref bytes, offset + 67, 20).Trim();
                record[$"ScheduleInfo_{i:00}_Ryakusyo6"] = MidB2S(ref bytes, offset + 87, 12).Trim();
                record[$"ScheduleInfo_{i:00}_Ryakusyo3"] = MidB2S(ref bytes, offset + 99, 6).Trim();
                record[$"ScheduleInfo_{i:00}_Kubun"] = MidB2S(ref bytes, offset + 105, 1);
                record[$"ScheduleInfo_{i:00}_Nkai"] = MidB2S(ref bytes, offset + 106, 3).Trim();
                record[$"ScheduleInfo_{i:00}_GradeCD"] = MidB2S(ref bytes, offset + 109, 1);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_SyubetuCD"] = MidB2S(ref bytes, offset + 110, 2);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_KigoCD"] = MidB2S(ref bytes, offset + 112, 3);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyuryoCD"] = MidB2S(ref bytes, offset + 115, 1);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyokenCD1"] = MidB2S(ref bytes, offset + 116, 3);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyokenCD2"] = MidB2S(ref bytes, offset + 119, 3);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyokenCD3"] = MidB2S(ref bytes, offset + 122, 3);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyokenCD4"] = MidB2S(ref bytes, offset + 125, 3);
                record[$"ScheduleInfo_{i:00}_JyokenInfo_JyokenCD5"] = MidB2S(ref bytes, offset + 128, 3);
                record[$"ScheduleInfo_{i:00}_Kyori"] = MidB2S(ref bytes, offset + 131, 4);
                record[$"ScheduleInfo_{i:00}_TrackCD"] = MidB2S(ref bytes, offset + 135, 2);
                record[$"ScheduleInfo_{i:00}_TorokuTosu"] = MidB2S(ref bytes, offset + 137, 3);
                record[$"ScheduleInfo_{i:00}_HassoTime"] = MidB2S(ref bytes, offset + 140, 4);
                record[$"ScheduleInfo_{i:00}_TorokuFlag"] = MidB2S(ref bytes, offset + 144, 1);
                record[$"ScheduleInfo_{i:00}_YosouFlag"] = MidB2S(ref bytes, offset + 145, 1);
            }
        }
    }
}