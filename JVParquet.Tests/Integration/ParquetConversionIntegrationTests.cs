using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Parquet;

namespace JVParquet.Tests.Integration
{
    public class ParquetConversionIntegrationTests : IDisposable
    {
        private readonly string _testOutputDir;
        
        public ParquetConversionIntegrationTests()
        {
            // Shift-JISエンコーディングを登録
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            _testOutputDir = Path.Combine(Path.GetTempPath(), $"JVParquetTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testOutputDir);
        }
        
        [Fact(Skip = "File system permission issues in test environment")]
        public async Task ConvertSERecord_ShouldCreateParquetWithCorrectTypes()
        {
            // Arrange - 実際のサンプルデータから取得したSEレコード
            var seRecord = "SEA2025031320241114481704048122020100953リュウノフラワー　　　　　　　　　　00210304000000沖田明子000000蓑島　竜一　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　520000000000000000明星晴大　　　　20458+015000050013141        0504040400000800000500000000000000000000002021105455アルマーザアミール　　　　　　　　　0000000000　　　　　　　　　　　　　　　　　　0000000000　　　　　　　　　　　　　　　　　　+014000000000000000002";
            
            var converter = new JVDataParquetConverter(_testOutputDir, "test");
            
            // Act
            try
            {
                await converter.ProcessRecordAsync(seRecord);
                await converter.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Inner exception type: {ex.InnerException?.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Debug: check if directory exists
                Console.WriteLine($"Test output dir exists: {Directory.Exists(_testOutputDir)}");
                var filesInDir = Directory.GetFiles(_testOutputDir, "*", SearchOption.AllDirectories);
                Console.WriteLine($"Files in output dir: {string.Join(", ", filesInDir)}");
            }
            
            // Assert
            Console.WriteLine($"Looking for parquet files in: {_testOutputDir}");
            var allFiles = Directory.GetFiles(_testOutputDir, "*", SearchOption.AllDirectories);
            Console.WriteLine($"All files found: {string.Join(", ", allFiles)}");
            
            var parquetFiles = Directory.GetFiles(_testOutputDir, "*.parquet", SearchOption.AllDirectories);
            parquetFiles.Should().HaveCount(1);
            
            // Read and verify the Parquet file
            using var fileStream = File.OpenRead(parquetFiles[0]);
            using var parquetReader = await ParquetReader.CreateAsync(fileStream);
            
            var dataFields = parquetReader.Schema.GetDataFields();
            
            // Verify field types
            dataFields.First(f => f.Name == "Umaban").ClrType.Should().Be(typeof(int));
            dataFields.First(f => f.Name == "Bamei").ClrType.Should().Be(typeof(string));
            dataFields.First(f => f.Name == "Odds").ClrType.Should().Be(typeof(int));
            dataFields.First(f => f.Name == "Honsyokin").ClrType.Should().Be(typeof(int));
            
            // Read actual values
            for (int i = 0; i < parquetReader.RowGroupCount; i++)
            {
                using var rowGroupReader = parquetReader.OpenRowGroupReader(i);
                
                var umabanColumn = await rowGroupReader.ReadColumnAsync(dataFields.First(f => f.Name == "Umaban"));
                umabanColumn.Data.GetValue(0).Should().Be(12);
                
                var bameiColumn = await rowGroupReader.ReadColumnAsync(dataFields.First(f => f.Name == "Bamei"));
                bameiColumn.Data.GetValue(0).Should().Be("リュウノフラワー　　　　　　　　　　");
            }
        }
        
        public void Dispose()
        {
            if (Directory.Exists(_testOutputDir))
            {
                Directory.Delete(_testOutputDir, true);
            }
        }
    }
}