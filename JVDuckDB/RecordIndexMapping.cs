using System.Collections.Generic;

namespace JVDuckDB
{
    /// <summary>
    /// 各レコードタイプのインデックスカラム情報を管理するクラス
    /// </summary>
    public static class RecordIndexMapping
    {
        /// <summary>
        /// レコードタイプごとのインデックスカラム定義（実際のParquetカラム名）
        /// </summary>
        public static readonly Dictionary<string, List<string>> IndexColumns = new()
        {
            // レース関連
            ["RA"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["SE"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "Umaban", "KettoNum" },
            ["HR"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            
            // オッズ関連
            ["O1"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["O2"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["O3"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["O4"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["O5"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["O6"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            
            // 票数関連
            ["H1"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["H6"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            
            // マスター系レコード (RECORDSPECS_MASTER)
            ["UM"] = new() { "KettoNum" },                    // 競走馬マスタ
            ["KS"] = new() { "KisyuCode" },                   // 騎手マスタ
            ["CH"] = new() { "ChokyosiCode" },                // 調教師マスタ
            ["BR"] = new() { "BreederCode" },                 // 生産者マスタ
            ["BN"] = new() { "BanusiCode" },                  // 馬主マスタ
            ["RC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "TokuNum", "SyubetuCD", "Kyori", "TrackCD" }, // レコードマスタ
            ["HN"] = new() { "HansyokuNum" },                 // 繁殖馬マスタ（注：Pythonの定義と異なる可能性あり）
            ["SK"] = new() { "KettoNum" },                    // 産駒マスタ
            ["BT"] = new() { "HansyokuNum" },                 // 系統情報
            
            // その他
            ["YS"] = new() { "id_Year", "id_JyoCD", "id_Kaiji", "id_Nichiji" },
            
            // 調教・競走関連（注意：実際のカラム名を要確認）
            ["WC"] = new() { "TresenKubun", "ChokyoDate_Year", "ChokyoDate_Month", "ChokyoDate_Day", "ChokyoTime", "KettoNum" },
            ["WF"] = new() { "id_Year", "id_MonthDay", "id_JyoCD" },
            ["JG"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "KettoNum" },
            ["HC"] = new() { "TresenKubun", "ChokyoDate_Year", "ChokyoDate_Month", "ChokyoDate_Day", "ChokyoTime", "KettoNum" },
            ["CC"] = new() { "id_Year", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["WH"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            
            // その他の情報系レコード
            ["AV"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["JC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["TC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["CK"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "Umaban" },
            ["CS"] = new() { "id_JyoCD", "TrackCD", "CourseKubun", "Kyori" },
            ["DM"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "Umaban" },
            ["HS"] = new() { "KettoNum" },
            ["HY"] = new() { "KettoNum" },
            ["TK"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["TM"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["WE"] = new() { "id_Year", "id_JyoCD", "id_Kaiji", "id_Nichiji", "HenkoID" }
        };

        /// <summary>
        /// 指定されたレコードタイプのインデックスカラムを取得
        /// </summary>
        public static List<string>? GetIndexColumns(string recordSpec)
        {
            return IndexColumns.TryGetValue(recordSpec, out var columns) ? columns : null;
        }

        /// <summary>
        /// 指定されたレコードタイプがインデックスカラムを持っているかチェック
        /// </summary>
        public static bool HasIndexColumns(string recordSpec)
        {
            return IndexColumns.ContainsKey(recordSpec);
        }
    }
}