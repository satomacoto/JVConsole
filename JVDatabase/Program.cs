using CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using JVDatabase;

// Shift_JISを使用可能にする
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// コンソール出力をUTF-8に設定
Console.OutputEncoding = Encoding.UTF8;

// コマンドライン引数の解析と実行
return await Parser.Default.ParseArguments<ConfigOptions, InitOptions, UpdateOptions, RealtimeOptions, AnalyzeOptions, FetchOddsOptions>(args)
    .MapResult(
        async (ConfigOptions opts) => await RunConfigAsync(opts),
        async (InitOptions opts) => await RunInitAsync(opts),
        async (UpdateOptions opts) => await RunUpdateAsync(opts),
        async (RealtimeOptions opts) => await RunRealtimeAsync(opts),
        async (AnalyzeOptions opts) => await RunAnalyzeAsync(opts),
        async (FetchOddsOptions opts) => await RunFetchOddsAsync(opts),
        async errs => await Task.FromResult(1));

// config コマンドの実装
static async Task<int> RunConfigAsync(ConfigOptions opts)
{
    try
    {
        var configPath = GetConfigPath();
        var config = await LoadOrCreateConfigAsync(configPath);
        
        if (opts.List)
        {
            // 現在の設定を表示
            Console.WriteLine("現在の設定:");
            Console.WriteLine($"  デフォルトパス: {config.DefaultDatabasePath}");
            Console.WriteLine($"  デフォルトデータ種別: {string.Join(",", config.DefaultDataSpecs)}");
            Console.WriteLine($"  スキップレコード: {string.Join(",", config.SkipRecordSpecs)}");
            Console.WriteLine($"  自動バックアップ: {(config.AutoBackup ? "有効" : "無効")}");
            Console.WriteLine($"  設定ファイル: {configPath}");
            return 0;
        }
        
        // 対話的に設定を変更
        Console.WriteLine("JVDatabase設定を変更します（Enterでスキップ）");
        Console.WriteLine();
        
        Console.WriteLine($"デフォルトデータベースパス [{config.DefaultDatabasePath}]:");
        var newPath = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            config.DefaultDatabasePath = newPath;
        }
        
        Console.WriteLine($"デフォルトデータ種別 [{string.Join(",", config.DefaultDataSpecs)}]:");
        var newDataSpecs = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDataSpecs))
        {
            config.DefaultDataSpecs = newDataSpecs.Split(',').Select(s => s.Trim()).ToList();
        }
        
        Console.WriteLine($"スキップするレコード種別 [{string.Join(",", config.SkipRecordSpecs)}]:");
        var newSkipSpecs = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newSkipSpecs))
        {
            config.SkipRecordSpecs = newSkipSpecs.Split(',').Select(s => s.Trim()).ToList();
        }
        
        Console.WriteLine($"自動バックアップ (y/n) [{(config.AutoBackup ? "y" : "n")}]:");
        var autoBackup = Console.ReadLine()?.ToLower();
        if (autoBackup == "y" || autoBackup == "yes")
        {
            config.AutoBackup = true;
        }
        else if (autoBackup == "n" || autoBackup == "no")
        {
            config.AutoBackup = false;
        }
        
        // 設定を保存
        await SaveConfigAsync(configPath, config);
        Console.WriteLine("\n設定を保存しました。");
        
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

