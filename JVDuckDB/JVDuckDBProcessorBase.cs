using System.Text;
using static JVData_Struct;

namespace JVDuckDB
{
    public class JVDuckDBProcessorBase
    {
        // MidB2S wrapper method
        protected static string MidB2S(ref byte[] bytes, int start, int length)
        {
            return JVData_Struct.MidB2S(ref bytes, start, length);
        }

        public Dictionary<string, object?>? ParseRecord(string line)
        {
            var bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(line);
            var recordSpec = line.Substring(0, 2);

            // 共通ヘッダーの解析
            var record = new Dictionary<string, object?>
            {
                ["record_spec"] = recordSpec,
                ["data_kubun"] = line.Substring(2, 1),
                ["head_MakeDate_Year"] = line.Substring(3, 4),
                ["head_MakeDate_Month"] = line.Substring(7, 2),
                ["head_MakeDate_Day"] = line.Substring(9, 2),
                ["raw_data"] = line // 元データも保存（デバッグ用）
            };

            // レコード種別ごとの解析
            switch (recordSpec)
            {
                case "RA": ParseRARecord(bytes, record); break;
                case "SE": ParseSERecord(bytes, record); break;
                case "UM": ParseUMRecord(bytes, record); break;
                case "KS": ParseKSRecord(bytes, record); break;
                case "CH": ParseCHRecord(bytes, record); break;
                case "HN": ParseHNRecord(bytes, record); break;
                case "SK": ParseSKRecord(bytes, record); break;
                case "JG": ParseJGRecord(bytes, record); break;
                case "H1": ParseH1Record(bytes, record); break;
                case "H6": ParseH6Record(bytes, record); break;
                case "AV": ParseAVRecord(bytes, record); break;
                case "BN": ParseBNRecord(bytes, record); break;
                case "BR": ParseBRRecord(bytes, record); break;
                case "BT": ParseBTRecord(bytes, record); break;
                case "CC": ParseCCRecord(bytes, record); break;
                case "CK": ParseCKRecord(bytes, record); break;
                case "CS": ParseCSRecord(bytes, record); break;
                case "CY": ParseCYRecord(bytes, record); break;
                case "DM": ParseDMRecord(bytes, record); break;
                case "HC": ParseHCRecord(bytes, record); break;
                case "HR": ParseHRRecord(bytes, record); break;
                case "HS": ParseHSRecord(bytes, record); break;
                case "HY": ParseHYRecord(bytes, record); break;
                case "JC": ParseJCRecord(bytes, record); break;
                case "JH": ParseJHRecord(bytes, record); break;
                case "O1": ParseO1Record(bytes, record); break;
                case "O2": ParseO2Record(bytes, record); break;
                case "O3": ParseO3Record(bytes, record); break;
                case "O4": ParseO4Record(bytes, record); break;
                case "O5": ParseO5Record(bytes, record); break;
                case "O6": ParseO6Record(bytes, record); break;
                case "OU": ParseOURecord(bytes, record); break;
                case "OV": ParseOVRecord(bytes, record); break;
                case "OW": ParseOWRecord(bytes, record); break;
                case "RC": ParseRCRecord(bytes, record); break;
                case "TC": ParseTCRecord(bytes, record); break;
                case "TK": ParseTKRecord(bytes, record); break;
                case "TM": ParseTMRecord(bytes, record); break;
                case "WC": ParseWCRecord(bytes, record); break;
                case "WE": ParseWERecord(bytes, record); break;
                case "WF": ParseWFRecord(bytes, record); break;
                case "WH": ParseWHRecord(bytes, record); break;
                case "YS": ParseYSRecord(bytes, record); break;
                default:
                    return null;
            }

            return record;
        }

        protected void ParseRARecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);

