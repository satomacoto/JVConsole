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
            Console.WriteLine($"Analyzing Parquet files for record spec: {recordSpec}");
            Console.WriteLine(new string('=', 80));

            var parquetFiles = Directory.GetFiles(directory, "*.parquet", SearchOption.AllDirectories)
                .Where(f => f.Contains($"/{recordSpec}/") || f.Contains($"\\{recordSpec}\\"))
                .ToArray();
            
            if (parquetFiles.Length == 0)
            {
                Console.WriteLine($"No Parquet files found for record spec {recordSpec}");
                return;
            }

            foreach (var file in parquetFiles)
            {
                Console.WriteLine($"\nFile: {file}");
                var columns = await GetParquetColumnsAsync(file);
                
                // head関連のカラムを探す
                var headColumns = columns.Where(c => c.StartsWith("head", StringComparison.OrdinalIgnoreCase)).ToList();
                
                if (headColumns.Any())
                {
                    Console.WriteLine("Found head columns:");
                    foreach (var col in headColumns)
                    {
                        Console.WriteLine($"  - {col}");
                    }
                }
                else
                {
                    Console.WriteLine("No head columns found!");
                    Console.WriteLine("First 10 columns:");
                    foreach (var col in columns.Take(10))
                    {
                        Console.WriteLine($"  - {col}");
                    }
                }

                // 最初の数行を読んで実際の値を確認
                await ReadParquetFileAsync(file, 1);
                break; // 最初のファイルだけ詳細表示
            }
        }
    }
}