// init コマンドの実装
static async Task<int> RunInitAsync(InitOptions opts)
{
    try
    {
        // 設定ファイルを読み込み
        var config = await LoadOrCreateConfigAsync(GetConfigPath());
        
        // 対話モードの場合
        if (opts.Interactive)
        {
            Console.WriteLine("JVDatabase初期化設定");
            Console.WriteLine("===================");
            
            // データベースパス
            if (string.IsNullOrEmpty(opts.DatabasePath) || opts.DatabasePath == "./jvdb")
            {
                Console.WriteLine($"\nデータベースの保存場所 [{config.DefaultDatabasePath}]:");
                Console.WriteLine("（例: C:\\JVData, D:\\Racing\\Database）");
                var inputPath = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputPath))
                {
                    opts.DatabasePath = inputPath;
                }
                else
                {
                    opts.DatabasePath = config.DefaultDatabasePath;
                }
            }
            
            // 開始日
            if (string.IsNullOrEmpty(opts.StartDate))
            {
                Console.WriteLine($"\nデータ取得開始日 (YYYYMMDD) [{DateTime.Now.AddYears(-1):yyyyMMdd}]:");
                var inputStart = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputStart))
                {
                    opts.StartDate = inputStart;
                }
                else
                {
                    opts.StartDate = DateTime.Now.AddYears(-1).ToString("yyyyMMdd");
                }
            }
            
            // 終了日
            if (string.IsNullOrEmpty(opts.EndDate))
            {
                Console.WriteLine($"\nデータ取得終了日 (YYYYMMDD) [{DateTime.Now:yyyyMMdd}]:");
                var inputEnd = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputEnd))
                {
                    opts.EndDate = inputEnd;
                }
                else
                {
                    opts.EndDate = DateTime.Now.ToString("yyyyMMdd");
                }
            }
            
            // データ種別
            if (!opts.DataSpecs.Any() || opts.DataSpecs.SequenceEqual(new[] { "TOKU", "RACE", "DIFN", "BLDN", "SNPN", "SLOP", "WOOD", "YSCH", "HOSN", "HOYU" }))
            {
                Console.WriteLine($"\n取得するデータ種別（カンマ区切り）:");
                Console.WriteLine($"[{string.Join(",", config.DefaultDataSpecs)}]");
                Console.WriteLine("  TOKU: 特別登録馬");
                Console.WriteLine("  RACE: レース情報（必須）");
                Console.WriteLine("  SLOP: 坂路調教");
                Console.WriteLine("  WOOD: ウッドチップ調教");
                Console.WriteLine("  YSCH: 予想（騎手、厩舎、馬、総合）");
                Console.WriteLine("  HOYU: 馬主");
                Console.WriteLine("  MING: マイニング予想");
                Console.WriteLine("  SNPN: SNAP");
                Console.WriteLine("  HOSN: 競走馬市場取引価格");
                Console.WriteLine("  DIFN: マスター系");
                Console.WriteLine("  その他: BLDN（血統）など");
                var inputSpecs = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputSpecs))
                {
                    opts.DataSpecs = inputSpecs.Split(',').Select(s => s.Trim());
                }
                else
                {
                    opts.DataSpecs = config.DefaultDataSpecs;
                }
            }
            
            // 確認
            Console.WriteLine("\n設定内容の確認:");
            Console.WriteLine($"  保存場所: {opts.DatabasePath}");
            Console.WriteLine($"  期間: {opts.StartDate} ～ {opts.EndDate}");
            Console.WriteLine($"  データ種別: {string.Join(", ", opts.DataSpecs)}");
            Console.WriteLine($"  推定サイズ: 約{EstimateDataSize(opts.StartDate, opts.EndDate, opts.DataSpecs)}GB");
            Console.WriteLine("\nこの設定で初期化を開始しますか？ (y/N):");
            
            var confirm = Console.ReadLine()?.ToLower();
            if (confirm != "y" && confirm != "yes")
            {
                Console.WriteLine("初期化をキャンセルしました。");
                return 0;
            }
        }
        
        Console.WriteLine("\nJVDatabase初期化を開始します...");
        
        // 1. メタデータディレクトリの作成
        var metadataPath = Path.Combine(opts.DatabasePath, ".jvdb");
        Directory.CreateDirectory(metadataPath);
        
        // 2. 初期データのダウンロード
        Console.WriteLine($"\n[1/3] {opts.StartDate}から{opts.EndDate}までのデータをダウンロード中...");
        
        // 1年以上前のデータが必要かチェック
        var oneYearAgo = DateTime.Now.AddYears(-1);
        var startDate = DateTime.ParseExact(opts.StartDate, "yyyyMMdd", null);
        var endDate = DateTime.ParseExact(opts.EndDate, "yyyyMMdd", null);
        var useSetupOption = startDate < oneYearAgo;
        
        if (useSetupOption)
        {
            Console.WriteLine("※ 1年以上前のデータを含むため、セットアップモードでダウンロードします。");
            Console.WriteLine("※ 初回ダウンロードには時間がかかる場合があります。");
        }
        
        var downloadArgs = new[]
        {
            "jv",
            "--dataspec", string.Join(",", opts.DataSpecs),
            "--fromdate", $"{opts.StartDate}000000-{opts.EndDate}000000",
            "--option", useSetupOption ? "4" : "1", // 期間指定の場合はoption 3を使用
            "--outputDir", Path.Combine(opts.DatabasePath, "raw")
        };
        
        Console.WriteLine("※ JVDownloaderが自動的に期間を分割して処理します。");
        Console.WriteLine("※ ダウンロード中は進捗が表示されます。しばらくお待ちください...");
        
        var downloadResult = await RunJVDownloaderAsync(string.Join(" ", downloadArgs), Path.Combine(opts.DatabasePath, "raw"));
        if (downloadResult != 0)
        {
            Console.Error.WriteLine($"ダウンロードに失敗しました。");
            return downloadResult;
        }
        
        // 4. ダウンロードしたファイルの一覧取得
        var rawDir = Path.Combine(opts.DatabasePath, "raw");
        var downloadedFiles = Directory.GetFiles(rawDir, "JV-*.txt")
            .OrderBy(f => f)
            .ToList();
        
        Console.WriteLine($"\n[2/3] {downloadedFiles.Count}個のファイルをParquet形式に変換中...");
        
        // 5. Parquet変換
        var parquetDir = Path.Combine(opts.DatabasePath, "parquet");
        Directory.CreateDirectory(parquetDir);
        
        int convertedCount = 0;
        var metadata = new DatabaseMetadata
        {
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            StartDate = opts.StartDate,
            EndDate = opts.EndDate,
            DataSpecs = opts.DataSpecs.ToList(),
            DatabasePath = opts.DatabasePath,
            Version = "1.0.0"
        };
        
        foreach (var file in downloadedFiles)
        {
            var fileName = Path.GetFileName(file);
            Console.WriteLine($"\n変換中: {convertedCount + 1}/{downloadedFiles.Count} - {fileName}");
            
            var startTime = DateTime.Now;
            var result = await RunJVParquetAsync(file, parquetDir, opts.SkipRecordSpecs.ToArray());
            var elapsed = DateTime.Now - startTime;
            
            if (result == 0)
            {
                convertedCount++;
                Console.WriteLine($"  → 完了 ({elapsed.TotalSeconds:F1}秒)");
                
                // ファイル情報を抽出してメタデータに記録
                var fileInfo = await ExtractJVFileInfoAsync(file);
                if (fileInfo.HasValue)
                {
                    var (dataspec, lastfiletimestamp) = fileInfo.Value;
                    if (!metadata.DataSpecStatuses.ContainsKey(dataspec))
                    {
                        metadata.DataSpecStatuses[dataspec] = new DataSpecStatus();
                    }
                    metadata.DataSpecStatuses[dataspec].LastFileTimestamp = lastfiletimestamp;
                    metadata.DataSpecStatuses[dataspec].LastFileName = fileName;
                    metadata.DataSpecStatuses[dataspec].LastUpdatedAt = DateTime.UtcNow;
                }
                
                // 処理済みファイルリストに追加
                if (!metadata.ProcessedFiles.Contains(fileName))
                {
                    metadata.ProcessedFiles.Add(fileName);
                }
            }
            else
            {
                Console.WriteLine($"  → 失敗 (エラーコード: {result})");
            }
        }
        Console.WriteLine($"\n変換完了: {convertedCount}/{downloadedFiles.Count}");
        
        // 6. メタデータの保存
        Console.WriteLine("\n[3/3] メタデータを保存中...");
        
        var metadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(metadataPath, "metadata.json"), metadataJson);
        
        Console.WriteLine($"\nJVDatabase初期化が完了しました。");
        Console.WriteLine($"データベースパス: {opts.DatabasePath}");
        Console.WriteLine($"Parquetファイル: {parquetDir}");
        
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

