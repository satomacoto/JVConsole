using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace JVParquet.Tests.Integration
{
    /// <summary>
    /// シンプルな統合テスト - 実際のコマンドラインツールの動作を確認
    /// </summary>
    public class SimpleIntegrationTest
    {
        public SimpleIntegrationTest()
        {
            // Shift-JISエンコーディングを登録
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        
        [Fact]
        public async Task ConvertCommand_WithSampleData_ShouldSucceed()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), $"JVParquetSimpleTest_{Guid.NewGuid()}");
            var inputFile = Path.Combine(tempDir, "test_input.txt");
            var outputDir = Path.Combine(tempDir, "output");
            
            try
            {
                Directory.CreateDirectory(tempDir);
                
                // サンプルデータを準備（HNレコード - 馬名データ）
                var sampleData = @"HN12025040312200743310000000020171058850アークストーン　　　　　　　　　　　ｱｰｸｽﾄｰﾝ                                 Arc Stone                                                                       2017210100000浦河町　　　　　　　11200020241220060225
HN12025040312200743320000000020191030730マリブレディ　　　　　　　　　　　　ﾏﾘﾌﾞﾚﾃﾞｨ                                Malibu Lady                                                                     2019210300000新ひだか町　　　　　11200023901220065625";
                
                await File.WriteAllTextAsync(inputFile, sampleData);
                
                // Act - プログラムを直接実行する代わりに、コンバーターを使用
                var converter = new JVDataParquetConverter(outputDir, "test_input");
                
                using var reader = new StreamReader(inputFile);
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("JV"))
                    {
                        await converter.ProcessRecordAsync(line);
                    }
                }
                
                await converter.DisposeAsync();
                
                // Assert
                var parquetFiles = Directory.GetFiles(outputDir, "*.parquet", SearchOption.AllDirectories);
                parquetFiles.Should().NotBeEmpty();
                parquetFiles.Should().Contain(f => f.Contains("HN"));
                
                // ファイルが存在し、サイズが0より大きいことを確認
                foreach (var file in parquetFiles)
                {
                    var fileInfo = new FileInfo(file);
                    fileInfo.Exists.Should().BeTrue();
                    fileInfo.Length.Should().BeGreaterThan(0);
                }
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempDir))
                {
                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch
                    {
                        // テスト環境でのクリーンアップエラーは無視
                    }
                }
            }
        }
    }
}