            // レース情報
            record["YoubiCD"] = MidB2S(ref bytes, 27, 1);
            record["TokuNum"] = MidB2S(ref bytes, 28, 4);
            record["Hondai"] = MidB2S(ref bytes, 32, 60).Trim();
            record["Fukudai"] = MidB2S(ref bytes, 92, 60).Trim();
            record["Kakko"] = MidB2S(ref bytes, 152, 60).Trim();
            record["HondaiEng"] = MidB2S(ref bytes, 212, 120).Trim();
            record["FukudaiEng"] = MidB2S(ref bytes, 332, 120).Trim();
            record["KakkoEng"] = MidB2S(ref bytes, 452, 120).Trim();
            record["Ryakusyo10"] = MidB2S(ref bytes, 572, 20).Trim();
            record["Ryakusyo6"] = MidB2S(ref bytes, 592, 12).Trim();
            record["Ryakusyo3"] = MidB2S(ref bytes, 604, 6).Trim();
            record["Kubun"] = MidB2S(ref bytes, 610, 1);
            record["Nkai"] = MidB2S(ref bytes, 611, 3).Trim();

            // 競走条件
            record["GradeCD"] = MidB2S(ref bytes, 614, 1);
            record["GradeCDBefore"] = MidB2S(ref bytes, 615, 1);
            record["SyubetuCD"] = MidB2S(ref bytes, 616, 2);
            record["KigoCD"] = MidB2S(ref bytes, 618, 3);
            record["JyuryoCD"] = MidB2S(ref bytes, 621, 1);
            record["JyokenCD1"] = MidB2S(ref bytes, 622, 3);
            record["JyokenCD2"] = MidB2S(ref bytes, 625, 3);
            record["JyokenCD3"] = MidB2S(ref bytes, 628, 3);
            record["JyokenCD4"] = MidB2S(ref bytes, 631, 3);
            record["JyokenCD5"] = MidB2S(ref bytes, 634, 3);
            record["JyokenName"] = MidB2S(ref bytes, 637, 60).Trim();
            record["Kyori"] = MidB2S(ref bytes, 697, 4);
            record["KyoriBefore"] = MidB2S(ref bytes, 701, 4);
            record["TrackCD"] = MidB2S(ref bytes, 705, 2);
            record["TrackCDBefore"] = MidB2S(ref bytes, 707, 2);
            record["CourseKubunCD"] = MidB2S(ref bytes, 709, 2);
            record["CourseKubunCDBefore"] = MidB2S(ref bytes, 711, 2);

            // 本賞金
            record["Honsyokin1"] = MidB2S(ref bytes, 713, 8);
            record["Honsyokin2"] = MidB2S(ref bytes, 721, 8);
            record["Honsyokin3"] = MidB2S(ref bytes, 729, 8);
            record["Honsyokin4"] = MidB2S(ref bytes, 737, 8);
            record["Honsyokin5"] = MidB2S(ref bytes, 745, 8);
            record["HonsyokinBefore1"] = MidB2S(ref bytes, 753, 8);
            record["HonsyokinBefore2"] = MidB2S(ref bytes, 761, 8);
            record["HonsyokinBefore3"] = MidB2S(ref bytes, 769, 8);
            record["HonsyokinBefore4"] = MidB2S(ref bytes, 777, 8);
            record["HonsyokinBefore5"] = MidB2S(ref bytes, 785, 8);

            // その他
            record["HassoTime"] = MidB2S(ref bytes, 793, 4);
            record["HassoTimeBefore"] = MidB2S(ref bytes, 797, 4);
            record["TorokuTosu"] = MidB2S(ref bytes, 801, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 803, 2);
            record["NyusenTosu"] = MidB2S(ref bytes, 805, 2);
            record["TenkoCD"] = MidB2S(ref bytes, 807, 1);
            record["SibaBabaCD"] = MidB2S(ref bytes, 808, 1);
            record["DirtBabaCD"] = MidB2S(ref bytes, 809, 1);

