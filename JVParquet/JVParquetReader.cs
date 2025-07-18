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

                using var fileStream = File.OpenRead(filePath);
                using var parquetReader = await ParquetReader.CreateAsync(fileStream);

                // ファイルのメタデータ
                var rowGroupCount = parquetReader.RowGroupCount;
                
                // カスタムメタデータ
                var customMetadata = parquetReader.CustomMetadata;

                // スキーマ情報
                var schema = parquetReader.Schema;

                // データを読み込み

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

                    // 行データの処理
                    for (int row = 0; row < rowsToDisplay; row++)
                    {
                        foreach (var col in columns)
                        {
                            var value = col.Value.GetValue(row);
                        }
                    }

                    rowsRead += rowsToDisplay;
                }
            }
            catch (Exception ex)
            {
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
            }

            return columns;
        }

        public static async Task AnalyzeParquetDirectoryAsync(string directory, string recordSpec = "CK")
        {

            var parquetFiles = Directory.GetFiles(directory, "*.parquet", SearchOption.AllDirectories)
                .Where(f => f.Contains($"/{recordSpec}/") || f.Contains($"\\{recordSpec}\\"))
                .ToArray();
            
            if (parquetFiles.Length == 0)
            {
                return;
            }

            var file = parquetFiles.First();
            
            var columns = await GetParquetColumnsAsync(file);
            
            // Python定義のindex_colsとの対応を分析
            if (RecordIndexMapping.ContainsKey(recordSpec))
            {
                
                var pythonIndexCols = RecordIndexMapping[recordSpec];
                foreach (var pyCol in pythonIndexCols)
                {
                    // 完全一致を探す
                    if (columns.Contains(pyCol))
                    {
                        // 完全一致
                    }
                    else
                    {
                        // ドットをアンダースコアに変換したパターンを探す
                        var underscored = pyCol.Replace(".", "_");
                        if (columns.Contains(underscored))
                        {
                            // アンダースコア変換でマッチ
                        }
                        else
                        {
                            // 類似のカラムを探す
                            var similarColumn = FindSimilarColumn(pyCol, columns);
                            if (similarColumn != null)
                            {
                                // 類似カラムでマッチ
                            }
                            else
                            {
                                // マッチなし
                            }
                        }
                    }
                }
            }
            
            // 実際のカラム情報
            var displayColumns = columns.Take(30);
            var remainingCount = Math.Max(0, columns.Count - 30);
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