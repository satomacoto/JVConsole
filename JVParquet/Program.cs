using System.Text;
using CommandLine;
using JVParquet;
using JVParquet.Tests;

// Shift-JISエンコーディングを登録
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// テストモード実行
if (args.Length > 0 && args[0] == "test")
{
    Console.WriteLine("Running JVParquet Conversion Test...");
    var test = new ParquetConversionTest();
    
    try
    {
        var testResult = await test.RunFullConversionTestAsync();
        test.GenerateReport(testResult);
        return testResult.ConversionSucceeded ? 0 : 1;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Test failed: {ex.Message}");
        return 1;
    }
    finally
    {
        test.Cleanup();
    }
}
// SEレコードのテスト実行
else if (args.Length > 0 && args[0] == "test-se")
{
    Console.WriteLine("Running SE Record Type Conversion Test...");
    TestSERecords.RunTest();
    return 0;
}
// SEレコードのParquet変換テスト実行
else if (args.Length > 0 && args[0] == "test-se-parquet")
{
    Console.WriteLine("Running SE Record Parquet Conversion Test...");
    await SimpleSETest.RunTest();
    return 0;
}

// コマンドライン引数の解析と実行
var result = Parser.Default.ParseArguments<Options, ReadOptions>(args);

if (result.Tag == ParserResultType.Parsed)
{
    if (result.Value is Options options)
    {
        return await RunJVParquetAsync(options);
    }
    else if (result.Value is ReadOptions readOptions)
    {
        return await RunReadParquetAsync(readOptions);
    }
}

return 1;

static async Task<int> RunJVParquetAsync(Options options)
{
    try
    {
        // 出力ディレクトリの作成
        if (!Directory.Exists(options.OutputDir))
        {
            Directory.CreateDirectory(options.OutputDir);
        }

        // スキップするレコード種別のセット
        var skipRecordSpecs = new HashSet<string>();
        if (!string.IsNullOrEmpty(options.SkipRecordSpec))
        {
            skipRecordSpecs = new HashSet<string>(options.SkipRecordSpec.Split(','));
        }

        // 入力ファイル名からプレフィックスを生成
        var inputFileName = Path.GetFileNameWithoutExtension(options.InputPath);
        
        // コンバーターの初期化
        var converter = new JVDataParquetConverter(options.OutputDir, inputFileName);

        // 入力ファイルの処理
        Console.WriteLine($"Processing: {options.InputPath}");
        
        using var reader = new StreamReader(options.InputPath);
        
        string? line;
        int lineCount = 0;
        int recordCount = 0;

        while ((line = reader.ReadLine()) != null)
        {
            lineCount++;

            // 空行またはヘッダー行をスキップ
            if (string.IsNullOrEmpty(line) || line.StartsWith("JV"))
            {
                continue;
            }

            // レコード種別の取得（最初の2文字）
            if (line.Length < 2)
            {
                continue;
            }

            string recordSpec = line.Substring(0, 2);

            // スキップ対象のレコード種別の場合
            if (skipRecordSpecs.Contains(recordSpec))
            {
                continue;
            }

            // レコードの処理
            await converter.ProcessRecordAsync(line);
            recordCount++;

            // 進捗表示
            if (recordCount % 1000 == 0)
            {
                Console.WriteLine($"Processed {recordCount} records...");
            }
        }

        // 最終的な書き込みとクローズ
        await converter.DisposeAsync();

        Console.WriteLine($"Completed! Processed {recordCount} records from {lineCount} lines.");
        Console.WriteLine($"Output directory: {options.OutputDir}");

        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Console.Error.WriteLine(ex.StackTrace);
        return 1;
    }
}

static async Task<int> RunReadParquetAsync(ReadOptions options)
{
    try
    {
        if (!string.IsNullOrEmpty(options.FilePath))
        {
            // 単一ファイルを読む
            await JVParquetReader.ReadParquetFileAsync(options.FilePath, options.MaxRows);
        }
        else if (!string.IsNullOrEmpty(options.Directory))
        {
            // ディレクトリを分析
            await JVParquetReader.AnalyzeParquetDirectoryAsync(options.Directory, options.RecordSpec);
        }
        else
        {
            Console.Error.WriteLine("Either --file or --directory must be specified");
            return 1;
        }

        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Console.Error.WriteLine(ex.StackTrace);
        return 1;
    }
}

namespace JVParquet
{
    [Verb("convert", true, HelpText = "Convert JV text data to Parquet format")]
    public class Options
    {
        [Option('i', "inputPath", Required = true, HelpText = "Input text file path")]
        public string InputPath { get; set; } = string.Empty;

        [Option('o', "outputDir", Required = true, HelpText = "Output directory for Parquet files")]
        public string OutputDir { get; set; } = string.Empty;

        [Option('s', "skipRecordSpec", Required = false, HelpText = "Comma-separated list of record specifications to skip")]
        public string? SkipRecordSpec { get; set; }
    }

    [Verb("read", HelpText = "Read and analyze Parquet files")]
    public class ReadOptions
    {
        [Option('f', "file", Required = false, HelpText = "Parquet file to read")]
        public string? FilePath { get; set; }

        [Option('d', "directory", Required = false, HelpText = "Directory containing Parquet files")]
        public string? Directory { get; set; }

        [Option('r', "recordSpec", Required = false, Default = "CK", HelpText = "Record specification to analyze")]
        public string RecordSpec { get; set; } = "CK";

        [Option('m', "maxRows", Required = false, Default = 10, HelpText = "Maximum number of rows to display")]
        public int MaxRows { get; set; } = 10;
    }
}
