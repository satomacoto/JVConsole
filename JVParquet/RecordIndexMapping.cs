using System.Collections.Generic;

namespace JVParquet
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
            
            // その他
            ["UM"] = new() { "KettoNum" },
            ["HN"] = new() { "HansyokuFNum", "KettoNum", "BirthDate_Year" },
            ["BT"] = new() { "HansyokuNum" },
            ["SK"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["YS"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["CH"] = new() { "ChokyosiCode" },
            ["BR"] = new() { "BreederCode" },
            ["BN"] = new() { "BanusiCode" },
            ["KS"] = new() { "KisyuCode" },
            
            // 調教・競走関連（注意：実際のカラム名を要確認）
            ["WC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "id_Umaban" },
            ["WF"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" },
            ["JG"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Umaban", "KettoNum" },
            ["HC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "id_Umaban" },
            ["CC"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "ChakuUmaInfo_0__KettoNum" },
            ["WH"] = new() { "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" }
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