// update コマンドの実装
static async Task<int> RunUpdateAsync(UpdateOptions opts)
{
    try
    {
        // メタデータの読み込み
        var metadataPath = Path.Combine(opts.DatabasePath, ".jvdb", "metadata.json");
        if (!File.Exists(metadataPath))
        {
            Console.Error.WriteLine("データベースが初期化されていません。先に 'init' コマンドを実行してください。");
            return 1;
        }
        
        var metadataJson = await File.ReadAllTextAsync(metadataPath);
        var metadata = JsonSerializer.Deserialize<DatabaseMetadata>(metadataJson);
        
        if (metadata == null)
        {
            Console.Error.WriteLine("メタデータの読み込みに失敗しました。");
            return 1;
        }
        
        Console.WriteLine($"最終更新日時: {metadata.LastUpdated:yyyy-MM-dd HH:mm:ss} UTC");
        
        // 更新期間の決定
        var toDate = opts.EndDate ?? DateTime.Now.ToString("yyyyMMdd");
        var rawDir = Path.Combine(opts.DatabasePath, "raw");
        
        // データ種別ごとに差分更新を実行
        Console.WriteLine($"\n[1/3] 差分データをダウンロード中...");
        var downloadedFiles = new List<string>();
        
        foreach (var dataspec in metadata.DataSpecs)
        {
            Console.WriteLine($"\n  {dataspec}のデータを確認中...");
            
            // このデータ種別の最終タイムスタンプを取得
            string fromDate = metadata.EndDate + "000000"; // デフォルト値
            if (metadata.DataSpecStatuses.TryGetValue(dataspec, out var status) && !string.IsNullOrEmpty(status.LastFileTimestamp))
            {
                fromDate = status.LastFileTimestamp;
                Console.WriteLine($"    最終タイムスタンプ: {fromDate}");
            }
            
            var downloadArgs = new[]
            {
                "jv",
                "--dataspec", dataspec,
                "--fromdate", $"{fromDate}",
                "--option", "1", // 通常データダウンロード
                "--outputDir", rawDir
            };
            
            var downloadResult = await RunJVDownloaderAsync(string.Join(" ", downloadArgs), rawDir);
            if (downloadResult != 0)
            {
                Console.WriteLine($"    {dataspec}のダウンロードに失敗しました。");
                continue;
            }
            
            // ダウンロードしたファイルから最新のものを特定
            var pattern = $"JV-{dataspec}-*.txt";
            var specFiles = Directory.GetFiles(rawDir, pattern)
                .Where(f => !metadata.ProcessedFiles.Contains(Path.GetFileName(f)))
                .OrderBy(f => f)
                .ToList();
            
            downloadedFiles.AddRange(specFiles);
            Console.WriteLine($"    新規ファイル: {specFiles.Count}個");
        }
        
        if (downloadedFiles.Count == 0)
        {
            Console.WriteLine("\n更新するデータがありません。");
            return 0;
        }
        
        Console.WriteLine($"\n[2/3] {downloadedFiles.Count}個の新しいファイルをParquet形式に変換中...");
        
        // Parquet変換
        var parquetDir = Path.Combine(opts.DatabasePath, "parquet");
        int convertedCount = 0;
        
        foreach (var file in downloadedFiles)
        {
            var fileName = Path.GetFileName(file);
            Console.WriteLine($"\n変換中: {convertedCount + 1}/{downloadedFiles.Count} - {fileName}");
            
            var skipSpecs = metadata.DataSpecs.Contains("H6") || metadata.DataSpecs.Contains("O6") 
                ? new[] { "H6", "O6" } 
                : Array.Empty<string>();
            
            var startTime = DateTime.Now;
            var result = await RunJVParquetAsync(file, parquetDir, skipSpecs);
            var elapsed = DateTime.Now - startTime;
            
            if (result == 0)
            {
                convertedCount++;
                Console.WriteLine($"  → 完了 ({elapsed.TotalSeconds:F1}秒)");
                
                // ファイル情報を抽出してメタデータを更新
                var fileInfo = await ExtractJVFileInfoAsync(file);
                if (fileInfo.HasValue)
                {
                    var (dataspec, lastfiletimestamp) = fileInfo.Value;
                    if (!metadata.DataSpecStatuses.ContainsKey(dataspec))
                    {
                        metadata.DataSpecStatuses[dataspec] = new DataSpecStatus();
                    }
                    metadata.DataSpecStatuses[dataspec].LastFileTimestamp = lastfiletimestamp;
                    metadata.DataSpecStatuses[dataspec].LastFileName = fileName;
                    metadata.DataSpecStatuses[dataspec].LastUpdatedAt = DateTime.UtcNow;
                }
                
                // 処理済みファイルリストに追加
                if (!metadata.ProcessedFiles.Contains(fileName))
                {
                    metadata.ProcessedFiles.Add(fileName);
                }
            }
            else
            {
                Console.WriteLine($"  → 失敗 (エラーコード: {result})");
            }
        }
        Console.WriteLine($"\n変換完了: {convertedCount}/{downloadedFiles.Count}");
        
        // メタデータの更新
        Console.WriteLine("\n[3/3] メタデータを更新中...");
        metadata.LastUpdated = DateTime.UtcNow;
        metadata.EndDate = toDate;
        
        // 古い処理済みファイルリストを整理（最新1000件のみ保持）
        if (metadata.ProcessedFiles.Count > 1000)
        {
            metadata.ProcessedFiles = metadata.ProcessedFiles.Skip(metadata.ProcessedFiles.Count - 1000).ToList();
        }
        
        var updatedMetadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(metadataPath, updatedMetadataJson);
        
        Console.WriteLine($"\nデータベース更新が完了しました。");
        Console.WriteLine($"更新ファイル数: {convertedCount}");
        
        // データ種別ごとの最終タイムスタンプを表示
        Console.WriteLine("\n各データ種別の最終更新状況:");
        foreach (var kvp in metadata.DataSpecStatuses)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value.LastFileTimestamp} ({kvp.Value.LastFileName})");
        }
        
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

