using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static JVData_Struct;


namespace JVConsole
{
    internal class Program
    {
        [Verb("setup")]
        public class SetupOptions
        {

        }

        [Verb("jv")]
        public class JvOptions
        {
            [Option("output", Default = "json", HelpText = "Specify output format. `json` or `txt`")]
            public string Output { get; set; }


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
        }


        [Verb("jvrt")]
        public class JvrtOptions
        {
            [Option("output", Default = "json", HelpText = "Specify output format. `json` or `txt`")]
            public string Output { get; set; }

            [Option("dataspec", Required = true, HelpText = @"dataspec. see http://jra-van.jp/dlb/sdv/sdk.html, http://jra-van.jp/dlb/sdv/sdk/JV-Data470.pdf pp.47-48

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
            public string Dataspec { get; set; }

            [Option("key", Required = true, HelpText = @"該当データを取得するための要求キー
レース毎の場合`YYYYMMDDJJKKHHRR` または `YYYYMMDDJJRR`
開催日単位の場合 `YYYYMMDD`
YYYY:開催年, MM:開催月, DD:開催日, JJ:場コード, KK:回次, HH:日次, RR:レース番号")]
            public string Key { get; set; }
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
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit("UNKNOWN");

            if (string.IsNullOrWhiteSpace(opts.Fromdate))
            {
                throw new Exception("fromdateを指定してください");
            }
            if (opts.Option < 1 || opts.Option > 4)
            {
                throw new Exception("optionは1,2,3,4のいずれかを指定してください");
            }

            foreach (var dataspec in opts.Dataspec)
            {
                JVOpen(jvLink, dataspec, opts.Fromdate, opts.Option, opts.Output);
            }
        }

        static void RunJvrtOptions(JvrtOptions opts)
        {
            var jvLink = new JVDTLabLib.JVLink();
            jvLink.JVInit("UNKNOWN");

            if (string.IsNullOrWhiteSpace(opts.Dataspec))
            {
                throw new Exception("dataspecを指定してください");
            }
            if (string.IsNullOrWhiteSpace(opts.Key))
            {
                throw new Exception("keyを指定してください");
            }
            JVRTOpen(jvLink, opts.Dataspec, opts.Key);
            if (opts.Output == "raw")
            {
                JVReadToTxt(jvLink);
            }
            else
            {
                JVReadToJson(jvLink);
            }
            jvLink.JVClose();
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }

        class OpenSpec
        {
            public string OpenType { get; set; }
            public string DataSpec { get; set; }
            public string FromDate { get; set; }
            public int Option { get; set; }
            public string Key { get; set; }
            public int ReadCount { get; set; }
            public int DownloadCount { get; set; }
            public string LastFileTimesatmp { get; set; }
        }

        static void JVOpen(JVDTLabLib.JVLink jvLink, string dataspec, string fromdate, int option, string output)
        {

            var nReadCount = 0;             // JVOpen: 総読み込みファイル数
            var nDownloadCount = 0;         // JVOpen: 総ダウンロードファイル数
            var strLastFileTimestamp = "";  // JVOpen: 最新ファイルのタイムスタンプ
            jvLink.JVOpen(dataspec, fromdate, option, ref nReadCount, ref nDownloadCount, out strLastFileTimestamp);

            var openspec = new OpenSpec() { OpenType = "JVOpen", DataSpec = dataspec, FromDate = fromdate, Option = option, ReadCount = nReadCount, DownloadCount = nDownloadCount, LastFileTimesatmp = strLastFileTimestamp };
            Console.WriteLine(
                JsonConvert.SerializeObject(openspec)
            );

            if (output == "txt")
            {
                JVReadToTxt(jvLink);
            }
            else
            {
                JVReadToJson(jvLink);
            }
            jvLink.JVClose();
        }

        static void JVRTOpen(JVDTLabLib.JVLink jvLink, string dataspec, string key)
        {
            jvLink.JVRTOpen(dataspec, key);
            var openspec = new OpenSpec() { OpenType = "JVRTOpen", DataSpec = dataspec, Key = key };
            Console.WriteLine(
                JsonConvert.SerializeObject(openspec)
            );
        }

        static void JVReadToTxt(JVDTLabLib.JVLink jvLink)
        {
            var nBuffSize = 110000;                         // JVRead: データ格納バッファサイズ
            var nNameSize = 256;                            // JVRead: ファイル名サイズ
            var strBuff = new string('\0', nBuffSize);      // JVRead: データ格納バッファ
            var strFileName = new string('\0', nNameSize);  // JVRead: 読み込み中ファイル名

            var errorMessage = "";

            bool flg_exit = false;
            do
            {
                switch (jvLink.JVRead(out strBuff, out nBuffSize, out strFileName))
                {
                    case 0: // 全ファイル読み込み終了
                        flg_exit = true;
                        break;
                    case -1: // ファイル切り替わり
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
                        Console.WriteLine(strBuff);
                        break;
                }
            }
            while (!flg_exit);

            if (errorMessage != "")
            {
                throw new Exception(errorMessage);
            }
        }

        static void JVReadToJson(JVDTLabLib.JVLink jvLink)
        {
            var nBuffSize = 110000;                         // JVRead: データ格納バッファサイズ
            var nNameSize = 256;                            // JVRead: ファイル名サイズ
            var strBuff = new string('\0', nBuffSize);      // JVRead: データ格納バッファ
            var strFileName = new string('\0', nNameSize);  // JVRead: 読み込み中ファイル名

            var av = new JV_AV_INFO();
            var bn = new JV_BN_BANUSI();
            var br = new JV_BR_BREEDER();
            var bt = new JV_BT_KEITO();
            var cc = new JV_CC_INFO();
            var ch = new JV_CH_CHOKYOSI();
            var ck = new JV_CK_CHAKU();
            var cs = new JV_CS_COURSE();
            var dm = new JV_DM_INFO();
            var h1 = new JV_H1_HYOSU_ZENKAKE();
            var h6 = new JV_H6_HYOSU_SANRENTAN();
            var hc = new JV_HC_HANRO();
            var hn = new JV_HN_HANSYOKU();
            var hr = new JV_HR_PAY();
            var hs = new JV_HS_SALE();
            var hy = new JV_HY_BAMEIORIGIN();
            var jc = new JV_JC_INFO();
            var jg = new JV_JG_JOGAIBA();
            var o1 = new JV_O1_ODDS_TANFUKUWAKU();
            var o2 = new JV_O2_ODDS_UMAREN();
            var o3 = new JV_O3_ODDS_WIDE();
            var o4 = new JV_O4_ODDS_UMATAN();
            var o5 = new JV_O5_ODDS_SANREN();
            var o6 = new JV_O6_ODDS_SANRENTAN();
            var ra = new JV_RA_RACE();
            var rc = new JV_RC_RECORD();
            var se = new JV_SE_RACE_UMA();
            var sk = new JV_SK_SANKU();
            var tc = new JV_TC_INFO();
            var tk = new JV_TK_TOKUUMA();
            var tm = new JV_TM_INFO();
            var um = new JV_UM_UMA();
            // var wc = new(); JV_WC
            var we = new JV_WE_WEATHER();
            var wf = new JV_WF_INFO();
            var wh = new JV_WH_BATAIJYU();
            var ys = new JV_YS_SCHEDULE();


            var jsonString = "";
            var errorMessage = "";


            bool flg_exit = false;
            do
            {
                switch (jvLink.JVRead(out strBuff, out nBuffSize, out strFileName))
                {
                    case 0: // 全ファイル読み込み終了
                        flg_exit = true;
                        break;
                    case -1: // ファイル切り替わり
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
                        var dataKubun = strBuff.Substring(0, 2);
                        switch (dataKubun)
                        {
                            case "AV":
                                av.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(av);
                                break;
                            case "BN":
                                bn.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(bn);
                                break;
                            case "BR":
                                br.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(br);
                                break;
                            case "BT":
                                bt.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(bt);
                                break;
                            case "CC":
                                cc.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(cc);
                                break;
                            case "CH":
                                ch.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(ch);
                                break;
                            case "CK":
                                ck.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(ck);
                                break;
                            case "CS":
                                cs.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(cs);
                                break;
                            case "DM":
                                dm.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(dm);
                                break;
                            case "H1":
                                h1.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(h1);
                                break;
                            case "H6":
                                h6.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(h6);
                                break;
                            case "HC":
                                hc.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(hc);
                                break;
                            case "HN":
                                hn.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(hn);
                                break;
                            case "HR":
                                hr.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(hr);
                                break;
                            case "HS":
                                hs.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(hs);
                                break;
                            case "HY":
                                hy.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(hy);
                                break;
                            case "JC":
                                jc.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(jc);
                                break;
                            case "JG":
                                jg.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(jg);
                                break;
                            case "O1":
                                o1.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o1);
                                break;
                            case "O2":
                                o2.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o2);
                                break;
                            case "O3":
                                o3.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o3);
                                break;
                            case "O4":
                                o4.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o4);
                                break;
                            case "O5":
                                o5.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o5);
                                break;
                            case "O6":
                                o6.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(o6);
                                break;
                            case "RA":
                                ra.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(ra);
                                break;
                            case "RC":
                                rc.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(rc);
                                break;
                            case "SE":
                                se.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(se);
                                break;
                            case "SK":
                                sk.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(sk);
                                break;
                            case "TC":
                                tc.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(tc);
                                break;
                            case "TK":
                                tk.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(tk);
                                break;
                            case "TM":
                                tm.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(tm);
                                break;
                            case "UM":
                                um.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(um);
                                break;
                            case "WE":
                                we.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(we);
                                break;
                            case "WF":
                                wf.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(wf);
                                break;
                            case "WH":
                                wh.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(wh);
                                break;
                            case "YS":
                                ys.SetDataB(ref strBuff);
                                jsonString = JsonConvert.SerializeObject(ys);
                                break;
                            default:
                                // 読み飛ばし
                                jvLink.JVSkip();
                                break;
                        }
                        // 終端記号の削除
                        jsonString = jsonString.Replace(",\"crlf\":\"\\r\\n\"", "");
                        Console.WriteLine(jsonString);
                        break;
                }
            }
            while (!flg_exit);

            if (errorMessage != "")
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
