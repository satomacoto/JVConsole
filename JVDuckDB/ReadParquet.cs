using DuckDB.NET.Data;
using System;
using System.Data;

namespace JVDuckDB
{
    public class ReadParquet
    {
        public static void ReadRAParquet(string parquetPath)
        {
            using var connection = new DuckDBConnection("DataSource=:memory:");
            connection.Open();

            // 2025年7月5日のレース情報を取得
            var query = $@"
                SELECT 
                    id_Year,
                    id_MonthDay,
                    id_JyoCD,
                    id_RaceNum,
                    JyokenName,
                    Kyori,
                    TrackCD,
                    COUNT(*) as count
                FROM read_parquet('{parquetPath}')
                WHERE id_Year = '2025' AND id_MonthDay = '0705'
                GROUP BY id_Year, id_MonthDay, id_JyoCD, id_RaceNum, JyokenName, Kyori, TrackCD
                ORDER BY id_MonthDay, id_JyoCD, id_RaceNum
            ";

            using var command = new DuckDBCommand(query, connection);
            using var reader = command.ExecuteReader();

            Console.WriteLine("2025年7月5日のレース情報:");
            Console.WriteLine("======================================");
            Console.WriteLine("年    月日  場  R# レース名                        距離   トラック");
            Console.WriteLine("--------------------------------------");

            int raceCount = 0;
            while (reader.Read())
            {
                var year = reader.GetString(0);
                var monthDay = reader.GetString(1);
                var jyoCD = reader.GetString(2);
                var raceNum = reader.GetString(3);
                var raceName = reader.GetString(4);
                var kyori = reader.GetInt32(5);
                var trackCD = reader.GetString(6);

                Console.WriteLine($"{year} {monthDay} {jyoCD} {raceNum,2} {raceName,-30} {kyori,4}m {trackCD}");
                raceCount++;
            }

            Console.WriteLine($"\n合計レース数: {raceCount}");

            // 日付ごとの集計
            var summaryQuery = $@"
                SELECT 
                    id_MonthDay,
                    COUNT(DISTINCT id_JyoCD || id_RaceNum) as race_count,
                    COUNT(DISTINCT id_JyoCD) as jyo_count
                FROM read_parquet('{parquetPath}')
                WHERE id_Year = '2025' AND id_MonthDay = '0705'
                GROUP BY id_MonthDay
                ORDER BY id_MonthDay
            ";

            using var summaryCommand = new DuckDBCommand(summaryQuery, connection);
            using var summaryReader = summaryCommand.ExecuteReader();

            Console.WriteLine("\n日付別集計:");
            Console.WriteLine("==================");
            Console.WriteLine("月日  レース数 開催場数");
            Console.WriteLine("------------------");

            while (summaryReader.Read())
            {
                var monthDay = summaryReader.GetString(0);
                var raceCount2 = summaryReader.GetInt64(1);
                var jyoCount = summaryReader.GetInt64(2);

                Console.WriteLine($"{monthDay}    {raceCount2,3}     {jyoCount}");
            }
        }

        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("使用方法: ReadParquet <parquetファイルパス>");
                Console.WriteLine("例: ReadParquet \"jvdb/parquet/RA/**/*.parquet\"");
                return;
            }

            try
            {
                ReadRAParquet(args[0]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"エラー: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }
    }
}