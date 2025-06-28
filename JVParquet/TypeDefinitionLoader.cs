using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JVParquet
{
    /// <summary>
    /// Python型定義からC#の型マッピングをロードするクラス
    /// </summary>
    public class TypeDefinitionLoader
    {
        private readonly Dictionary<string, Dictionary<string, Type>> _typeDefinitions;
        
        public TypeDefinitionLoader()
        {
            _typeDefinitions = new Dictionary<string, Dictionary<string, Type>>();
        }

        /// <summary>
        /// ハードコードされた型定義（Phase 2の簡易実装）
        /// </summary>
        public void LoadHardcodedDefinitions()
        {
            // 数値型の定義例
            var numericPatterns = new Dictionary<string, Type>
            {
                // 整数型
                { "*Tosu", typeof(int) },        // 頭数
                { "*Umaban", typeof(int) },      // 馬番
                { "*Waku", typeof(int) },        // 枠番
                { "*Ninki", typeof(int) },       // 人気
                { "*Chakujun", typeof(int) },    // 着順
                { "*Year", typeof(int) },        // 年
                { "*Month", typeof(int) },       // 月
                { "*Day", typeof(int) },         // 日
                { "*Kaiji", typeof(int) },       // 回次
                { "*Nichiji", typeof(int) },     // 日次
                { "*RaceNum", typeof(int) },     // レース番号
                
                // 小数型
                { "*Odds", typeof(decimal) },    // オッズ
                { "*Time", typeof(decimal) },    // タイム
                { "*Haito", typeof(decimal) },   // 配当
                { "*Kinryo", typeof(decimal) },  // 斤量
                { "*Bataijyu", typeof(decimal) },// 馬体重
                
                // 長整数型
                { "*Pay", typeof(long) },        // 払戻金
                { "*Honsyokin", typeof(long) },  // 本賞金
            };

            // RAレコード用の型定義（完全版）
            var raTypes = new Dictionary<string, Type>
            {
                // ヘッダー情報
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                
                // レースID
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                
                // レース情報
                { "RaceInfo_YoubiCD", typeof(string) },
                { "RaceInfo_TokuNum", typeof(int) },
                { "RaceInfo_Hondai", typeof(string) },
                { "RaceInfo_Fukudai", typeof(string) },
                { "RaceInfo_Kakko", typeof(string) },
                { "RaceInfo_HondaiEng", typeof(string) },
                { "RaceInfo_FukudaiEng", typeof(string) },
                { "RaceInfo_KakkoEng", typeof(string) },
                { "RaceInfo_Ryakusyo10", typeof(string) },
                { "RaceInfo_Ryakusyo6", typeof(string) },
                { "RaceInfo_Ryakusyo3", typeof(string) },
                { "RaceInfo_Kubun", typeof(string) },
                { "RaceInfo_Nkai", typeof(string) },
                
                // グレードと条件
                { "GradeCD", typeof(string) },
                { "GradeCDBefore", typeof(string) },
                { "JyokenInfo_SyubetuCD", typeof(string) },
                { "JyokenInfo_KigoCD", typeof(string) },
                { "JyokenInfo_JyuryoCD", typeof(string) },
                { "JyokenInfo_JyokenCD_0", typeof(string) },
                { "JyokenInfo_JyokenCD_1", typeof(string) },
                { "JyokenInfo_JyokenCD_2", typeof(string) },
                { "JyokenInfo_JyokenCD_3", typeof(string) },
                { "JyokenInfo_JyokenCD_4", typeof(string) },
                { "JyokenName", typeof(string) },
                
                // 距離とコース
                { "Kyori", typeof(string) },
                { "KyoriBefore", typeof(string) },
                { "TrackCD", typeof(string) },
                { "TrackCDBefore", typeof(string) },
                { "CourseKubunCD", typeof(string) },
                { "CourseKubunCDBefore", typeof(string) },
                
                // 頭数と時刻
                { "SyussoTosu", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "NyusenTosu", typeof(int) },
                { "HassoTime", typeof(string) },
                { "HassoTimeBefore", typeof(string) },
                { "TenkoBaba_TenkoCD", typeof(string) },
                { "TenkoBaba_SibaBabaCD", typeof(string) },
                { "TenkoBaba_DirtBabaCD", typeof(string) },
                
                // タイム情報
                { "SyogaiMileTime", typeof(string) },
                { "HaronTimeS3", typeof(string) },
                { "HaronTimeS4", typeof(string) },
                { "HaronTimeL3", typeof(string) },
                { "HaronTimeL4", typeof(string) },
                
                // 賞金
                { "Honsyokin_0", typeof(long) },
                { "Honsyokin_1", typeof(long) },
                { "Honsyokin_2", typeof(long) },
                { "Honsyokin_3", typeof(long) },
                { "Honsyokin_4", typeof(long) },
                { "Honsyokin_5", typeof(long) },
                { "Honsyokin_6", typeof(long) },
                { "HonsyokinBefore_0", typeof(long) },
                { "HonsyokinBefore_1", typeof(long) },
                { "HonsyokinBefore_2", typeof(long) },
                { "HonsyokinBefore_3", typeof(long) },
                { "HonsyokinBefore_4", typeof(long) },
                
                // 付加賞金
                { "Fukasyokin_0", typeof(long) },
                { "Fukasyokin_1", typeof(long) },
                { "Fukasyokin_2", typeof(long) },
                { "Fukasyokin_3", typeof(long) },
                { "Fukasyokin_4", typeof(long) },
                { "FukasyokinBefore_0", typeof(long) },
                { "FukasyokinBefore_1", typeof(long) },
                { "FukasyokinBefore_2", typeof(long) },
                
                // その他
                { "HenkoCD", typeof(string) },
                { "RecordUpKubun", typeof(string) },
            };
            
            // ラップタイム配列
            for (int i = 0; i < 25; i++)
            {
                raTypes[$"LapTime_{i}"] = typeof(string);
            }
            
            // コーナー通過順位
            for (int i = 0; i < 4; i++)
            {
                raTypes[$"CornerInfo_{i}__Corner"] = typeof(string);
                raTypes[$"CornerInfo_{i}__Syukaisu"] = typeof(string);
                raTypes[$"CornerInfo_{i}__Jyuni"] = typeof(string);
            }

            _typeDefinitions["RA"] = raTypes;

            // SEレコード用の型定義（完全版）
            var seTypes = new Dictionary<string, Type>
            {
                // ヘッダー情報
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                
                // レースID
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                
                // 馬情報
                { "Umaban", typeof(int) },
                { "Wakuban", typeof(int) },
                { "KettoNum", typeof(string) },
                { "Bamei", typeof(string) },
                { "BameiKana", typeof(string) },
                { "UmaKigoCD", typeof(string) },
                { "SexCD", typeof(string) },
                { "HinsyuCD", typeof(string) },
                { "KeiroCD", typeof(string) },
                { "Barei", typeof(string) },
                { "TozaiCD", typeof(string) },
                { "ChokyosiCode", typeof(string) },
                { "ChokyosiRyakusyo", typeof(string) },
                { "BanusiCode", typeof(string) },
                { "BanusiName", typeof(string) },
                { "Fukusyoku", typeof(string) },
                { "reserved1", typeof(string) },
                { "reserved2", typeof(string) },
                
                // 斤量と騎手
                { "Futan", typeof(string) },
                { "FutanBefore", typeof(string) },
                { "Blinker", typeof(string) },
                { "KisyuCode", typeof(string) },
                { "KisyuCodeBefore", typeof(string) },
                { "KisyuRyakusyo", typeof(string) },
                { "KisyuRyakusyoBefore", typeof(string) },
                { "MinaraiCD", typeof(string) },
                { "MinaraiCDBefore", typeof(string) },
                { "BaTaijyu", typeof(string) },
                { "ZogenFugo", typeof(string) },
                { "ZogenSa", typeof(string) },
                
                // 着順と成績
                { "IJyoCD", typeof(string) },
                { "NyusenJyuni", typeof(int) },
                { "KakuteiJyuni", typeof(int) },
                { "DochakuKubun", typeof(string) },
                { "DochakuTosu", typeof(int) },
                { "Time", typeof(string) },
                { "ChakusaCD", typeof(string) },
                { "ChakusaCDP", typeof(string) },
                { "ChakusaCDPP", typeof(string) },
                { "Jyuni1c", typeof(int) },
                { "Jyuni2c", typeof(int) },
                { "Jyuni3c", typeof(int) },
                { "Jyuni4c", typeof(int) },
                { "Odds", typeof(decimal) },
                { "Ninki", typeof(int) },
                { "Honsyokin", typeof(long) },
                { "Fukasyokin", typeof(long) },
                { "reserved3", typeof(string) },
                { "reserved4", typeof(string) },
                { "HaronTimeL4", typeof(string) },
                { "HaronTimeL3", typeof(string) },
                
                // 着馬情報
                { "ChakuUmaInfo_0__KettoNum", typeof(string) },
                { "ChakuUmaInfo_0__Bamei", typeof(string) },
                { "ChakuUmaInfo_1__KettoNum", typeof(string) },
                { "ChakuUmaInfo_1__Bamei", typeof(string) },
                { "ChakuUmaInfo_2__KettoNum", typeof(string) },
                { "ChakuUmaInfo_2__Bamei", typeof(string) },
                
                // その他
                { "TimeDiff", typeof(string) },
                { "RecordUpKubun", typeof(string) },
                { "DMKubun", typeof(string) },
                { "DMTime", typeof(string) },
                { "DMGosaP", typeof(string) },
                { "DMGosaM", typeof(string) },
                { "DMJyuni", typeof(int) },
                { "KyakusituKubun", typeof(string) },
            };

            _typeDefinitions["SE"] = seTypes;

            // CKレコード用の型定義
            var ckTypes = new Dictionary<string, Type>
            {
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "UmaChaku__KettoNum", typeof(string) },
                { "UmaChaku__Bamei", typeof(string) },
                { "UmaChaku__RuikeiHonsyoHeiti", typeof(long) },
                { "UmaChaku__RuikeiHonsyoSyogai", typeof(long) },
                { "UmaChaku__RuikeiFukaHeichi", typeof(long) },
                { "UmaChaku__RuikeiFukaSyogai", typeof(long) },
                { "UmaChaku__RuikeiSyutokuHeichi", typeof(long) },
                { "UmaChaku__RuikeiSyutokuSyogai", typeof(long) },
            };

            // 着回数の配列フィールドを追加
            for (int i = 0; i < 6; i++)
            {
                ckTypes[$"UmaChaku__ChakuSogo__ChakuKaisu_{i}"] = typeof(int);
                ckTypes[$"UmaChaku__ChakuChuo__ChakuKaisu_{i}"] = typeof(int);
                
                // 各競馬場の着回数
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        ckTypes[$"UmaChaku__ChakuKaisuBa_{j}__ChakuKaisu_{k}"] = typeof(int);
                        ckTypes[$"UmaChaku__ChakuKaisuJyotai_{j}__ChakuKaisu_{k}"] = typeof(int);
                    }
                }
            }

            _typeDefinitions["CK"] = ckTypes;

            // O1 (単複枠オッズ) レコード用の型定義
            var o1Types = new Dictionary<string, Type>
            {
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "HappyoTime_Hour", typeof(int) },
                { "HappyoTime_Minute", typeof(int) },
            };

            // オッズ配列を追加
            for (int i = 0; i < 28; i++)
            {
                o1Types[$"OddsTansyoInfo_{i}__Umaban"] = typeof(int);
                o1Types[$"OddsTansyoInfo_{i}__Odds"] = typeof(decimal);
                o1Types[$"OddsTansyoInfo_{i}__Ninki"] = typeof(int);
                o1Types[$"OddsFukusyoInfo_{i}__Umaban"] = typeof(int);
                o1Types[$"OddsFukusyoInfo_{i}__OddsLow"] = typeof(decimal);
                o1Types[$"OddsFukusyoInfo_{i}__OddsHigh"] = typeof(decimal);
                o1Types[$"OddsFukusyoInfo_{i}__Ninki"] = typeof(int);
            }

            _typeDefinitions["O1"] = o1Types;

            // HR (払戻) レコード用の型定義  
            var hrTypes = new Dictionary<string, Type>
            {
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
            };

            // 払戻情報の配列を追加
            string[] payTypes = { "PayTansyo", "PayFukusyo", "PayWakuren", "PayUmaren", 
                                  "PayWide", "PayUmatan", "PaySanrenpuku", "PaySanrentan" };
            foreach (var payType in payTypes)
            {
                for (int i = 0; i < 3; i++)
                {
                    hrTypes[$"{payType}_{i}__Umaban"] = typeof(string);
                    hrTypes[$"{payType}_{i}__Pay"] = typeof(long);
                    hrTypes[$"{payType}_{i}__Ninki"] = typeof(int);
                }
            }

            _typeDefinitions["HR"] = hrTypes;

            // BTレコード（系統情報）
            var btTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "HansyokuNum", typeof(string) },
                { "KeitoId", typeof(string) },
                { "KeitoName", typeof(string) },
                { "KeitoEx", typeof(string) },
            };
            _typeDefinitions["BT"] = btTypes;

            // WCレコード（ウッドチップ調教）
            var wcTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "TresenKubun", typeof(string) },
                { "ChokyoDate", typeof(string) },
                { "ChokyoTime", typeof(string) },
                { "Baba", typeof(string) },
                { "BabaStatus", typeof(string) },
                { "Jyuryo", typeof(string) },
                { "HanroCD", typeof(string) },
                { "BaneiRaceKubun", typeof(string) },
            };
            
            // WCの調教情報配列
            for (int i = 0; i < 5; i++)
            {
                wcTypes[$"WCData_{i}__KettoNum"] = typeof(string);
                wcTypes[$"WCData_{i}__F4Time"] = typeof(int);
                wcTypes[$"WCData_{i}__HaronTime"] = typeof(int);
                wcTypes[$"WCData_{i}__LapTime_0"] = typeof(int);
                wcTypes[$"WCData_{i}__LapTime_1"] = typeof(int);
                wcTypes[$"WCData_{i}__LapTime_2"] = typeof(int);
                wcTypes[$"WCData_{i}__LapTime_3"] = typeof(int);
                wcTypes[$"WCData_{i}__TimeCB"] = typeof(string);
                wcTypes[$"WCData_{i}__Ayumi"] = typeof(string);
                wcTypes[$"WCData_{i}__Ichi"] = typeof(string);
                wcTypes[$"WCData_{i}__ChokyoType"] = typeof(string);
                wcTypes[$"WCData_{i}__Noriten"] = typeof(string);
                wcTypes[$"WCData_{i}__KisyuCode"] = typeof(string);
                wcTypes[$"WCData_{i}__ChokyoF"] = typeof(string);
                wcTypes[$"WCData_{i}__YobiLapTime_0"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_1"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_2"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_3"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_4"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_5"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_6"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_7"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_8"] = typeof(int);
                wcTypes[$"WCData_{i}__YobiLapTime_9"] = typeof(int);
            }
            
            // 追加の定義
            wcTypes["BabaAround"] = typeof(string);
            wcTypes["ChokyoDate_Day"] = typeof(int);
            wcTypes["ChokyoDate_Month"] = typeof(int);
            wcTypes["ChokyoDate_Year"] = typeof(int);
            wcTypes["Course"] = typeof(string);
            wcTypes["HaronTime10"] = typeof(decimal);
            wcTypes["HaronTime2"] = typeof(decimal);
            wcTypes["HaronTime3"] = typeof(decimal);
            wcTypes["HaronTime4"] = typeof(decimal);
            wcTypes["HaronTime5"] = typeof(decimal);
            wcTypes["HaronTime6"] = typeof(decimal);
            wcTypes["HaronTime7"] = typeof(decimal);
            wcTypes["HaronTime8"] = typeof(decimal);
            wcTypes["HaronTime9"] = typeof(decimal);
            wcTypes["KettoNum"] = typeof(string);
            wcTypes["LapTime1"] = typeof(decimal);
            wcTypes["LapTime10"] = typeof(decimal);
            wcTypes["LapTime2"] = typeof(decimal);
            wcTypes["LapTime3"] = typeof(decimal);
            wcTypes["LapTime4"] = typeof(decimal);
            wcTypes["LapTime5"] = typeof(decimal);
            wcTypes["LapTime6"] = typeof(decimal);
            wcTypes["LapTime7"] = typeof(decimal);
            wcTypes["LapTime8"] = typeof(decimal);
            wcTypes["LapTime9"] = typeof(decimal);
            wcTypes["reserved"] = typeof(string);
            
            _typeDefinitions["WC"] = wcTypes;

            // HNレコード（繁殖馬）
            var hnTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "HansyokuNum", typeof(string) },
                { "reserved", typeof(string) },
                { "KettoNum", typeof(string) },
                { "DelKubun", typeof(string) },
                { "BirthDate_Year", typeof(int) },
                { "BirthDate_Month", typeof(int) },
                { "BirthDate_Day", typeof(int) },
                { "SexCD", typeof(string) },
                { "HansyokuMochiKubun", typeof(string) },
                { "ImportYear", typeof(string) },
                { "SanchiName", typeof(string) },
                { "Tozai", typeof(string) },
                { "FNum", typeof(string) },
                { "MNum", typeof(string) },
                { "HansyokuNum2", typeof(string) },
                { "HansyokuNum3", typeof(string) },
                { "HansyokuNum4", typeof(string) },
                { "HansyokuNum5", typeof(string) },
                { "HansyokuNum6", typeof(string) },
                { "HansyokuNum7", typeof(string) },
                { "HansyokuNum8", typeof(string) },
                { "HansyokuNum9", typeof(string) },
                { "HansyokuNum10", typeof(string) },
                { "HansyokuNum11", typeof(string) },
                { "HansyokuNum12", typeof(string) },
                { "HansyokuNum13", typeof(string) },
                { "HansyokuNum14", typeof(string) },
                { "Bamei", typeof(string) },
                { "BameiEng", typeof(string) },
                { "BameiKana", typeof(string) },
                { "BirthYear", typeof(string) },
                { "HansyokuFNum", typeof(string) },
                { "HansyokuMNum", typeof(string) },
                { "HinsyuCD", typeof(string) },
                { "KeiroCD", typeof(string) },
            };
            _typeDefinitions["HN"] = hnTypes;

            // JGレコード（騎手騎乗馬）
            var jgTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "KisyuCode", typeof(string) },
                { "SiboriKubun", typeof(string) },
                { "JyogaiJyoutai_Kyuuyo", typeof(string) },
                { "JyogaiJyoutai_ShuppanSeigen", typeof(string) },
                { "JyogaiJyoutai_Henkou", typeof(string) },
                { "JyogaiJyoutai_Heni", typeof(string) },
                { "JyogaiJyoutai_Joumae", typeof(string) },
                { "JyogaiJyoutai_Jyogai", typeof(string) },
            };
            
            // 騎乗馬情報配列
            for (int i = 0; i < 12; i++)
            {
                jgTypes[$"JyogaiInfo_{i}__Kinou"] = typeof(string);
                jgTypes[$"JyogaiInfo_{i}__Umaban"] = typeof(int);
            }
            
            // 追加の定義
            jgTypes["Bamei"] = typeof(string);
            jgTypes["JogaiJotaiKubun"] = typeof(string);
            jgTypes["KettoNum"] = typeof(string);
            jgTypes["ShussoKubun"] = typeof(string);
            jgTypes["ShutsubaTohyoJun"] = typeof(string);
            
            _typeDefinitions["JG"] = jgTypes;

            // HCレコード（ハンデキャップ）
            var hcTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "HappyoTime_Hour", typeof(int) },
                { "HappyoTime_Minute", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "ChokyoDate_Year", typeof(int) },
                { "ChokyoDate_Month", typeof(int) },
                { "ChokyoDate_Day", typeof(int) },
                { "ChokyoTime", typeof(decimal) },
                { "HaronTime2", typeof(decimal) },
                { "HaronTime3", typeof(decimal) },
                { "HaronTime4", typeof(decimal) },
                { "KettoNum", typeof(string) },
                { "LapTime1", typeof(decimal) },
                { "LapTime2", typeof(decimal) },
                { "LapTime3", typeof(decimal) },
                { "LapTime4", typeof(decimal) },
                { "TresenKubun", typeof(string) },
            };
            
            // ハンデ情報配列
            for (int i = 0; i < 18; i++)
            {
                hcTypes[$"HCInfoData_{i}__KettoNum"] = typeof(string);
                hcTypes[$"HCInfoData_{i}__Hando"] = typeof(decimal);
            }
            _typeDefinitions["HC"] = hcTypes;

            // SKレコード（先行情報）
            var skTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "HappyoTime_Hour", typeof(int) },
                { "HappyoTime_Minute", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "KakuteiTosu", typeof(int) },
            };
            
            // 先行情報配列
            for (int i = 0; i < 18; i++)
            {
                skTypes[$"SKData_{i}__Umaban"] = typeof(int);
                skTypes[$"SKData_{i}__KettoNum"] = typeof(string);
                skTypes[$"SKData_{i}__SexCD"] = typeof(string);
                skTypes[$"SKData_{i}__HinsyuCD"] = typeof(string);
                skTypes[$"SKData_{i}__KeiroCD"] = typeof(string);
                skTypes[$"SKData_{i}__Barei"] = typeof(string);
                skTypes[$"SKData_{i}__Tozai"] = typeof(string);
                skTypes[$"SKData_{i}__ChokyosiCode"] = typeof(string);
                skTypes[$"SKData_{i}__BanusiCode"] = typeof(string);
                skTypes[$"SKData_{i}__Kinryo"] = typeof(decimal);
                skTypes[$"SKData_{i}__KinryoDai"] = typeof(decimal);
                skTypes[$"SKData_{i}__KisyuCode"] = typeof(string);
                skTypes[$"SKData_{i}__MinaraiCD"] = typeof(string);
            }
            
            // 追加の定義
            skTypes["BirthDate_Day"] = typeof(int);
            skTypes["BirthDate_Month"] = typeof(int);
            skTypes["BirthDate_Year"] = typeof(int);
            skTypes["BreederCode"] = typeof(string);
            skTypes["HinsyuCD"] = typeof(string);
            skTypes["ImportYear"] = typeof(string);
            skTypes["KeiroCD"] = typeof(string);
            skTypes["KettoNum"] = typeof(string);
            skTypes["SanchiName"] = typeof(string);
            skTypes["SankuMochiKubun"] = typeof(string);
            skTypes["SexCD"] = typeof(string);
            
            // HansyokuNum配列（0-13）
            for (int i = 0; i <= 13; i++)
            {
                skTypes[$"HansyokuNum_{i}"] = typeof(string);
            }
            
            _typeDefinitions["SK"] = skTypes;

            // WFレコード（Win5）  
            var wfTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
            };
            
            // 各レースの情報
            for (int i = 0; i < 5; i++)
            {
                wfTypes[$"id_{i}__Year"] = typeof(int);
                wfTypes[$"id_{i}__MonthDay"] = typeof(int);
                wfTypes[$"id_{i}__JyoCD"] = typeof(string);
                wfTypes[$"id_{i}__Kaiji"] = typeof(int);
                wfTypes[$"id_{i}__Nichiji"] = typeof(int);
                wfTypes[$"id_{i}__RaceNum"] = typeof(int);
                wfTypes[$"YoubiCD_{i}"] = typeof(string);
                wfTypes[$"SyussoTosu_{i}"] = typeof(int);
                
                // WFRaceInfo配列
                wfTypes[$"WFRaceInfo_{i}__JyoCD"] = typeof(string);
                wfTypes[$"WFRaceInfo_{i}__Kaiji"] = typeof(int);
                wfTypes[$"WFRaceInfo_{i}__Nichiji"] = typeof(int);
                wfTypes[$"WFRaceInfo_{i}__RaceNum"] = typeof(int);
                
                // WFYukoHyoInfo配列
                wfTypes[$"WFYukoHyoInfo_{i}__Yuko_Hyo"] = typeof(string);
            }
            
            wfTypes["HatubaiFlag"] = typeof(string);
            wfTypes["HenkanFlag"] = typeof(string);
            wfTypes["TekichunashiFlag"] = typeof(string);
            wfTypes["FuseiritsuFlag"] = typeof(string);
            wfTypes["reserved1"] = typeof(string);
            wfTypes["reserved2"] = typeof(string);
            wfTypes["KaisaiDate_Year"] = typeof(int);
            wfTypes["KaisaiDate_Month"] = typeof(int);
            wfTypes["KaisaiDate_Day"] = typeof(int);
            wfTypes["Hatsubai_Hyo"] = typeof(string);
            wfTypes["COShoki"] = typeof(string);
            wfTypes["COZanDaka"] = typeof(string);
            
            // Win5情報配列（膨大な数）
            for (int i = 0; i < 200; i++) // 実際はもっと多いが代表的なものだけ
            {
                wfTypes[$"WFInfo_{i}__Kumiban"] = typeof(string);
                wfTypes[$"WFInfo_{i}__Odds"] = typeof(decimal);
                wfTypes[$"WFInfo_{i}__Ninki"] = typeof(int);
            }
            
            // Win5払戻情報配列
            for (int i = 0; i < 243; i++) // 3^5 = 243通り
            {
                wfTypes[$"WFPayInfo_{i}__Kumiban"] = typeof(string);
                wfTypes[$"WFPayInfo_{i}__Pay"] = typeof(long);
                wfTypes[$"WFPayInfo_{i}__Tekichu_Hyo"] = typeof(string);
            }
            
            _typeDefinitions["WF"] = wfTypes;

            // H1レコード（表示用単複枠）- 基本定義のみ
            var h1Types = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "TansyoFlag", typeof(string) },
                { "FukusyoFlag", typeof(string) },
                { "WakurenFlag", typeof(string) },
                { "UmarenFlag", typeof(string) },
                { "WideFlag", typeof(string) },
                { "UmatanFlag", typeof(string) },
                { "SanrenpukuFlag", typeof(string) },
            };
            _typeDefinitions["H1"] = h1Types;

            // H6レコード（表示用三連単）- 基本定義のみ
            var h6Types = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "SanrentanFlag", typeof(string) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "Henkan", typeof(string) },
            };
            _typeDefinitions["H6"] = h6Types;

            // O2〜O6のオッズレコード基本定義
            var o2Types = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "HappyoTime_Hour", typeof(int) },
                { "HappyoTime_Minute", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "UmarenFlag", typeof(string) },
                { "Henkan", typeof(string) },
            };
            _typeDefinitions["O2"] = o2Types;

            // O3（ワイド）
            var o3Types = new Dictionary<string, Type>(o2Types);
            o3Types["WideFlag"] = typeof(string);
            o3Types.Remove("UmarenFlag");
            _typeDefinitions["O3"] = o3Types;

            // O4（馬単）
            var o4Types = new Dictionary<string, Type>(o2Types);
            o4Types["UmatanFlag"] = typeof(string);
            o4Types.Remove("UmarenFlag");
            _typeDefinitions["O4"] = o4Types;

            // O5（三連複）
            var o5Types = new Dictionary<string, Type>(o2Types);
            o5Types["SanrenpukuFlag"] = typeof(string);
            o5Types.Remove("UmarenFlag");
            _typeDefinitions["O5"] = o5Types;

            // O6（三連単）
            var o6Types = new Dictionary<string, Type>(o2Types);
            o6Types["SanrentanFlag"] = typeof(string);
            o6Types.Remove("UmarenFlag");
            _typeDefinitions["O6"] = o6Types;

            // 他のレコードタイプも必要に応じて追加
            
            // CKレコードの詳細定義を追加
            LoadCKDefinitions();
            
            // H1とH6の詳細定義を追加
            LoadH1H6Definitions();
            
            // O2-O6の詳細定義を追加
            LoadO2O6Definitions();
            
            // HRとO1の定義を追加
            LoadHRO1Definitions();
        }

        private readonly HashSet<string> _undefinedColumns = new HashSet<string>();

        /// <summary>
        /// カラム名から型を取得（未定義の場合はエラー出力）
        /// </summary>
        public Type GetColumnType(string recordSpec, string columnName)
        {
            // レコード固有の定義を確認
            if (_typeDefinitions.TryGetValue(recordSpec, out var recordTypes))
            {
                if (recordTypes.TryGetValue(columnName, out var type))
                {
                    return type;
                }
            }

            // 未定義のカラムを記録
            var key = $"{recordSpec}.{columnName}";
            if (!_undefinedColumns.Contains(key))
            {
                _undefinedColumns.Add(key);
                Console.WriteLine($"WARNING: Undefined column type - {key}");
            }

            // デフォルトはstring（推定はしない）
            return typeof(string);
        }

        /// <summary>
        /// 未定義カラムのサマリーを出力とファイル保存
        /// </summary>
        public void PrintUndefinedColumnsSummary()
        {
            if (_undefinedColumns.Count > 0)
            {
                Console.WriteLine("\n=== Undefined Column Types Summary ===");
                var groupedByRecord = _undefinedColumns
                    .Select(x => x.Split('.'))
                    .GroupBy(x => x[0])
                    .OrderBy(g => g.Key);

                foreach (var group in groupedByRecord)
                {
                    Console.WriteLine($"\n{group.Key} record:");
                    foreach (var col in group.OrderBy(x => x[1]))
                    {
                        Console.WriteLine($"  - {col[1]}");
                    }
                }
                Console.WriteLine($"\nTotal undefined columns: {_undefinedColumns.Count}");
                Console.WriteLine("=====================================\n");

                // ファイルに書き出し（開発用）
                SaveUndefinedColumnsToFile();
            }
        }

        /// <summary>
        /// 未定義カラムをCSVファイルに保存
        /// </summary>
        private void SaveUndefinedColumnsToFile()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"undefined_columns_{timestamp}.csv";
                
                using (var writer = new StreamWriter(filename))
                {
                    writer.WriteLine("RecordSpec,ColumnName,SuggestedType");
                    
                    var sortedColumns = _undefinedColumns
                        .Select(x => x.Split('.'))
                        .OrderBy(x => x[0])
                        .ThenBy(x => x[1]);

                    foreach (var col in sortedColumns)
                    {
                        var recordSpec = col[0];
                        var columnName = col[1];
                        var suggestedType = GuessTypeFromColumnName(columnName);
                        
                        writer.WriteLine($"{recordSpec},{columnName},{suggestedType}");
                    }
                }

                Console.WriteLine($"Undefined columns saved to: {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving undefined columns to file: {ex.Message}");
            }
        }

        /// <summary>
        /// カラム名から型を推測（参考用）
        /// </summary>
        private string GuessTypeFromColumnName(string columnName)
        {
            // 末尾や含まれる文字列から型を推測
            if (columnName.EndsWith("_Year") || columnName.EndsWith("_Month") || 
                columnName.EndsWith("_Day") || columnName.EndsWith("_Kaiji") ||
                columnName.EndsWith("_Nichiji") || columnName.EndsWith("_RaceNum") ||
                columnName.Contains("Tosu") || columnName.Contains("Umaban") ||
                columnName.Contains("Wakuban") || columnName.Contains("Ninki") ||
                columnName.Contains("Jyuni") || columnName.Contains("Chakujun") ||
                columnName.Contains("Count") || columnName.Contains("Kaisu"))
            {
                return "int";
            }

            if (columnName.Contains("Odds") || columnName.Contains("Time") ||
                columnName.Contains("Haito") || columnName.Contains("Kinryo") ||
                columnName.Contains("Fukusyo") || columnName.Contains("Tansyo"))
            {
                return "decimal";
            }

            if (columnName.Contains("Pay") || columnName.Contains("Honsyokin") ||
                columnName.Contains("Syokin") || columnName.Contains("Kingaku"))
            {
                return "long";
            }

            if (columnName.EndsWith("CD") || columnName.EndsWith("Kubun") ||
                columnName.EndsWith("Flag") || columnName.EndsWith("FLG"))
            {
                return "string (code)";
            }

            if (columnName.Contains("Name") || columnName.Contains("Mei") ||
                columnName.Contains("Ryakusyo") || columnName.Contains("Hondai"))
            {
                return "string (name)";
            }

            return "string";
        }

        /// <summary>
        /// 値を適切な型に変換（JRA-VAN特殊値を考慮）
        /// </summary>
        public object? ConvertValue(string? value, Type targetType)
        {
            // nullまたは空文字列の場合
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            // スペースのみの場合
            var trimmedValue = value.Trim();
            if (string.IsNullOrEmpty(trimmedValue))
            {
                // 数値型の場合は0として扱う（JRA-VANの仕様）
                if (targetType == typeof(int)) return 0;
                if (targetType == typeof(long)) return 0L;
                if (targetType == typeof(decimal)) return 0m;
                // 文字列型の場合は空文字列
                return string.Empty;
            }

            try
            {
                if (targetType == typeof(int))
                {
                    // 全てゼロの場合は0として扱う
                    if (trimmedValue.All(c => c == '0'))
                    {
                        return 0;
                    }
                    
                    // 全て9の場合（999, 9999など）は特殊値として扱う
                    // ※将来的にはnull許容型で扱うことも検討
                    if (trimmedValue.All(c => c == '9'))
                    {
                        // とりあえず値をそのまま返す（999なら999、9999なら9999）
                        if (int.TryParse(trimmedValue, out var nineValue))
                        {
                            return nineValue;
                        }
                    }
                    
                    if (int.TryParse(trimmedValue, out var intValue))
                    {
                        return intValue;
                    }
                    // 変換できない場合は0
                    return 0;
                }
                else if (targetType == typeof(long))
                {
                    if (trimmedValue.All(c => c == '0'))
                    {
                        return 0L;
                    }
                    
                    // 全て9の場合も値として扱う
                    if (trimmedValue.All(c => c == '9'))
                    {
                        if (long.TryParse(trimmedValue, out var nineValue))
                        {
                            return nineValue;
                        }
                    }
                    
                    if (long.TryParse(trimmedValue, out var longValue))
                    {
                        return longValue;
                    }
                    return 0L;
                }
                else if (targetType == typeof(decimal))
                {
                    if (trimmedValue.All(c => c == '0' || c == '.'))
                    {
                        return 0m;
                    }
                    
                    // 999.9のようなパターンも考慮
                    if (trimmedValue.Replace(".", "").All(c => c == '9'))
                    {
                        if (decimal.TryParse(trimmedValue, out var nineValue))
                        {
                            return nineValue;
                        }
                    }
                    
                    if (decimal.TryParse(trimmedValue, out var decimalValue))
                    {
                        return decimalValue;
                    }
                    return 0m;
                }
                else if (targetType == typeof(bool))
                {
                    return trimmedValue == "1" || trimmedValue.ToLower() == "true";
                }

                // stringの場合はtrimして返す
                return trimmedValue;
            }
            catch
            {
                // 変換エラーの場合
                if (targetType == typeof(int)) return 0;
                if (targetType == typeof(long)) return 0L;
                if (targetType == typeof(decimal)) return 0m;
                return string.Empty;
            }
        }
        
        /// <summary>
        /// CKレコードの詳細定義を追加
        /// </summary>
        private void LoadCKDefinitions()
        {
            var ckTypes = new Dictionary<string, Type>
            {
                // ヘッダー情報
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                
                // 馬主着度数
                { "BanusiChaku_BanusiCode", typeof(string) },
                { "BanusiChaku_BanusiName", typeof(string) },
                { "BanusiChaku_BanusiName_Co", typeof(string) },
                
                // 本累計（最新年と前年）の配列
                // HonRuikei_0 (最新年)
                { "BanusiChaku_HonRuikei_0__SetYear", typeof(string) },
                { "BanusiChaku_HonRuikei_0__HonSyokinTotal", typeof(long) },
                { "BanusiChaku_HonRuikei_0__FukaSyokin", typeof(long) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_0", typeof(int) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_1", typeof(int) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_2", typeof(int) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_3", typeof(int) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_4", typeof(int) },
                { "BanusiChaku_HonRuikei_0__ChakuKaisu_5", typeof(int) },
                
                // HonRuikei_1 (前年)
                { "BanusiChaku_HonRuikei_1__SetYear", typeof(string) },
                { "BanusiChaku_HonRuikei_1__HonSyokinTotal", typeof(long) },
                { "BanusiChaku_HonRuikei_1__FukaSyokin", typeof(long) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_0", typeof(int) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_1", typeof(int) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_2", typeof(int) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_3", typeof(int) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_4", typeof(int) },
                { "BanusiChaku_HonRuikei_1__ChakuKaisu_5", typeof(int) },
                
                // 生産者着度数
                { "BreederChaku_BreederCode", typeof(string) },
                { "BreederChaku_BreederName", typeof(string) },
                { "BreederChaku_BreederName_Co", typeof(string) },
                
                // 生産者本累計
                { "BreederChaku_HonRuikei_0__SetYear", typeof(string) },
                { "BreederChaku_HonRuikei_0__HonSyokinTotal", typeof(long) },
                { "BreederChaku_HonRuikei_0__FukaSyokin", typeof(long) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_0", typeof(int) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_1", typeof(int) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_2", typeof(int) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_3", typeof(int) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_4", typeof(int) },
                { "BreederChaku_HonRuikei_0__ChakuKaisu_5", typeof(int) },
                
                { "BreederChaku_HonRuikei_1__SetYear", typeof(string) },
                { "BreederChaku_HonRuikei_1__HonSyokinTotal", typeof(long) },
                { "BreederChaku_HonRuikei_1__FukaSyokin", typeof(long) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_0", typeof(int) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_1", typeof(int) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_2", typeof(int) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_3", typeof(int) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_4", typeof(int) },
                { "BreederChaku_HonRuikei_1__ChakuKaisu_5", typeof(int) },
                
                // 調教師着度数
                { "ChokyoChaku_ChokyosiCode", typeof(string) },
                { "ChokyoChaku_ChokyosiName", typeof(string) },
            };
            
            // 調教師着度数の詳細（ダート・距離別）
            // HonRuikei_0とHonRuikei_1の各種着回数
            for (int i = 0; i <= 1; i++)
            {
                // 基本情報
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__SetYear"] = typeof(string);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__HonSyokinTotal"] = typeof(long);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__FukaSyokin"] = typeof(long);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__HonSyokinHeichi"] = typeof(long);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__HonSyokinSyogai"] = typeof(long);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__FukaSyokinHeichi"] = typeof(long);
                ckTypes[$"ChokyoChaku_HonRuikei_{i}__FukaSyokinSyogai"] = typeof(long);
                
                // 芝
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuSiba_ChakuKaisu_{j}"] = typeof(int);
                }
                
                // ダート
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuDirt_ChakuKaisu_{j}"] = typeof(int);
                }
                
                // 芝距離別（9種類: 0-8）
                for (int k = 0; k <= 8; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuSibaKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // ダート距離別（9種類: 0-8）
                for (int k = 0; k <= 8; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuDirtKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 場所別（10場所）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuJyo_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // トラック別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuJyoDirt_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 芝場所別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuJyoSiba_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 障害
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuSyogai_ChakuKaisu_{j}"] = typeof(int);
                }
                
                // 障害場所別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"ChokyoChaku_HonRuikei_{i}__ChakuKaisuJyoSyogai_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
            }
            
            // 騎手着度数
            ckTypes["KisyuChaku_KisyuCode"] = typeof(string);
            ckTypes["KisyuChaku_KisyuName"] = typeof(string);
            
            // 騎手着度数の詳細
            for (int i = 0; i <= 1; i++)
            {
                ckTypes[$"KisyuChaku_HonRuikei_{i}__SetYear"] = typeof(string);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__HonSyokinTotal"] = typeof(long);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__FukaSyokin"] = typeof(long);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__HonSyokinHeichi"] = typeof(long);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__HonSyokinSyogai"] = typeof(long);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__FukaSyokinHeichi"] = typeof(long);
                ckTypes[$"KisyuChaku_HonRuikei_{i}__FukaSyokinSyogai"] = typeof(long);
                
                // 芝・ダート
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuSiba_ChakuKaisu_{j}"] = typeof(int);
                    ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuDirt_ChakuKaisu_{j}"] = typeof(int);
                }
                
                // 芝・ダート距離別（9種類: 0-8）
                for (int k = 0; k <= 8; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuSibaKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuDirtKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 場所別とトラック別
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuJyo_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // トラック別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuJyoDirt_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 芝場所別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuJyoSiba_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
                
                // 障害
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuSyogai_ChakuKaisu_{j}"] = typeof(int);
                }
                
                // 障害場所別（10種類: 0-9）
                for (int k = 0; k <= 9; k++)
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        ckTypes[$"KisyuChaku_HonRuikei_{i}__ChakuKaisuJyoSyogai_{k}__ChakuKaisu_{j}"] = typeof(int);
                    }
                }
            }
            
            // 馬着度数
            ckTypes["UmaChaku_KettoNum"] = typeof(string);
            ckTypes["UmaChaku_Bamei"] = typeof(string);
            ckTypes["UmaChaku_RuikeiHonsyoHeiti"] = typeof(long);
            ckTypes["UmaChaku_RuikeiHonsyoSyogai"] = typeof(long);
            ckTypes["UmaChaku_RuikeiFukaHeichi"] = typeof(long);
            ckTypes["UmaChaku_RuikeiFukaSyogai"] = typeof(long);
            ckTypes["UmaChaku_RuikeiSyutokuHeichi"] = typeof(long);
            ckTypes["UmaChaku_RuikeiSyutokuSyogai"] = typeof(long);
            ckTypes["UmaChaku_RaceCount"] = typeof(int);
            
            // 脚質 (0-3)
            for (int i = 0; i <= 3; i++)
            {
                ckTypes[$"UmaChaku_Kyakusitu_{i}"] = typeof(string);
            }
            
            // 馬の着回数（総合、中央、場別、状態別、距離別、ローテ別、回り別、騎手別、斤量別、ペース別）
            // 総合
            for (int j = 0; j <= 5; j++)
            {
                ckTypes[$"UmaChaku_ChakuSogo_ChakuKaisu_{j}"] = typeof(int);
            }
            
            // 中央
            for (int j = 0; j <= 5; j++)
            {
                ckTypes[$"UmaChaku_ChakuChuo_ChakuKaisu_{j}"] = typeof(int);
            }
            
            // 場別（7場）
            for (int k = 0; k <= 6; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuBa_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 状態別（12種）
            for (int k = 0; k <= 11; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuJyotai_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 距離別（5種）
            for (int k = 0; k <= 4; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                    ckTypes[$"UmaChaku_ChakuKaisuTurf_{k}__ChakuKaisu_{j}"] = typeof(int);
                    ckTypes[$"UmaChaku_ChakuKaisuDirt_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 芝距離別（9種類: 0-8）
            for (int k = 0; k <= 8; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuSibaKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // ダート距離別（9種類: 0-8）
            for (int k = 0; k <= 8; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuDirtKyori_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 場所別芝（10場所: 0-9）
            for (int k = 0; k <= 9; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuJyoSiba_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 場所別ダート（10場所: 0-9）
            for (int k = 0; k <= 9; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuJyoDirt_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // 場所別障害（10場所: 0-9）
            for (int k = 0; k <= 9; k++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    ckTypes[$"UmaChaku_ChakuKaisuJyoSyogai_{k}__ChakuKaisu_{j}"] = typeof(int);
                }
            }
            
            // その他の馬着度数パターンも追加
            // レース情報
            ckTypes["id_Year"] = typeof(int);
            ckTypes["id_MonthDay"] = typeof(int);
            ckTypes["id_JyoCD"] = typeof(string);
            ckTypes["id_Kaiji"] = typeof(int);
            ckTypes["id_Nichiji"] = typeof(int);
            ckTypes["id_RaceNum"] = typeof(int);
            
            // CKレコードを登録
            if (!_typeDefinitions.ContainsKey("CK"))
            {
                _typeDefinitions["CK"] = ckTypes;
            }
            else
            {
                // 既存の定義に追加
                foreach (var kvp in ckTypes)
                {
                    _typeDefinitions["CK"][kvp.Key] = kvp.Value;
                }
            }
        }
        
        /// <summary>
        /// H1とH6レコードの詳細定義を追加
        /// </summary>
        private void LoadH1H6Definitions()
        {
            // H1レコードの詳細定義を追加
            if (_typeDefinitions.ContainsKey("H1"))
            {
                var h1Types = _typeDefinitions["H1"];
                
                // FukuChakuBaraiKey
                h1Types["FukuChakuBaraiKey"] = typeof(string);
                
                // HenkanDoWaku配列（0-7）
                for (int i = 0; i <= 7; i++)
                {
                    h1Types[$"HenkanDoWaku_{i}"] = typeof(string);
                }
                
                // HenkanUma配列（0-27）
                for (int i = 0; i <= 27; i++)
                {
                    h1Types[$"HenkanUma_{i}"] = typeof(string);
                }
                
                // HenkanWaku配列（0-7）
                for (int i = 0; i <= 7; i++)
                {
                    h1Types[$"HenkanWaku_{i}"] = typeof(string);
                }
                
                // HatubaiFlag配列（0-9）
                for (int i = 0; i <= 9; i++)
                {
                    h1Types[$"HatubaiFlag_{i}"] = typeof(string);
                }
                
                // HyoTotal配列（0-13）
                for (int i = 0; i <= 13; i++)
                {
                    h1Types[$"HyoTotal_{i}"] = typeof(string);
                }
                
                // 各種オッズ配列
                // HyoTansyo（単勝オッズ）
                for (int i = 0; i < 28; i++)
                {
                    h1Types[$"HyoTansyo_{i}__Umaban"] = typeof(int);
                    h1Types[$"HyoTansyo_{i}__Hyo"] = typeof(decimal);
                    h1Types[$"HyoTansyo_{i}__Ninki"] = typeof(int);
                }
                
                // HyoFukusyo（複勝オッズ）
                for (int i = 0; i < 28; i++)
                {
                    h1Types[$"HyoFukusyo_{i}__Umaban"] = typeof(int);
                    h1Types[$"HyoFukusyo_{i}__HyoMin"] = typeof(decimal);
                    h1Types[$"HyoFukusyo_{i}__HyoMax"] = typeof(decimal);
                    h1Types[$"HyoFukusyo_{i}__Hyo"] = typeof(decimal);
                    h1Types[$"HyoFukusyo_{i}__Ninki"] = typeof(int);
                }
                
                // HyoWakuren（枠連オッズ）
                for (int i = 0; i < 36; i++)
                {
                    h1Types[$"HyoWakuren_{i}__Kumi"] = typeof(string);
                    h1Types[$"HyoWakuren_{i}__Hyo"] = typeof(string);
                    h1Types[$"HyoWakuren_{i}__Ninki"] = typeof(int);
                    h1Types[$"HyoWakuren_{i}__Umaban"] = typeof(int);
                }
                
                // HyoUmaren（馬連オッズ）
                for (int i = 0; i < 153; i++)
                {
                    h1Types[$"HyoUmaren_{i}__Kumi"] = typeof(string);
                    h1Types[$"HyoUmaren_{i}__Hyo"] = typeof(string);
                    h1Types[$"HyoUmaren_{i}__Ninki"] = typeof(int);
                }
                
                // HyoWide（ワイドオッズ）
                for (int i = 0; i < 153; i++)
                {
                    h1Types[$"HyoWide_{i}__Kumi"] = typeof(string);
                    h1Types[$"HyoWide_{i}__HyoMin"] = typeof(string);
                    h1Types[$"HyoWide_{i}__HyoMax"] = typeof(string);
                    h1Types[$"HyoWide_{i}__Hyo"] = typeof(string);
                    h1Types[$"HyoWide_{i}__Ninki"] = typeof(int);
                }
                
                // HyoUmatan（馬単オッズ）
                for (int i = 0; i < 306; i++)
                {
                    h1Types[$"HyoUmatan_{i}__Kumi"] = typeof(string);
                    h1Types[$"HyoUmatan_{i}__Hyo"] = typeof(string);
                    h1Types[$"HyoUmatan_{i}__Ninki"] = typeof(int);
                }
                
                // HyoSanrenpuku（三連複オッズ）
                for (int i = 0; i < 816; i++)
                {
                    h1Types[$"HyoSanrenpuku_{i}__Kumi"] = typeof(string);
                    h1Types[$"HyoSanrenpuku_{i}__Hyo"] = typeof(string);
                    h1Types[$"HyoSanrenpuku_{i}__Ninki"] = typeof(int);
                }
            }
            
            // H6レコードの詳細定義を追加
            if (_typeDefinitions.ContainsKey("H6"))
            {
                var h6Types = _typeDefinitions["H6"];
                
                // HatubaiFlag
                h6Types["HatubaiFlag"] = typeof(string);
                
                // HenkanUma配列（0-17）
                for (int i = 0; i <= 17; i++)
                {
                    h6Types[$"HenkanUma_{i}"] = typeof(string);
                }
                
                // HyoTotal配列（0-1）
                for (int i = 0; i <= 1; i++)
                {
                    h6Types[$"HyoTotal_{i}"] = typeof(string);
                }
                
                // HyoSanrentan（三連単オッズ）
                // 4896個の組み合わせ
                for (int i = 0; i < 4896; i++)
                {
                    h6Types[$"HyoSanrentan_{i}__Kumi"] = typeof(string);
                    h6Types[$"HyoSanrentan_{i}__Hyo"] = typeof(string);
                    h6Types[$"HyoSanrentan_{i}__Ninki"] = typeof(int);
                }
            }
        }
        
        /// <summary>
        /// O2-O6レコードの詳細定義を追加
        /// </summary>
        private void LoadO2O6Definitions()
        {
            // O2（馬連オッズ）の詳細定義
            if (_typeDefinitions.ContainsKey("O2"))
            {
                var o2Types = _typeDefinitions["O2"];
                
                // 時刻情報を修正
                o2Types["HappyoTime_Day"] = typeof(int);
                o2Types["HappyoTime_Month"] = typeof(int);
                o2Types["HappyoTime_Hour"] = typeof(int);
                o2Types["HappyoTime_Minute"] = typeof(int);
                
                // MonthDayを修正
                o2Types["id_MonthDay"] = typeof(int);
                
                // TotalHyosu追加
                o2Types["TotalHyosuUmaren"] = typeof(string);
                
                // OddsUmarenInfo配列（153個）
                for (int i = 0; i < 153; i++)
                {
                    o2Types[$"OddsUmarenInfo_{i}__Kumi"] = typeof(string);
                    o2Types[$"OddsUmarenInfo_{i}__Odds"] = typeof(decimal);
                    o2Types[$"OddsUmarenInfo_{i}__Ninki"] = typeof(int);
                }
            }
            
            // O3（ワイドオッズ）の詳細定義
            if (_typeDefinitions.ContainsKey("O3"))
            {
                var o3Types = _typeDefinitions["O3"];
                
                // 時刻情報
                o3Types["HappyoTime_Day"] = typeof(int);
                o3Types["HappyoTime_Month"] = typeof(int);
                o3Types["HappyoTime_Hour"] = typeof(int);
                o3Types["HappyoTime_Minute"] = typeof(int);
                o3Types["id_MonthDay"] = typeof(int);
                
                // TotalHyosu追加
                o3Types["TotalHyosuWide"] = typeof(string);
                
                // OddsWideInfo配列（153個）
                for (int i = 0; i < 153; i++)
                {
                    o3Types[$"OddsWideInfo_{i}__Kumi"] = typeof(string);
                    o3Types[$"OddsWideInfo_{i}__OddsLow"] = typeof(decimal);
                    o3Types[$"OddsWideInfo_{i}__OddsHigh"] = typeof(decimal);
                    o3Types[$"OddsWideInfo_{i}__Ninki"] = typeof(int);
                }
            }
            
            // O4（馬単オッズ）の詳細定義
            if (_typeDefinitions.ContainsKey("O4"))
            {
                var o4Types = _typeDefinitions["O4"];
                
                // 時刻情報
                o4Types["HappyoTime_Day"] = typeof(int);
                o4Types["HappyoTime_Month"] = typeof(int);
                o4Types["HappyoTime_Hour"] = typeof(int);
                o4Types["HappyoTime_Minute"] = typeof(int);
                o4Types["id_MonthDay"] = typeof(int);
                
                // TotalHyosu追加
                o4Types["TotalHyosuUmatan"] = typeof(string);
                
                // OddsUmatanInfo配列（306個）
                for (int i = 0; i < 306; i++)
                {
                    o4Types[$"OddsUmatanInfo_{i}__Kumi"] = typeof(string);
                    o4Types[$"OddsUmatanInfo_{i}__Odds"] = typeof(decimal);
                    o4Types[$"OddsUmatanInfo_{i}__Ninki"] = typeof(int);
                }
            }
            
            // O5（三連複オッズ）の詳細定義
            if (_typeDefinitions.ContainsKey("O5"))
            {
                var o5Types = _typeDefinitions["O5"];
                
                // 時刻情報
                o5Types["HappyoTime_Day"] = typeof(int);
                o5Types["HappyoTime_Month"] = typeof(int);
                o5Types["HappyoTime_Hour"] = typeof(int);
                o5Types["HappyoTime_Minute"] = typeof(int);
                o5Types["id_MonthDay"] = typeof(int);
                
                // TotalHyosu追加
                o5Types["TotalHyosuSanrenpuku"] = typeof(string);
                
                // OddsSanrenInfo配列（816個） - 名前を修正
                for (int i = 0; i < 816; i++)
                {
                    o5Types[$"OddsSanrenInfo_{i}__Kumi"] = typeof(string);
                    o5Types[$"OddsSanrenInfo_{i}__Odds"] = typeof(decimal);
                    o5Types[$"OddsSanrenInfo_{i}__Ninki"] = typeof(int);
                }
            }
            
            // O6（三連単オッズ）の詳細定義
            if (_typeDefinitions.ContainsKey("O6"))
            {
                var o6Types = _typeDefinitions["O6"];
                
                // 時刻情報
                o6Types["HappyoTime_Day"] = typeof(int);
                o6Types["HappyoTime_Month"] = typeof(int);
                o6Types["HappyoTime_Hour"] = typeof(int);
                o6Types["HappyoTime_Minute"] = typeof(int);
                o6Types["id_MonthDay"] = typeof(int);
                
                // TotalHyosu追加
                o6Types["TotalHyosuSanrentan"] = typeof(string);
                
                // OddsSanrentanInfo配列（4896個）
                for (int i = 0; i < 4896; i++)
                {
                    o6Types[$"OddsSanrentanInfo_{i}__Kumi"] = typeof(string);
                    o6Types[$"OddsSanrentanInfo_{i}__Odds"] = typeof(decimal);
                    o6Types[$"OddsSanrentanInfo_{i}__Ninki"] = typeof(int);
                }
            }
        }
        
        /// <summary>
        /// HRとO1レコードの定義を追加
        /// </summary>
        private void LoadHRO1Definitions()
        {
            // HRレコード（払戻）の定義
            var hrTypes = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
            };
            
            // HenkanDoWaku配列（0-7）
            for (int i = 0; i <= 7; i++)
            {
                hrTypes[$"HenkanDoWaku_{i}"] = typeof(string);
            }
            
            // FuseirituFlag配列（0-8）
            for (int i = 0; i <= 8; i++)
            {
                hrTypes[$"FuseirituFlag_{i}"] = typeof(string);
            }
            
            // HenkanFlag配列（0-27）
            for (int i = 0; i <= 27; i++)
            {
                hrTypes[$"HenkanFlag_{i}"] = typeof(string);
            }
            
            // HenkanUma配列（0-27）
            for (int i = 0; i <= 27; i++)
            {
                hrTypes[$"HenkanUma_{i}"] = typeof(string);
            }
            
            // HenkanWaku配列（0-7）
            for (int i = 0; i <= 7; i++)
            {
                hrTypes[$"HenkanWaku_{i}"] = typeof(string);
            }
            
            // TokubaraiFlag配列（0-8）
            for (int i = 0; i <= 8; i++)
            {
                hrTypes[$"TokubaraiFlag_{i}"] = typeof(string);
            }
            
            // 各種払戻情報
            // PayTansyo（単勝払戻）
            for (int i = 0; i < 3; i++)
            {
                hrTypes[$"PayTansyo_{i}__Umaban"] = typeof(int);
                hrTypes[$"PayTansyo_{i}__Pay"] = typeof(long);
                hrTypes[$"PayTansyo_{i}__Ninki"] = typeof(int);
            }
            
            // PayFukusyo（複勝払戻）
            for (int i = 0; i < 5; i++)
            {
                hrTypes[$"PayFukusyo_{i}__Umaban"] = typeof(int);
                hrTypes[$"PayFukusyo_{i}__Pay"] = typeof(long);
                hrTypes[$"PayFukusyo_{i}__Ninki"] = typeof(int);
            }
            
            // PayWakuren（枠連払戻）
            for (int i = 0; i < 3; i++)
            {
                hrTypes[$"PayWakuren_{i}__Kumi"] = typeof(string);
                hrTypes[$"PayWakuren_{i}__Pay"] = typeof(long);
                hrTypes[$"PayWakuren_{i}__Ninki"] = typeof(int);
                hrTypes[$"PayWakuren_{i}__Umaban"] = typeof(int);
            }
            
            // PayUmaren（馬連払戻）
            for (int i = 0; i < 3; i++)
            {
                hrTypes[$"PayUmaren_{i}__Kumi"] = typeof(string);
                hrTypes[$"PayUmaren_{i}__Pay"] = typeof(long);
                hrTypes[$"PayUmaren_{i}__Ninki"] = typeof(int);
            }
            
            // PayWide（ワイド払戻）
            for (int i = 0; i < 7; i++)
            {
                hrTypes[$"PayWide_{i}__Kumi"] = typeof(string);
                hrTypes[$"PayWide_{i}__Pay"] = typeof(long);
                hrTypes[$"PayWide_{i}__Ninki"] = typeof(int);
            }
            
            // PayUmatan（馬単払戻）
            for (int i = 0; i < 6; i++)
            {
                hrTypes[$"PayUmatan_{i}__Kumi"] = typeof(string);
                hrTypes[$"PayUmatan_{i}__Pay"] = typeof(long);
                hrTypes[$"PayUmatan_{i}__Ninki"] = typeof(int);
            }
            
            // PaySanrenpuku（三連複払戻）
            for (int i = 0; i < 3; i++)
            {
                hrTypes[$"PaySanrenpuku_{i}__Kumi"] = typeof(string);
                hrTypes[$"PaySanrenpuku_{i}__Pay"] = typeof(long);
                hrTypes[$"PaySanrenpuku_{i}__Ninki"] = typeof(int);
            }
            
            // PaySanrentan（三連単払戻）
            for (int i = 0; i < 6; i++)
            {
                hrTypes[$"PaySanrentan_{i}__Kumi"] = typeof(string);
                hrTypes[$"PaySanrentan_{i}__Pay"] = typeof(long);
                hrTypes[$"PaySanrentan_{i}__Ninki"] = typeof(int);
            }
            
            // PayReserved1（予備払戻）
            for (int i = 0; i < 3; i++)
            {
                hrTypes[$"PayReserved1_{i}__Kumi"] = typeof(long);
                hrTypes[$"PayReserved1_{i}__Pay"] = typeof(long);
                hrTypes[$"PayReserved1_{i}__Ninki"] = typeof(int);
            }
            
            _typeDefinitions["HR"] = hrTypes;
            
            // O1レコード（単複枠オッズ）の定義
            var o1Types = new Dictionary<string, Type>
            {
                { "head_RecordSpec", typeof(string) },
                { "head_DataKubun", typeof(string) },
                { "head_MakeDate_Year", typeof(int) },
                { "head_MakeDate_Month", typeof(int) },
                { "head_MakeDate_Day", typeof(int) },
                { "id_Year", typeof(int) },
                { "id_MonthDay", typeof(int) },
                { "id_JyoCD", typeof(string) },
                { "id_Kaiji", typeof(int) },
                { "id_Nichiji", typeof(int) },
                { "id_RaceNum", typeof(int) },
                { "HappyoTime_Month", typeof(int) },
                { "HappyoTime_Day", typeof(int) },
                { "HappyoTime_Hour", typeof(int) },
                { "HappyoTime_Minute", typeof(int) },
                { "TorokuTosu", typeof(int) },
                { "SyussoTosu", typeof(int) },
                { "TansyoFlag", typeof(string) },
                { "FukusyoFlag", typeof(string) },
                { "WakurenFlag", typeof(string) },
                { "FukuChakuBaraiKey", typeof(string) },
                { "TotalHyosuTansyo", typeof(string) },
                { "TotalHyosuFukusyo", typeof(string) },
                { "TotalHyosuWakuren", typeof(string) },
            };
            
            // TansyoOdds（単勝オッズ）- 名前を修正
            for (int i = 0; i < 28; i++)
            {
                o1Types[$"OddsTansyoInfo_{i}__Umaban"] = typeof(int);
                o1Types[$"OddsTansyoInfo_{i}__Odds"] = typeof(decimal);
                o1Types[$"OddsTansyoInfo_{i}__Ninki"] = typeof(int);
            }
            
            // FukusyoOdds（複勝オッズ）- 名前を修正
            for (int i = 0; i < 28; i++)
            {
                o1Types[$"OddsFukusyoInfo_{i}__Umaban"] = typeof(int);
                o1Types[$"OddsFukusyoInfo_{i}__OddsLow"] = typeof(decimal);
                o1Types[$"OddsFukusyoInfo_{i}__OddsHigh"] = typeof(decimal);
                o1Types[$"OddsFukusyoInfo_{i}__Ninki"] = typeof(int);
            }
            
            // WakurenOdds（枠連オッズ）
            for (int i = 0; i < 36; i++)
            {
                o1Types[$"OddsWakurenInfo_{i}__Kumi"] = typeof(string);
                o1Types[$"OddsWakurenInfo_{i}__Odds"] = typeof(decimal);
                o1Types[$"OddsWakurenInfo_{i}__Ninki"] = typeof(int);
            }
            
            _typeDefinitions["O1"] = o1Types;
        }
    }
}