            // データ区分
            record["DataKubun"] = MidB2S(ref bytes, 823, 1);
            record["SakkujoTosu"] = MidB2S(ref bytes, 824, 2);
            record["ChuteiTosu"] = MidB2S(ref bytes, 826, 2);
        }

        protected void ParseSERecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);

            // 馬情報
            record["Wakuban"] = MidB2S(ref bytes, 27, 1);
            record["Umaban"] = MidB2S(ref bytes, 28, 2);
            record["KettoNum"] = MidB2S(ref bytes, 30, 10);
            record["Bamei"] = MidB2S(ref bytes, 40, 36).Trim();
            record["UmaKigoCD"] = MidB2S(ref bytes, 76, 2);
            record["SexCD"] = MidB2S(ref bytes, 78, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 79, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 80, 2);

            // 馬の情報
            record["Barei"] = MidB2S(ref bytes, 82, 2);
            record["TozaiCD"] = MidB2S(ref bytes, 84, 1);
            record["ChokyosiCode"] = MidB2S(ref bytes, 85, 5);
            record["ChokyosiName"] = MidB2S(ref bytes, 90, 34).Trim();
            record["BanusiCode"] = MidB2S(ref bytes, 124, 6);
            record["BanusiName"] = MidB2S(ref bytes, 130, 64).Trim();
            record["Fukusyoku"] = MidB2S(ref bytes, 194, 60).Trim();
            record["reserved1"] = MidB2S(ref bytes, 254, 60);
            record["Futan"] = MidB2S(ref bytes, 314, 3);
            record["FutanBefore"] = MidB2S(ref bytes, 317, 3);
            record["Blinker"] = MidB2S(ref bytes, 320, 1);
            record["reserved2"] = MidB2S(ref bytes, 321, 1);

            // 騎手情報
            record["KisyuCode"] = MidB2S(ref bytes, 322, 5);
            record["KisyuCodeBefore"] = MidB2S(ref bytes, 327, 5);
            record["KisyuName"] = MidB2S(ref bytes, 332, 34).Trim();
            record["KisyuNameBefore"] = MidB2S(ref bytes, 366, 34).Trim();
            record["MinaraiCD"] = MidB2S(ref bytes, 400, 1);
            record["MinaraiCDBefore"] = MidB2S(ref bytes, 401, 1);
            record["BaTaijyu"] = MidB2S(ref bytes, 402, 3);
            record["ZogenFugo"] = MidB2S(ref bytes, 405, 1);
            record["ZogenSa"] = MidB2S(ref bytes, 406, 3);

            // 成績情報
            record["IJyoCD"] = MidB2S(ref bytes, 409, 1);
            record["NyusenJyuni"] = MidB2S(ref bytes, 410, 2);
            record["KakuteiJyuni"] = MidB2S(ref bytes, 412, 2);
            record["DochakuKubun"] = MidB2S(ref bytes, 414, 1);
            record["DochakuTosu"] = MidB2S(ref bytes, 415, 1);
            record["Time"] = MidB2S(ref bytes, 416, 4);
            record["ChakusaCD"] = MidB2S(ref bytes, 420, 3);
            record["ChakusaCDP"] = MidB2S(ref bytes, 423, 3);
            record["ChakusaCDPP"] = MidB2S(ref bytes, 426, 3);
            record["Jyuni1c"] = MidB2S(ref bytes, 429, 2);
            record["Jyuni2c"] = MidB2S(ref bytes, 431, 2);
            record["Jyuni3c"] = MidB2S(ref bytes, 433, 2);
            record["Jyuni4c"] = MidB2S(ref bytes, 435, 2);
            record["Odds"] = MidB2S(ref bytes, 437, 4);
            record["Ninki"] = MidB2S(ref bytes, 441, 2);
            record["Honsyokin"] = MidB2S(ref bytes, 443, 8);
            record["Fukasyokin"] = MidB2S(ref bytes, 451, 8);
        }

        protected void ParseUMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["KettoNum"] = MidB2S(ref bytes, 11, 10);
            record["DelKubun"] = MidB2S(ref bytes, 21, 1);
            record["RegDate"] = MidB2S(ref bytes, 22, 8);
            record["DelDate"] = MidB2S(ref bytes, 30, 8);
            record["BirthDate"] = MidB2S(ref bytes, 38, 8);
            record["Bamei"] = MidB2S(ref bytes, 46, 36).Trim();
            record["BameiKana"] = MidB2S(ref bytes, 82, 40).Trim();
            record["BameiEng"] = MidB2S(ref bytes, 122, 80).Trim();
            record["ZaikyuFlag"] = MidB2S(ref bytes, 202, 1);
            record["Reserved"] = MidB2S(ref bytes, 203, 19);
            record["UmaKigoCD"] = MidB2S(ref bytes, 222, 2);
            record["SexCD"] = MidB2S(ref bytes, 224, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 225, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 226, 2);
        }

        protected void ParseKSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["KisyuCode"] = MidB2S(ref bytes, 11, 5);
            record["DelKubun"] = MidB2S(ref bytes, 16, 1);
            record["IssueDate"] = MidB2S(ref bytes, 17, 8);
            record["DelDate"] = MidB2S(ref bytes, 25, 8);
            record["BirthDate"] = MidB2S(ref bytes, 33, 8);
            record["KisyuName"] = MidB2S(ref bytes, 41, 34).Trim();
            record["KisyuNameKana"] = MidB2S(ref bytes, 75, 30).Trim();
            record["KisyuRyakusyo"] = MidB2S(ref bytes, 105, 8).Trim();
            record["KisyuNameEng"] = MidB2S(ref bytes, 113, 70).Trim();
            record["SexCD"] = MidB2S(ref bytes, 183, 1);
            record["SikakuCD"] = MidB2S(ref bytes, 184, 1);
        }

        protected void ParseCHRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            record["ChokyosiCode"] = MidB2S(ref bytes, 11, 5);
            record["DelKubun"] = MidB2S(ref bytes, 16, 1);
            record["IssueDate"] = MidB2S(ref bytes, 17, 8);
            record["DelDate"] = MidB2S(ref bytes, 25, 8);
            record["BirthDate"] = MidB2S(ref bytes, 33, 8);
            record["ChokyosiName"] = MidB2S(ref bytes, 41, 34).Trim();
            record["ChokyosiNameKana"] = MidB2S(ref bytes, 75, 30).Trim();
            record["ChokyosiRyakusyo"] = MidB2S(ref bytes, 105, 8).Trim();
            record["ChokyosiNameEng"] = MidB2S(ref bytes, 113, 70).Trim();
            record["SexCD"] = MidB2S(ref bytes, 183, 1);
            record["TozaiCD"] = MidB2S(ref bytes, 184, 1);
            record["Syotai"] = MidB2S(ref bytes, 185, 3);
        }

        protected void ParseHNRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 繁殖馬マスタ (JV_HN_HANSYOKU)
            record["HansyokuNum"] = MidB2S(ref bytes, 12, 10);
            record["reserved"] = MidB2S(ref bytes, 22, 8);
            record["KettoNum"] = MidB2S(ref bytes, 30, 10);
            record["DelKubun"] = MidB2S(ref bytes, 40, 1);
            record["Bamei"] = MidB2S(ref bytes, 41, 36).Trim();
            record["BameiKana"] = MidB2S(ref bytes, 77, 40).Trim();
            record["BameiEng"] = MidB2S(ref bytes, 117, 80).Trim();
            record["BirthYear"] = MidB2S(ref bytes, 197, 4);
            record["SexCD"] = MidB2S(ref bytes, 201, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 202, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 203, 2);
            record["HansyokuMochiKubun"] = MidB2S(ref bytes, 205, 1);
            record["ImportYear"] = MidB2S(ref bytes, 206, 4);
            record["SanchiName"] = MidB2S(ref bytes, 210, 20).Trim();
            record["HansyokuFNum"] = MidB2S(ref bytes, 230, 10);
            record["HansyokuMNum"] = MidB2S(ref bytes, 240, 10);
        }

        protected void ParseSKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 産駒マスタ (JV_SK_SANKU)
            record["KettoNum"] = MidB2S(ref bytes, 12, 10);
            record["BirthDate"] = MidB2S(ref bytes, 22, 8);
            record["SexCD"] = MidB2S(ref bytes, 30, 1);
            record["HinsyuCD"] = MidB2S(ref bytes, 31, 1);
            record["KeiroCD"] = MidB2S(ref bytes, 32, 2);
            record["SankuMochiKubun"] = MidB2S(ref bytes, 34, 1);
            record["ImportYear"] = MidB2S(ref bytes, 35, 4);
            record["BreederCode"] = MidB2S(ref bytes, 39, 8);
            record["SanchiName"] = MidB2S(ref bytes, 47, 20).Trim();

            // 3代血統 繁殖登録番号（14個）
            for (int i = 0; i < 14; i++)
            {
                record[$"HansyokuNum_{i:00}"] = MidB2S(ref bytes, 67 + (10 * i), 10);
            }
        }

        protected void ParseJGRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 除外馬（JG: JV_JG_JOGAIBA）
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            // 除外馬情報
            record["KettoNum"] = MidB2S(ref bytes, 27, 10);
            record["Bamei"] = MidB2S(ref bytes, 37, 36).Trim();
            record["ShutsubaTohyoJun"] = MidB2S(ref bytes, 73, 3);
            record["ShussoKubun"] = MidB2S(ref bytes, 76, 1);
            record["JogaiJotaiKubun"] = MidB2S(ref bytes, 77, 1);
        }

        protected void ParseH1Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 票数（全掛） (H1: JV_H1_HYOSU_ZENKAKE)
            // RACE_IDの解析
            record["id_Year"] = MidB2S(ref bytes, 11, 4);
            record["id_MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["id_JyoCD"] = MidB2S(ref bytes, 19, 2);
            record["id_Kaiji"] = MidB2S(ref bytes, 21, 2);
            record["id_Nichiji"] = MidB2S(ref bytes, 23, 2);
            record["id_RaceNum"] = MidB2S(ref bytes, 25, 2);
            
            record["TorokuTosu"] = MidB2S(ref bytes, 27, 2);
            record["SyussoTosu"] = MidB2S(ref bytes, 29, 2);
            
            // 発売フラグ
            for (int i = 0; i < 7; i++)
            {
                record[$"HatubaiFlag_{i}"] = MidB2S(ref bytes, 31 + i, 1);
            }
            
            record["FukuChakuBaraiKey"] = MidB2S(ref bytes, 38, 1);
            
            // 返還馬番情報(馬番01～28)
            for (int i = 0; i < 28; i++)
            {
                record[$"HenkanUma_{i:00}"] = MidB2S(ref bytes, 39 + i, 1);
            }
            
            // 返還枠番情報(枠番1～8)
            for (int i = 0; i < 8; i++)
            {
                record[$"HenkanWaku_{i}"] = MidB2S(ref bytes, 67 + i, 1);
            }
        }

        protected void ParseH6Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 票数（３連複） (H6: JV_H6_HYOSU_SANRENPUKU)
            // H6レコードの実装
        }

        protected void ParseAVRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // JV_AV_INFO レコードの実装
        }

        protected void ParseBNRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬主（BN: JV_BN_BANUSI）
            record["BanusiCode"] = MidB2S(ref bytes, 11, 6);
            record["BanusiName_Co"] = MidB2S(ref bytes, 17, 64).Trim();
            record["BanusiName_Kana"] = MidB2S(ref bytes, 81, 50).Trim();
            record["BanusiRyakusyo"] = MidB2S(ref bytes, 131, 12).Trim();
            record["HonUmamochiFlag"] = MidB2S(ref bytes, 143, 1);
        }

        protected void ParseBRRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 生産者（BR: JV_BR_BREEDER）
            record["BreederCode"] = MidB2S(ref bytes, 11, 8);
            record["BreederName_Co"] = MidB2S(ref bytes, 19, 70).Trim();
            record["Address"] = MidB2S(ref bytes, 89, 20).Trim();
            record["HonUmamochiFlag"] = MidB2S(ref bytes, 109, 1);
        }

        protected void ParseBTRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 調教師マスタ変更（BT: JV_BT_CHOKYOSI）
            record["ChokyosiCode"] = MidB2S(ref bytes, 11, 5);
            record["DelKubun"] = MidB2S(ref bytes, 16, 1);
            record["IssueDate"] = MidB2S(ref bytes, 17, 8);
            record["DelDate"] = MidB2S(ref bytes, 25, 8);
            record["BirthDate"] = MidB2S(ref bytes, 33, 8);
            record["ChokyosiName"] = MidB2S(ref bytes, 41, 34).Trim();
            record["ChokyosiNameKana"] = MidB2S(ref bytes, 75, 30).Trim();
            record["ChokyosiRyakusyo"] = MidB2S(ref bytes, 105, 8).Trim();
            record["ChokyosiNameEng"] = MidB2S(ref bytes, 113, 70).Trim();
            record["SexCD"] = MidB2S(ref bytes, 183, 1);
            record["TozaiCD"] = MidB2S(ref bytes, 184, 1);
            record["Syotai"] = MidB2S(ref bytes, 185, 3);
        }

        protected void ParseCCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 調教師データ（CC: JV_CC_INFO）
            record["Year"] = MidB2S(ref bytes, 11, 4);
            record["MonthDay"] = MidB2S(ref bytes, 15, 4);
            record["ChokyosiCode"] = MidB2S(ref bytes, 19, 5);
            record["KonketsuMochiba"] = MidB2S(ref bytes, 24, 1);
            record["ChokyosiInfo"] = MidB2S(ref bytes, 25, 255).Trim();
        }

        protected void ParseCKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // チャクラク（CK: JV_CK_CHAKU）レコードの実装
        }

        protected void ParseCSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 調教師データ（CS: JV_CS_CHOKYOSI）レコードの実装  
        }

        protected void ParseCYRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 騎手データ（CY: JV_CY_KISYU）レコードの実装
        }

        protected void ParseDMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // データマイニング予想（DM: JV_DM_INFO）レコードの実装
        }

        protected void ParseHCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬体重（HC: JV_HC_HANRO）レコードの実装
        }

        protected void ParseHRRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 払戻（HR: JV_HR_PAY）レコードの実装
        }

        protected void ParseHSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 発売票数（全掛け）（HS: JV_HS_SALE）レコードの実装
        }

        protected void ParseHYRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 配当（HY: JV_HY_BAMEIORIGIN）レコードの実装
        }

        protected void ParseJCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 騎手変更（JC: JV_JC_INFO）レコードの実装
        }

        protected void ParseJHRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 上位10レース（JH: JV_JH_JOHAIKYO）レコードの実装
        }

        protected void ParseO1Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 単複オッズ（O1: JV_O1_ODDS_TANFUKU）レコードの実装
        }

        protected void ParseO2Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬連オッズ（O2: JV_O2_ODDS_UMAREN）レコードの実装
        }

        protected void ParseO3Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // ワイドオッズ（O3: JV_O3_ODDS_WIDE）レコードの実装
        }

        protected void ParseO4Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬単オッズ（O4: JV_O4_ODDS_UMATAN）レコードの実装
        }

        protected void ParseO5Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 三連複オッズ（O5: JV_O5_ODDS_SANREN）レコードの実装
        }

        protected void ParseO6Record(byte[] bytes, Dictionary<string, object?> record)
        {
            // 三連単オッズ（O6: JV_O6_ODDS_SANRENTAN）レコードの実装
        }

        protected void ParseOURecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬出走情報（仮想）（OU: JV_OU_ODDS_UMADEME）レコードの実装
        }

        protected void ParseOVRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 出走馬名表（OV: JV_OV_ODDS_SYUTSUBA）レコードの実装
        }

        protected void ParseOWRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬体重（実売）（OW: JV_OW_ODDS_WIDE）レコードの実装
        }

        protected void ParseRCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // レコード（RC: JV_RC_RECORD）レコードの実装
        }

        protected void ParseTCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 特別登録馬（TC: JV_TC_INFO）レコードの実装
        }

        protected void ParseTKRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 特別登録馬（TK: JV_TK_TOKUUMA）レコードの実装
        }

        protected void ParseTMRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // タイム型データマイニング（TM: JV_TM_INFO）レコードの実装
        }

        protected void ParseWCRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // ウッドチップ調教（WC: JV_WC_WOOD）レコードの実装
        }

        protected void ParseWERecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 馬券発売詳細（WE: JV_WE_WEATHER）レコードの実装
        }

        protected void ParseWFRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // Win5（WF: JV_WF_INFO）レコードの実装
        }

        protected void ParseWHRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // Win5（WH: JV_WH_BATAIJYU）レコードの実装
        }

        protected void ParseYSRecord(byte[] bytes, Dictionary<string, object?> record)
        {
            // 予想スコア（YS: JV_YS_YSCH）レコードの実装
        }
    }
}