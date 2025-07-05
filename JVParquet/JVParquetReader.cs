using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Parquet;
using Parquet.Data;

namespace JVParquet
{
    public class JVParquetReader
    {
        // 各レコードタイプのPython定義でのindex_cols
        private static readonly Dictionary<string, List<string>> RecordIndexMapping = new()
        {
            ["RA"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["SE"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum", "Umaban", "KettoNum" },
            ["HR"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O1"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O2"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O3"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O4"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O5"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["O6"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["H1"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["H6"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["CK"] = new() { "ChokyoDate.Year", "ChokyoDate.Month", "ChokyoDate.Day", "ChokyoshiCode", "JyoCD" },
            ["WC"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum", "id.Umaban" },
            ["WF"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["UM"] = new() { "KettoNum" },
            ["SK"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum" },
            ["HN"] = new() { "HansyokuFNum", "KettoNum", "BirthDate.Year" },
            ["BT"] = new() { "HansyokuNum" },
            ["JG"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Umaban", "KettoNum" },
            ["HC"] = new() { "id.Year", "id.MonthDay", "id.JyoCD", "id.Kaiji", "id.Nichiji", "id.RaceNum", "id.Umaban" }
        };
        public static async Task ReadParquetFileAsync(string filePath, int maxRows = 10)
        {
            try
            {
                Console.WriteLine($"Reading Parquet file: {filePath}");
                Console.WriteLine(new string('-', 80));

                using var fileStream = File.OpenRead(filePath);
                using var parquetReader = await ParquetReader.CreateAsync(fileStream);

                // ファイルのメタデータを表示
                Console.WriteLine($"Row Groups: {parquetReader.RowGroupCount}");
                
                // カスタムメタデータを表示
                if (parquetReader.CustomMetadata != null && parquetReader.CustomMetadata.Count > 0)
                {
                    Console.WriteLine("\nCustom Metadata:");
                    foreach (var kvp in parquetReader.CustomMetadata)
                    {
                        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }
                Console.WriteLine();

                // スキーマ情報を表示
                var schema = parquetReader.Schema;
                Console.WriteLine("Schema:");
                foreach (var field in schema.GetDataFields())
                {
                    Console.WriteLine($"  {field.Name} ({field.ClrType.Name})");
                }
                Console.WriteLine();

                // データを読み込んで表示
                Console.WriteLine($"First {maxRows} rows:");
                Console.WriteLine(new string('-', 80));

                int rowsRead = 0;
                for (int i = 0; i < parquetReader.RowGroupCount && rowsRead < maxRows; i++)
                {
                    using var rowGroupReader = parquetReader.OpenRowGroupReader(i);
                    var columns = new Dictionary<string, Array>();

                    // 各カラムのデータを読み込む
                    foreach (var field in schema.GetDataFields())
                    {
                        var column = await rowGroupReader.ReadColumnAsync(field);
                        columns[field.Name] = column.Data;
                    }

                    // 行数を取得
                    int rowCount = columns.First().Value.Length;
                    int rowsToDisplay = Math.Min(rowCount, maxRows - rowsRead);

                    // 各行を表示
                    for (int row = 0; row < rowsToDisplay; row++)
                    {
                        Console.WriteLine($"Row {rowsRead + row + 1}:");
                        foreach (var col in columns)
                        {
                            var value = col.Value.GetValue(row);
                            Console.WriteLine($"  {col.Key}: {value ?? "null"}");
                        }
                        Console.WriteLine();
                    }

                    rowsRead += rowsToDisplay;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Parquet file: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static async Task<List<string>> GetParquetColumnsAsync(string filePath)
        {
            var columns = new List<string>();
            
            try
            {
                using var fileStream = File.OpenRead(filePath);
                using var parquetReader = await ParquetReader.CreateAsync(fileStream);
                
                var schema = parquetReader.Schema;
                foreach (var field in schema.GetDataFields())
                {
                    columns.Add(field.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Parquet schema: {ex.Message}");
            }

            return columns;
        }

        public static async Task AnalyzeParquetDirectoryAsync(string directory, string recordSpec = "CK")
        {
            Console.WriteLine($"# {recordSpec}レコードの分析");
            Console.WriteLine(new string('=', 80));

            var parquetFiles = Directory.GetFiles(directory, "*.parquet", SearchOption.AllDirectories)
                .Where(f => f.Contains($"/{recordSpec}/") || f.Contains($"\\{recordSpec}\\"))
                .ToArray();
            
            if (parquetFiles.Length == 0)
            {
                Console.WriteLine($"No Parquet files found for record spec {recordSpec}");
                return;
            }

            var file = parquetFiles.First();
            Console.WriteLine($"\nファイル: {Path.GetFileName(file)}");
            
            var columns = await GetParquetColumnsAsync(file);
            
            // Python定義のindex_colsとの対応を分析
            if (RecordIndexMapping.ContainsKey(recordSpec))
            {
                Console.WriteLine("\n## インデックスカラムの対応");
                Console.WriteLine("| Python定義 | Parquetカラム名 | 存在 |");
                Console.WriteLine("|------------|----------------|------|");
                
                var pythonIndexCols = RecordIndexMapping[recordSpec];
                foreach (var pyCol in pythonIndexCols)
                {
                    // 完全一致を探す
                    if (columns.Contains(pyCol))
                    {
                        Console.WriteLine($"| {pyCol} | {pyCol} | ✓ |");
                    }
                    else
                    {
                        // ドットをアンダースコアに変換したパターンを探す
                        var underscored = pyCol.Replace(".", "_");
                        if (columns.Contains(underscored))
                        {
                            Console.WriteLine($"| {pyCol} | {underscored} | ✓ |");
                        }
                        else
                        {
                            // 類似のカラムを探す
                            var similarColumn = FindSimilarColumn(pyCol, columns);
                            if (similarColumn != null)
                            {
                                Console.WriteLine($"| {pyCol} | {similarColumn} | ✓ |");
                            }
                            else
                            {
                                Console.WriteLine($"| {pyCol} | - | ✗ |");
                            }
                        }
                    }
                }
            }
            
            // 実際のカラムを表示
            Console.WriteLine($"\n## 実際のカラム（最初の30個）");
            int index = 1;
            foreach (var col in columns.Take(30))
            {
                Console.WriteLine($"{index}. `{col}`");
                index++;
            }
            
            if (columns.Count > 30)
            {
                Console.WriteLine($"\n... 他 {columns.Count - 30} カラム");
            }
            
            Console.WriteLine();
        }

        private static string? FindSimilarColumn(string pythonCol, List<string> parquetColumns)
        {
            // ドットを除去してマッチングを試みる
            var normalizedPyCol = pythonCol.Replace(".", "").ToLower();
            
            foreach (var parquetCol in parquetColumns)
            {
                var normalizedParquetCol = parquetCol.Replace("_", "").ToLower();
                if (normalizedParquetCol.Contains(normalizedPyCol) || normalizedPyCol.Contains(normalizedParquetCol))
                {
                    return parquetCol;
                }
            }
            
            return null;
        }
    }
}