// realtime コマンドの実装
static async Task<int> RunRealtimeAsync(RealtimeOptions opts)
{
    try
    {
        Console.WriteLine($"リアルタイムデータ取得を開始します...");
        Console.WriteLine($"対象日: {opts.TargetDate}");
        Console.WriteLine($"データ種別: {string.Join(", ", opts.DataSpecs)}");
        
        // リアルタイム用ディレクトリの作成
        var realtimeDir = Path.Combine(opts.DatabasePath, "realtime");
        var rawDir = Path.Combine(realtimeDir, "raw");
        var parquetDir = Path.Combine(realtimeDir, "parquet");
        Directory.CreateDirectory(rawDir);
        Directory.CreateDirectory(parquetDir);
        
        // キーの生成（開催日単位）
        var keys = new List<string> { opts.TargetDate };
        
        foreach (var dataspec in opts.DataSpecs)
        {
            Console.WriteLine($"\n[{dataspec}] データを取得中...");
            
            var downloadArgs = new[]
            {
                "jvrt",
                "--dataspec", dataspec,
                "--key", string.Join(",", keys),
                "--outputDir", rawDir
            };
            
            var downloadResult = await RunJVDownloaderAsync(string.Join(" ", downloadArgs), rawDir);
            if (downloadResult != 0)
            {
                Console.Error.WriteLine($"{dataspec}のダウンロードに失敗しました。");
                continue;
            }
        }
        
        // ダウンロードしたファイルをParquet変換
        var rtFiles = Directory.GetFiles(rawDir, "JVRT-*.txt")
            .OrderBy(f => f)
            .ToList();
        
        if (rtFiles.Count > 0)
        {
            Console.WriteLine($"\n{rtFiles.Count}個のリアルタイムファイルをParquet形式に変換中...");
            
            int convertedCount = 0;
            foreach (var file in rtFiles)
            {
                Console.Write($"\r変換中: {convertedCount + 1}/{rtFiles.Count}");
                var result = await RunJVParquetAsync(file, parquetDir, Array.Empty<string>());
                if (result == 0)
                {
                    convertedCount++;
                }
            }
            Console.WriteLine($"\r変換完了: {convertedCount}/{rtFiles.Count}");
        }
        
        Console.WriteLine($"\nリアルタイムデータ取得が完了しました。");
        Console.WriteLine($"保存先: {parquetDir}");
        
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

// JVDownloaderを実行するヘルパーメソッド
static async Task<int> RunJVDownloaderAsync(string arguments, string? outputDir)
{
    try
    {
        var jvDownloaderPath = GetJVDownloaderPath();
        
        if (!File.Exists(jvDownloaderPath))
        {
            Console.Error.WriteLine($"JVDownloader.exe が見つかりません");
            Console.Error.WriteLine($"ビルドを実行してファイルが正しくコピーされているか確認してください");
            Console.Error.WriteLine($"期待される場所: {jvDownloaderPath}");
            return -1;
        }
        
        var psi = new ProcessStartInfo
        {
            FileName = jvDownloaderPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.GetEncoding(932),  // Shift_JIS
            StandardErrorEncoding = Encoding.GetEncoding(932)    // Shift_JIS
        };
        
        // デバッグ用：実行するコマンドを表示
        Console.WriteLine($"  実行コマンド: {Path.GetFileName(jvDownloaderPath)} {arguments}");
        
        if (!string.IsNullOrEmpty(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        
        using var process = Process.Start(psi);
        if (process == null)
        {
            Console.Error.WriteLine("JVDownloaderの起動に失敗しました。");
            return -1;
        }
        
        // リアルタイムで出力を表示
        var lastProgressTime = DateTime.Now;
        var outputTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                // JVDownloaderの出力をそのまま表示
                Console.WriteLine($"  {line}");
                
                // 進捗表示の間隔を調整（5秒ごとに進行中メッセージ）
                var now = DateTime.Now;
                if ((now - lastProgressTime).TotalSeconds > 5 && line.Contains("JV-"))
                {
                    Console.WriteLine($"  >> ダウンロード進行中... {now:HH:mm:ss}");
                    lastProgressTime = now;
                }
            }
        });
        
        var errorBuilder = new System.Text.StringBuilder();
        var errorTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardError.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    errorBuilder.AppendLine(line);
                }
            }
        });
        
        await process.WaitForExitAsync();
        await Task.WhenAll(outputTask, errorTask);
        
        // エラーがあれば表示
        if (process.ExitCode != 0 && errorBuilder.Length > 0)
        {
            Console.Error.WriteLine($"    エラー: {errorBuilder.ToString().TrimEnd()}");
        }
        
        return process.ExitCode;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"JVDownloader実行エラー: {ex.Message}");
        return -1;
    }
}

