using System.Text;
using CommandLine;
using JVParquet;

// Shift-JISエンコーディングを登録
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
        ValidateAndCreateOutputDirectory(options.OutputDir);
        
        var skipRecordSpecs = ParseSkipRecordSpecs(options.SkipRecordSpec);
        var inputFileName = Path.GetFileNameWithoutExtension(options.InputPath);
        
        await using var converter = new JVDataParquetConverter(options.OutputDir, inputFileName);
        
        var (lineCount, recordCount) = await ProcessFileAsync(
            options.InputPath, 
            converter, 
            skipRecordSpecs);

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

static void ValidateAndCreateOutputDirectory(string outputDir)
{
    if (!Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }
}

static HashSet<string> ParseSkipRecordSpecs(string? skipRecordSpec)
{
    if (string.IsNullOrEmpty(skipRecordSpec))
        return new HashSet<string>();
    
    return new HashSet<string>(skipRecordSpec.Split(','));
}

static async Task<(int lineCount, int recordCount)> ProcessFileAsync(
    string inputPath,
    JVDataParquetConverter converter,
    HashSet<string> skipRecordSpecs)
{
    Console.WriteLine($"Processing: {inputPath}");
    
    using var reader = new StreamReader(inputPath);
    
    int lineCount = 0;
    int recordCount = 0;
    string? line;

    while ((line = reader.ReadLine()) != null)
    {
        lineCount++;

        if (ShouldSkipLine(line))
            continue;

        var recordSpec = line.Substring(0, Constants.RecordSpecLength);
        
        if (skipRecordSpecs.Contains(recordSpec))
            continue;

        var result = await converter.ProcessRecordAsync(line);
        if (result.IsSuccess)
        {
            recordCount++;
            ReportProgressIfNeeded(recordCount);
        }
        else
        {
            Console.WriteLine($"Warning: Failed to process line {lineCount}: {result.Error}");
        }
    }

    return (lineCount, recordCount);
}

static bool ShouldSkipLine(string? line)
{
    return string.IsNullOrEmpty(line) || 
           line.StartsWith("JV") || 
           line.Length < Constants.RecordSpecLength;
}

static void ReportProgressIfNeeded(int recordCount)
{
    if (recordCount % Constants.ProgressReportInterval == 0)
    {
        Console.WriteLine($"Processed {recordCount} records...");
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
