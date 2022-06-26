using CommandLine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;


namespace JVDownloader
{
    internal class Program
    {
        const string Sid = "UNKNOWN";

        [Verb("setup", HelpText = "設定ダイアログの表示")]
        public class SetupOptions
        {

        }

        [Verb("jv", HelpText = "蓄積系データのダウンロード")]
        public class JvOptions
        {
            [Option("dataspec", Required = true, Separator = ',', HelpText = @"dataspec. see http://jra-van.jp/dlb/sdv/sdk.html, http://jra-van.jp/dlb/sdv/sdk/JV-Data470.pdf pp.47-48

option = 1
TOKU,RACE,DIFF,BLOD,SNAP,SLOP,WOOD,YSCH,HOSE,HOYU,COMM,MING

option = 2
TOKU,RACE,TCOV,RCOV,SNAP

option = 3,4
TOKU,RACE,DIFF,BLOD,SNAP,SLOP,WOOD,YSCH,HOSE,HOYU,COMM,MING")]
            public IEnumerable<string> Dataspec { get; set; }


            [Option("fromdate", Required = false, Default = "20211101000000", HelpText = @"fromdate. YYYYMMDDhhmmss or YYYYMMDDhhmmss-YYYYMMDDhhmmss.
e.g. 20181001000000")]
            public string Fromdate { get; set; }

            [Option("option", Required = true, HelpText = "1:通常データ, 2:今週データ, 3:セットアップデータ, 4:ダイアログ無しセットアップデータ")]
            public int Option { get; set; }

            [Option("outputDir", Required = false, Default = ".", HelpText = @"output directory")]
            public string OutputDir { get; set; }

            [Option("wait", Required = false, Default = 1000, HelpText = @"処理間隔 [sec]")]
            public int Wait { get; set; }
        }


        [Verb("jvrt", HelpText = "速報系データのダウンロード")]
        public class JvrtOptions
        {
            [Option("dataspec", Required = true, Separator = ',', HelpText = @"dataspec. see http://jra-van.jp/dlb/sdv/sdk.html, http://jra-van.jp/dlb/sdv/sdk/JV-Data470.pdf pp.47-48

0B12 速報レース情報（成績確定後） 開催日単位またはレース毎
0B15 速報レース情報（出走馬名表～） 開催日単位またはレース毎
0B30 速報オッズ（全賭式） レース毎
0B31 速報オッズ（単複枠） レース毎
0B32 速報オッズ（馬連） レース毎
0B33 速報オッズ（ワイド） レース毎
0B34 速報オッズ（馬単） レース毎
0B35 速報オッズ（３連複） レース毎
0B36 速報オッズ（３連単） レース毎
0B20 速報票数（全賭式） レース毎
0B11 速報馬体重 開催日単位またはレース毎
0B14 速報開催情報（一括） 開催日単位
0B16 速報開催情報（指定） 指定された変更情報単位
0B13 速報タイム型データマイニング予想 開催日単位またはレース毎
0B17 速報対戦型データマイニング予想 開催日単位またはレース毎
0B41 時系列オッズ（単複枠） レース毎（該当レースの複数時間帯のオッズ）
0B42 時系列オッズ（馬連） レース毎（該当レースの複数時間帯のオッズ）
0B51 速報重勝式(WIN5) 重勝式開催毎")]
            public IEnumerable<string> Dataspec { get; set; }

            [Option("key", Required = true, Separator = ',', HelpText = @"該当データを取得するための要求キー
カンマ区切りで複数指定
レース毎の場合 `YYYYMMDDJJKKHHRR` または `YYYYMMDDJJRR`
開催日単位の場合 `YYYYMMDD`
YYYY:開催年, MM:開催月, DD:開催日, JJ:場コード, KK:回次, HH:日次, RR:レース番号

場コード: 01札,02函,03福,04新,05東,06中,07名,08京,09阪,10小")]
            public IEnumerable<string> Key { get; set; }

            [Option("outputDir", Required = false, Default = ".", HelpText = @"output directory")]
            public string OutputDir { get; set; }

            [Option("wait", Required = false, Default = 1000, HelpText = @"処理間隔 [sec]")]
            public int Wait { get; set; }
        }


        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<SetupOptions, JvOptions, JvrtOptions>(args)
                .WithParsed<SetupOptions>(RunSetupOptions)
                .WithParsed<JvOptions>(RunJvOptions)
                .WithParsed<JvrtOptions>(RunJvrtOptions)
                .WithNotParsed(HandleParseError);
        }

        static void RunSetupOptions(SetupOptions opts)
        {
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVSetUIProperties();
            jvLink.JVClose();
            return;
        }

        static void RunJvOptions(JvOptions opts)
        {
            if (string.IsNullOrWhiteSpace(opts.Fromdate))
            {
                throw new Exception("fromdateを指定してください");
            }
            if (opts.Option < 1 || opts.Option > 4)
            {
                throw new Exception("optionは1,2,3,4のいずれかを指定してください");
            }

            Console.Error.WriteLine($"Start to JV. Fromdate: {opts.Fromdate}, Option: {opts.Option}");
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit(Sid);
            foreach (var dataspec in opts.Dataspec)
            {
                Console.Error.WriteLine($"JV {dataspec}");
                RunJV(jvLink, dataspec, opts.Fromdate, opts.Option, opts.OutputDir, opts.Wait);
            }
        }

        static void RunJvrtOptions(JvrtOptions opts)
        {
            Console.Error.WriteLine($"Start to JVRT.");
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit(Sid);
            foreach (var dataspec in opts.Dataspec)
            {
                foreach (var key in opts.Key)
                {
                    Console.Error.WriteLine($"JVRT {dataspec} {key}");
                    RunJVRT(jvLink, dataspec, key, opts.OutputDir, opts.Wait);
                }
            }
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }


        static void RunJV(JVDTLabLib.JVLink jvLink, string dataspec, string fromdate, int option, string outputDir, int wait = 1000)
        {
            var nReadCount = 0;             // JVOpen: 総読み込みファイル数
            var nDownloadCount = 0;         // JVOpen: 総ダウンロードファイル数
            var strLastFileTimestamp = "";  // JVOpen: 最新ファイルのタイムスタンプ

            var openStatus = jvLink.JVOpen(dataspec, fromdate, option, ref nReadCount, ref nDownloadCount, out strLastFileTimestamp);

            if (openStatus != 0)
            {
                Console.Error.WriteLine($"Failed to open. status: {openStatus}");
                return;
            }

            Console.Error.WriteLine("Data spec: " + dataspec);
            Console.Error.WriteLine("Total read count: " + nReadCount.ToString());

            var outputPath = Path.Combine(outputDir, "JV-" + dataspec + "-" + fromdate + "-" + strLastFileTimestamp + ".txt");
            var streamWriter = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);

            streamWriter.WriteLine("JV DATASPEC:" + dataspec + " FROMDATE:" + fromdate + " LASTFILETIMESTAMP:" + strLastFileTimestamp);
            JVReadToTxt(jvLink, streamWriter);

            Console.WriteLine(outputPath);

            streamWriter.Close();
            jvLink.JVClose();

            Thread.Sleep(wait);
        }

        static void RunJVRT(JVDTLabLib.JVLink jvLink, string dataspec, string key, string outputDir, int wait = 1000)
        {
            var openStatus = jvLink.JVRTOpen(dataspec, key);

            if (openStatus != 0)
            {
                Console.Error.WriteLine($"Failed to open. status: {openStatus}");
                return;
            }

            var outputPath = Path.Combine(outputDir, "JVRT-" + dataspec + "-" + key + ".txt");
            var streamWriter = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);

            streamWriter.WriteLine("JVRT DATASPEC:" + dataspec + " KEY:" + key);
            JVReadToTxt(jvLink, streamWriter);

            Console.WriteLine(outputPath);

            streamWriter.Close();
            jvLink.JVClose();

            Thread.Sleep(wait);
        }

        static void JVReadToTxt(JVDTLabLib.JVLink jvLink, StreamWriter streamWriter)
        {
            var nBuffSize = 110000;                         // JVRead: データ格納バッファサイズ
            var nNameSize = 256;                            // JVRead: ファイル名サイズ
            var strBuff = new string('\0', nBuffSize);      // JVRead: データ格納バッファ
            var strFileName = new string('\0', nNameSize);  // JVRead: 読み込み中ファイル名

            var errorMessage = "";

            bool flg_exit = false;
            int currentReadCount = 0;
            do
            {
                switch (jvLink.JVRead(out strBuff, out nBuffSize, out strFileName))
                {
                    case 0: // 全ファイル読み込み終了
                        flg_exit = true;
                        break;
                    case -1: // ファイル切り替わり
                        currentReadCount++;
                        Console.Error.Write("Current read count: " + currentReadCount.ToString() + "\r");
                        break;
                    case -3: // ダウンロード中
                        break;
                    case -201: // JVInit されてない
                        errorMessage = "JVInit が行われていません。";
                        flg_exit = true;
                        break;
                    case -203: // JVOpen されてない
                        errorMessage = "JVOpen が行われていません。";
                        flg_exit = true;
                        break;
                    case -503: // ファイルがない
                        errorMessage = strFileName + "が存在しません。";
                        flg_exit = true;
                        break;
                    case int ret when ret > 0:
                        streamWriter.Write(strBuff);
                        break;
                }
            }
            while (!flg_exit);

            if (errorMessage != "")
            {
                Console.Error.WriteLine(errorMessage);
            }

            Console.Error.WriteLine($"Finished JVReadToTxt.");
        }
    }
}