// JVParquetを実行するヘルパーメソッド
static async Task<int> RunJVParquetAsync(string inputFile, string outputDir, string[] skipRecordSpecs)
{
    try
    {
        var jvParquetPath = GetJVParquetPath();
        
        if (!File.Exists(jvParquetPath))
        {
            Console.Error.WriteLine($"JVParquet.dll が見つかりません");
            Console.Error.WriteLine($"ビルドを実行してファイルが正しくコピーされているか確認してください");
            Console.Error.WriteLine($"期待される場所: {jvParquetPath}");
            return -1;
        }
        
        var arguments = $"\"{jvParquetPath}\" convert -i \"{inputFile}\" -o \"{outputDir}\"";
        if (skipRecordSpecs.Length > 0)
        {
            arguments += $" -s {string.Join(",", skipRecordSpecs)}";
        }
        
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.GetEncoding(932),  // Shift_JIS
            StandardErrorEncoding = Encoding.GetEncoding(932)    // Shift_JIS
        };
        
        using var process = Process.Start(psi);
        if (process == null)
        {
            Console.Error.WriteLine("JVParquetの起動に失敗しました。");
            return -1;
        }
        
        // 標準出力を非同期で読み取り、進捗を表示
        var outputTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine($"    {line}");
                }
            }
        });
        
        // エラー出力も読み取り
        var errorBuilder = new System.Text.StringBuilder();
        var errorTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardError.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    errorBuilder.AppendLine(line);
                }
            }
        });
        
        await process.WaitForExitAsync();
        await Task.WhenAll(outputTask, errorTask);
        
        // エラーがあれば表示
        if (process.ExitCode != 0 && errorBuilder.Length > 0)
        {
            Console.Error.WriteLine($"    エラー: {errorBuilder.ToString().TrimEnd()}");
        }
        
        return process.ExitCode;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"JVParquet実行エラー: {ex.Message}");
        return -1;
    }
}

// 実行ファイルのパスを取得するヘルパーメソッド
static string GetJVDownloaderPath()
{
    // ビルド済みを前提として、同じディレクトリから探す
    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JVDownloader.exe");
}

static string GetJVParquetPath()
{
    // ビルド済みを前提として、同じディレクトリから探す
    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JVParquet.dll");
}

// 設定ファイル関連のヘルパーメソッド
static string GetConfigPath()
{
    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    var configDir = Path.Combine(appDataPath, "JVDatabase");
    Directory.CreateDirectory(configDir);
    return Path.Combine(configDir, "config.json");
}

static async Task<JVDatabaseConfig> LoadOrCreateConfigAsync(string configPath)
{
    if (File.Exists(configPath))
    {
        try
        {
            var json = await File.ReadAllTextAsync(configPath);
            return JsonSerializer.Deserialize<JVDatabaseConfig>(json) ?? new JVDatabaseConfig();
        }
        catch
        {
            return new JVDatabaseConfig();
        }
    }
    
    var defaultConfig = new JVDatabaseConfig();
    await SaveConfigAsync(configPath, defaultConfig);
    return defaultConfig;
}

static async Task SaveConfigAsync(string configPath, JVDatabaseConfig config)
{
    var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(configPath, json);
}

static int EstimateDataSize(string startDate, string endDate, IEnumerable<string> dataSpecs)
{
    // 簡易的なサイズ推定（1年あたりのGB）
    var sizePerYearPerSpec = new Dictionary<string, double>
    {
        ["RACE"] = 3.0,
        ["TOKU"] = 0.5,
        ["DIFN"] = 2.0,
        ["BLDN"] = 1.0,
        ["SNPN"] = 0.5,
        ["SLOP"] = 1.0,
        ["WOOD"] = 1.0,
        ["YSCH"] = 0.5,
        ["HOSN"] = 0.5,
        ["HOYU"] = 0.5,
        ["MING"] = 0.8,
        ["BLDN"] = 1.0
    };
    
    var startYear = int.Parse(startDate.Substring(0, 4));
    var endYear = int.Parse(endDate.Substring(0, 4));
    var years = endYear - startYear + 1;
    
    double totalSize = 0;
    foreach (var spec in dataSpecs)
    {
        if (sizePerYearPerSpec.ContainsKey(spec))
        {
            totalSize += sizePerYearPerSpec[spec] * years;
        }
    }
    
    return (int)Math.Ceiling(totalSize);
}

