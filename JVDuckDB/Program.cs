using CommandLine;
using System.Text;
using JVDuckDB;

// Shift-JISを使用可能にする
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// テストモード（TestEncodingクラスが存在しない場合はコメントアウト）
// if (args.Length > 0 && args[0] == "--test-encoding")
// {
//     TestEncoding.TestJVDataEncoding();
//     return 0;
// }

// Parquet読み込みモード
if (args.Length > 0 && args[0] == "--read-parquet")
{
    if (args.Length < 2)
    {
        Console.WriteLine("使用方法: JVDuckDB --read-parquet <parquetファイルパス>");
        return 1;
    }
    ReadParquet.ReadRAParquet(args[1]);
    return 0;
}

// コマンドライン引数の解析と実行
return await Parser.Default.ParseArguments<ConvertOptions>(args)
    .MapResult(
        async (ConvertOptions opts) => await RunConvertAsync(opts),
        async errs => await Task.FromResult(1));

static async Task<int> RunConvertAsync(ConvertOptions opts)
{
    try
    {
        Console.WriteLine($"JVDuckDB - JVデータをParquet形式に変換します");
        Console.WriteLine($"入力: {opts.InputPath}");
        Console.WriteLine($"出力: {opts.OutputPath}");
        
        // プロセッサーの作成と実行
        // Directモードを使用（重複排除なし、高速処理）
        var processor = new JVDuckDB.JVDuckDBProcessorDirect(opts);
        await processor.ProcessAsync();
        
        Console.WriteLine("変換が完了しました。");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"エラーが発生しました: {ex.Message}");
        Console.Error.WriteLine(ex.StackTrace);
        return 1;
    }
}

namespace JVDuckDB
{
    [Verb("convert", isDefault: true, HelpText = "JVデータをParquet形式に変換")]
    public class ConvertOptions
    {
        [Option('i', "input", Required = true, HelpText = "入力ディレクトリまたはファイルパス")]
        public string InputPath { get; set; } = "";
        
        [Option('o', "output", Required = true, HelpText = "出力ディレクトリパス")]
        public string OutputPath { get; set; } = "";
        
        [Option('s', "skip", Required = false, Separator = ',', HelpText = "スキップするレコード種別（例: H6,O6）")]
        public IEnumerable<string> SkipRecordSpecs { get; set; } = Enumerable.Empty<string>();
        
        [Option('b', "batch-size", Required = false, Default = 10000, HelpText = "バッチサイズ")]
        public int BatchSize { get; set; } = 10000;
        
        [Option('m', "memory", Required = false, Default = ":memory:", HelpText = "DuckDBデータベースパス（デフォルト: メモリ）")]
        public string DatabasePath { get; set; } = ":memory:";
        
        [Option('f', "filter", Required = false, HelpText = "処理するレコード種別（指定しない場合は全て）")]
        public IEnumerable<string> FilterRecordSpecs { get; set; } = Enumerable.Empty<string>();
        
        [Option('d', "dedupe", Required = false, Default = true, HelpText = "重複排除を行うか")]
        public bool Deduplicate { get; set; } = true;
        
        [Option('v', "verbose", Required = false, Default = false, HelpText = "詳細ログを出力")]
        public bool Verbose { get; set; } = false;
    }
}