// JVダウンロードファイルからlastfiletimestampを抽出
static async Task<(string dataspec, string lastfiletimestamp)?> ExtractJVFileInfoAsync(string filePath)
{
    try
    {
        using var reader = new StreamReader(filePath);
        var firstLine = await reader.ReadLineAsync();
        if (firstLine != null && firstLine.StartsWith("JV DATASPEC:"))
        {
            // 例: "JV DATASPEC:RACE FROMDATE:20231201 LASTFILETIMESTAMP:20231231000000"
            var parts = firstLine.Split(' ');
            string? dataspec = null;
            string? lastfiletimestamp = null;
            
            foreach (var part in parts)
            {
                if (part.StartsWith("DATASPEC:"))
                {
                    dataspec = part.Substring(9);
                }
                else if (part.StartsWith("LASTFILETIMESTAMP:"))
                {
                    lastfiletimestamp = part.Substring(18);
                }
            }
            
            if (dataspec != null && lastfiletimestamp != null)
            {
                return (dataspec, lastfiletimestamp);
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ファイル情報の抽出エラー ({filePath}): {ex.Message}");
    }
    
    return null;
}

// fetch-odds コマンドの実装
static async Task<int> RunFetchOddsAsync(FetchOddsOptions opts)
{
    try
    {
        Console.WriteLine($"指定日のオッズデータを取得します: {opts.TargetDate}");
        
        // 1. RAデータから指定日のレース情報を取得
        var parquetDir = Path.Combine(opts.DatabasePath, "parquet", "RA");
        if (!Directory.Exists(parquetDir))
        {
            Console.Error.WriteLine("RAデータが見つかりません。先に 'init' または 'update' を実行してください。");
            return 1;
        }
        
        // Parquetファイルから指定日のレースを検索
        var targetDate = DateTime.ParseExact(opts.TargetDate, "yyyyMMdd", null);
        var races = new List<(string JyoCD, int RaceNum)>();
        
        // 年月日でパーティション分けされているので、該当ディレクトリを探す
        var yearDir = Path.Combine(parquetDir, $"year={targetDate.Year}");
        var monthDir = Path.Combine(yearDir, $"month={targetDate.Month}");
        var dayDir = Path.Combine(monthDir, $"day={targetDate.Day}");
        
        if (Directory.Exists(dayDir))
        {
            // 該当日のParquetファイルを読み込み
            var parquetFiles = Directory.GetFiles(dayDir, "*.parquet");
            Console.WriteLine($"\n{parquetFiles.Length}個のRAファイルが見つかりました。");
            
            // 簡易的にRAレコードから場コードを抽出
            // 実際にはParquetを読むべきだが、ここではrawファイルから抽出
            var rawDir = Path.Combine(opts.DatabasePath, "raw");
            var rawFiles = Directory.GetFiles(rawDir, "JV-*.txt");
            
            foreach (var rawFile in rawFiles)
            {
                using var reader = new StreamReader(rawFile, Encoding.GetEncoding(932));
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("RA") && line.Length > 30)
                    {
                        // レコード種別IDを確認（RA）
                        var recordSpec = line.Substring(0, 2);
                        if (recordSpec == "RA")
                        {
                            // 開催年月日: 11-18 (8桁)
                            var recordDate = line.Substring(11, 8);
                            if (recordDate == opts.TargetDate)
                            {
                                // 場コード: 19-20 (2桁)
                                var jyoCD = line.Substring(19, 2);
                                // レース番号: 23-24 (2桁)
                                var raceNumStr = line.Substring(23, 2).Trim();
                                if (!string.IsNullOrEmpty(raceNumStr) && int.TryParse(raceNumStr, out int raceNum))
                                {
                                    var race = (jyoCD, raceNum);
                                    if (!races.Contains(race))
                                    {
                                        races.Add(race);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine($"\n{opts.TargetDate}のRAデータが見つかりません。");
            Console.WriteLine($"確認したパス: {dayDir}");
            return 1;
        }
        
        if (races.Count == 0)
        {
            Console.WriteLine($"\n{opts.TargetDate}のレースが見つかりません。");
            Console.WriteLine("RAデータが存在するか、指定した日付が正しいか確認してください。");
            return 1;
        }
        
        // 場コードごとにグループ化
        var jyoGroups = races.GroupBy(r => r.JyoCD).OrderBy(g => g.Key);
        
        Console.WriteLine($"\n開催場数: {jyoGroups.Count()}");
        foreach (var group in jyoGroups)
        {
            var jyoName = GetJyoName(group.Key);
            Console.WriteLine($"  {group.Key}: {jyoName} ({group.Count()}レース)");
        }
        
        // 2. 各場のレースデータを取得
        var outputDir = Path.Combine(opts.DatabasePath, "realtime", "raw");
        Directory.CreateDirectory(outputDir);
        
        foreach (var jyoGroup in jyoGroups)
        {
            var jyoCD = jyoGroup.Key;
            var raceNums = jyoGroup.Select(r => r.RaceNum).OrderBy(n => n).ToList();
            
            Console.WriteLine($"\n[{GetJyoName(jyoCD)}] {opts.DataSpec}データを取得中...");
            
            // レースキーを生成
            var keys = raceNums.Select(num => $"{opts.TargetDate}{jyoCD}{num:00}").ToList();
            
            var downloadArgs = new[]
            {
                "jvrt",
                "--dataspec", opts.DataSpec,
                "--key", string.Join(",", keys),
                "--outputDir", outputDir
            };
            
            var downloadResult = await RunJVDownloaderAsync(string.Join(" ", downloadArgs), outputDir);
            if (downloadResult == 0)
            {
                Console.WriteLine($"  → {keys.Count}レース分のデータを取得しました");
            }
            else
            {
                Console.WriteLine($"  → ダウンロードに失敗しました（エラーコード: {downloadResult}）");
            }
        }
        
        // 3. Parquet変換
        var rtFiles = Directory.GetFiles(outputDir, $"JVRT-{opts.DataSpec}-*.txt");
        if (rtFiles.Length > 0)
        {
            Console.WriteLine($"\n{rtFiles.Length}個のファイルをParquet形式に変換中...");
            
            var parquetOutputDir = Path.Combine(opts.DatabasePath, "realtime", "parquet");
            int convertedCount = 0;
            
            foreach (var file in rtFiles)
            {
                var result = await RunJVParquetAsync(file, parquetOutputDir, Array.Empty<string>());
                if (result == 0)
                {
                    convertedCount++;
                }
            }
            
            Console.WriteLine($"変換完了: {convertedCount}/{rtFiles.Length}");
        }
        
        Console.WriteLine($"\n{opts.TargetDate}のオッズデータ取得が完了しました。");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

// 場コードから場名を取得
static string GetJyoName(string jyoCD)
{
    return jyoCD switch
    {
        "01" => "札幌",
        "02" => "函館",
        "03" => "福島",
        "04" => "新潟",
        "05" => "東京",
        "06" => "中山",
        "07" => "中京",
        "08" => "京都",
        "09" => "阪神",
        "10" => "小倉",
        _ => "不明"
    };
}

// analyze コマンドの実装
static async Task<int> RunAnalyzeAsync(AnalyzeOptions opts)
{
    try
    {
        // メタデータの確認
        var metadataPath = Path.Combine(opts.DatabasePath, ".jvdb", "metadata.json");
        if (!File.Exists(metadataPath))
        {
            Console.Error.WriteLine("データベースが初期化されていません。先に 'init' コマンドを実行してください。");
            return 1;
        }
        
        // Pythonスクリプトのパスを探す
        var analyzePyPath = Path.Combine(opts.DatabasePath, "analysis", "analyze.py");
        
        // 見つからない場合は同じディレクトリから探す
        if (!File.Exists(analyzePyPath))
        {
            analyzePyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jvdb", "analysis", "analyze.py");
        }
        
        // 見つからない場合は相対パスで探す（開発時用）
        if (!File.Exists(analyzePyPath))
        {
            analyzePyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "jvdb", "analysis", "analyze.py");
        }
        
        if (!File.Exists(analyzePyPath))
        {
            Console.Error.WriteLine($"分析スクリプトが見つかりません: {analyzePyPath}");
            Console.Error.WriteLine("Python分析パッケージが正しくインストールされているか確認してください。");
            return 1;
        }
        
        // Pythonコマンドの構築
        var arguments = new List<string> { $"\"{analyzePyPath}\"" };
        
        // パラメータに基づいてコマンドを構築
        if (opts.Command == "realtime-odds")
        {
            arguments.Add("realtime");
            arguments.Add("update-odds");
            arguments.Add("-d");
            arguments.Add($"\"{Path.Combine(opts.DatabasePath, "parquet")}\"");
            
            if (!string.IsNullOrEmpty(opts.Spec))
            {
                arguments.Add("-s");
                arguments.Add(opts.Spec);
            }
            
            if (opts.MinutesBefore > 0)
            {
                arguments.Add("-m");
                arguments.Add(opts.MinutesBefore.ToString());
            }
        }
        else if (opts.Command == "race-stats")
        {
            arguments.Add("stats");
            arguments.Add("race-summary");
            arguments.Add("-s");
            arguments.Add(opts.StartDate ?? DateTime.Now.AddMonths(-1).ToString("yyyyMMdd"));
            arguments.Add("-e");
            arguments.Add(opts.EndDate ?? DateTime.Now.ToString("yyyyMMdd"));
            
            if (!string.IsNullOrEmpty(opts.Output))
            {
                arguments.Add("-o");
                arguments.Add($"\"{opts.Output}\"");
            }
        }
        else
        {
            Console.Error.WriteLine($"未知の分析コマンド: {opts.Command}");
            Console.Error.WriteLine("利用可能なコマンド: realtime-odds, race-stats");
            return 1;
        }
        
        // Pythonの実行
        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = string.Join(" ", arguments),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(analyzePyPath)
        };
        
        Console.WriteLine($"実行コマンド: python {string.Join(" ", arguments)}");
        
        using var process = Process.Start(psi);
        if (process == null)
        {
            Console.Error.WriteLine("Pythonの起動に失敗しました。");
            Console.Error.WriteLine("Pythonがインストールされているか確認してください。");
            return -1;
        }
        
        // 出力を表示
        var outputTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                Console.WriteLine(line);
            }
        });
        
        var errorTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardError.ReadLineAsync()) != null)
            {
                Console.Error.WriteLine(line);
            }
        });
        
        await process.WaitForExitAsync();
        await Task.WhenAll(outputTask, errorTask);
        
        return process.ExitCode;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        return 1;
    }
}

namespace JVDatabase
{
    // 設定ファイルクラス
    public class JVDatabaseConfig
    {
        public string DefaultDatabasePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "JVDatabase");
        
        // デフォルトはRACE関連とマスター系データ
        public List<string> DefaultDataSpecs { get; set; } = new() 
        { 
            "TOKU",  // 特別登録馬（TK）
            "RACE",  // レース関連（RA,SE,HR,H1,H6,O1-O6,WF,JG）
            "SLOP",  // 坂路調教（HC）
            "WOOD",  // ウッドチップ調教（WC）
            "YSCH",  // 予想（騎手、厩舎、馬、総合）（YS）
            "HOYU",  // 馬主（HY）
            "MING",  // マイニング（TM,DM）
            "SNPN",  // SNAP（SN）
            "HOSN",  // 競走馬市場取引価格情報（HS）
            "DIFN"   // マスター系（UM,KS,CH,BR,BN,RC）
        };
        
        // 大容量のオッズ系レコードはデフォルトでスキップ
        public List<string> SkipRecordSpecs { get; set; } = new() { "H6", "O6" };
        public bool AutoBackup { get; set; } = false;
        public string? BackupPath { get; set; }
    }

    // コマンドラインオプションの定義
    [Verb("config", HelpText = "JVDatabaseの設定を管理")]
    public class ConfigOptions
    {
        [Option('l', "list", Required = false, HelpText = "現在の設定を表示")]
        public bool List { get; set; }
    }

    [Verb("init", HelpText = "JVDatabaseを初期化し、初期データをダウンロード")]
    public class InitOptions
    {
        [Option('i', "interactive", Required = false, HelpText = "対話的に設定を行う")]
        public bool Interactive { get; set; }
        [Option('p', "path", Required = false, Default = "./jvdb", HelpText = "データベースのパス")]
        public string DatabasePath { get; set; } = "./jvdb";
        
        [Option('s', "start", Required = false, HelpText = "開始日 (YYYYMMDD形式)")]
        public string StartDate { get; set; } = "";
        
        [Option('e', "end", Required = false, HelpText = "終了日 (YYYYMMDD形式)")]
        public string EndDate { get; set; } = "";
        
        [Option('d', "dataspec", Required = false, Default = new[] { "TOKU", "RACE", "DIFN", "BLDN", "SNPN", "SLOP", "WOOD", "YSCH", "HOSN", "HOYU" }, 
            Separator = ',', HelpText = "取得するデータ種別")]
        public IEnumerable<string> DataSpecs { get; set; } = new[] { "TOKU", "RACE", "DIFN", "BLDN", "SNPN", "SLOP", "WOOD", "YSCH", "HOSN", "HOYU" };
        
        [Option("skip", Required = false, Default = new[] { "H6", "O6" }, Separator = ',', HelpText = "スキップするレコード種別")]
        public IEnumerable<string> SkipRecordSpecs { get; set; } = new[] { "H6", "O6" };
    }
    
    [Verb("update", HelpText = "データベースを最新の状態に更新")]
    public class UpdateOptions
    {
        [Option('p', "path", Required = false, Default = "./jvdb", HelpText = "データベースのパス")]
        public string DatabasePath { get; set; } = "./jvdb";
        
        [Option('e', "end", Required = false, HelpText = "終了日 (YYYYMMDD形式、省略時は今日)")]
        public string? EndDate { get; set; }
    }
    
    [Verb("realtime", HelpText = "リアルタイム（速報系）データを取得")]
    public class RealtimeOptions
    {
        [Option('p', "path", Required = false, Default = "./jvdb", HelpText = "データベースのパス")]
        public string DatabasePath { get; set; } = "./jvdb";
        
        [Option('t', "target", Required = false, HelpText = "対象日 (YYYYMMDD形式、省略時は今日)")]
        public string TargetDate { get; set; } = DateTime.Now.ToString("yyyyMMdd");
        
        [Option('d', "dataspec", Required = false, Default = new[] { "0B15", "0B30", "0B11" }, 
            Separator = ',', HelpText = "取得するデータ種別")]
        public IEnumerable<string> DataSpecs { get; set; } = new[] { "0B15", "0B30", "0B11" };
    }
    
    [Verb("analyze", HelpText = "データベースを分析")]
    public class AnalyzeOptions
    {
        [Option('p', "path", Required = false, Default = "./jvdb", HelpText = "データベースのパス")]
        public string DatabasePath { get; set; } = "./jvdb";
        
        [Option('c', "command", Required = true, HelpText = "分析コマンド (realtime-odds, race-stats)")]
        public string Command { get; set; } = "";
        
        [Option('s', "start", Required = false, HelpText = "開始日 (YYYYMMDD形式)")]
        public string? StartDate { get; set; }
        
        [Option('e', "end", Required = false, HelpText = "終了日 (YYYYMMDD形式)")]
        public string? EndDate { get; set; }
        
        [Option("spec", Required = false, HelpText = "レコード種別 (O1, O2等)")]
        public string? Spec { get; set; }
        
        [Option('m', "minutes", Required = false, Default = 10, HelpText = "レース発走何分前のデータか")]
        public int MinutesBefore { get; set; } = 10;
        
        [Option('o', "output", Required = false, HelpText = "出力ファイルパス")]
        public string? Output { get; set; }
    }
    
    [Verb("fetch-odds", HelpText = "指定日の時系列オッズデータを取得")]
    public class FetchOddsOptions
    {
        [Option('p', "path", Required = false, Default = "./jvdb", HelpText = "データベースのパス")]
        public string DatabasePath { get; set; } = "./jvdb";
        
        [Option('d', "date", Required = true, HelpText = "対象日 (YYYYMMDD形式)")]
        public string TargetDate { get; set; } = "";
        
        [Option('s', "spec", Required = false, Default = "0B41", HelpText = "データ種別（0B11:票数時系列、0B12:払戻金時系列、0B15:レース結果、0B30:馬体重、0B31:単複枠最終オッズ、0B32:馬連最終オッズ、0B33:ワイド最終オッズ、0B34:馬単最終オッズ、0B35:三連複最終オッズ、0B36:三連単最終オッズ、0B41:単複枠時系列オッズ、0B42:馬連時系列オッズ）")]
        public string DataSpec { get; set; } = "0B41";
    }
    
    // データベースメタデータ
    public class DatabaseMetadata
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public List<string> DataSpecs { get; set; } = new();
        public string DatabasePath { get; set; } = "";
        public string Version { get; set; } = "";
        
        // 差分更新用の追加フィールド
        public Dictionary<string, DataSpecStatus> DataSpecStatuses { get; set; } = new();
        public List<string> ProcessedFiles { get; set; } = new();
    }
    
    // データ種別ごとの更新状態
    public class DataSpecStatus
    {
        public string LastFileTimestamp { get; set; } = "";
        public string LastFileName { get; set; } = "";
        public DateTime LastUpdatedAt { get; set; }
    }
}