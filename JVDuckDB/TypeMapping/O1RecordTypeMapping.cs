namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// O1レコード（オッズ（単勝・複勝・枠連））の型マッピング
    /// </summary>
    public class O1RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O1";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // 発表時刻
            { "HappyoTime_Month", typeof(string) },
            { "HappyoTime_Day", typeof(string) },
            { "HappyoTime_Hour", typeof(string) },
            { "HappyoTime_Minute", typeof(string) },
            
            // 基本情報
            { "TorokuTosu", typeof(int) },
            { "SyussoTosu", typeof(int) },
            { "TansyoFlag", typeof(string) },
            { "FukusyoFlag", typeof(string) },
            { "WakurenFlag", typeof(string) },
            { "FukuChakuBaraiKey", typeof(string) },
            
            // 票数
            { "TotalHyosuTansyo", typeof(long) },
            { "TotalHyosuFukusyo", typeof(long) },
            { "TotalHyosuWakuren", typeof(long) },
            
            // オッズ情報（構造体として文字列保存）
            // 単勝オッズ（28個）
            { "OddsTansyoInfo_0", typeof(string) },
            { "OddsTansyoInfo_1", typeof(string) },
            { "OddsTansyoInfo_2", typeof(string) },
            { "OddsTansyoInfo_3", typeof(string) },
            { "OddsTansyoInfo_4", typeof(string) },
            { "OddsTansyoInfo_5", typeof(string) },
            { "OddsTansyoInfo_6", typeof(string) },
            { "OddsTansyoInfo_7", typeof(string) },
            { "OddsTansyoInfo_8", typeof(string) },
            { "OddsTansyoInfo_9", typeof(string) },
            { "OddsTansyoInfo_10", typeof(string) },
            { "OddsTansyoInfo_11", typeof(string) },
            { "OddsTansyoInfo_12", typeof(string) },
            { "OddsTansyoInfo_13", typeof(string) },
            { "OddsTansyoInfo_14", typeof(string) },
            { "OddsTansyoInfo_15", typeof(string) },
            { "OddsTansyoInfo_16", typeof(string) },
            { "OddsTansyoInfo_17", typeof(string) },
            { "OddsTansyoInfo_18", typeof(string) },
            { "OddsTansyoInfo_19", typeof(string) },
            { "OddsTansyoInfo_20", typeof(string) },
            { "OddsTansyoInfo_21", typeof(string) },
            { "OddsTansyoInfo_22", typeof(string) },
            { "OddsTansyoInfo_23", typeof(string) },
            { "OddsTansyoInfo_24", typeof(string) },
            { "OddsTansyoInfo_25", typeof(string) },
            { "OddsTansyoInfo_26", typeof(string) },
            { "OddsTansyoInfo_27", typeof(string) },
            
            // 複勝オッズ（28個）
            { "OddsFukusyoInfo_0", typeof(string) },
            { "OddsFukusyoInfo_1", typeof(string) },
            { "OddsFukusyoInfo_2", typeof(string) },
            { "OddsFukusyoInfo_3", typeof(string) },
            { "OddsFukusyoInfo_4", typeof(string) },
            { "OddsFukusyoInfo_5", typeof(string) },
            { "OddsFukusyoInfo_6", typeof(string) },
            { "OddsFukusyoInfo_7", typeof(string) },
            { "OddsFukusyoInfo_8", typeof(string) },
            { "OddsFukusyoInfo_9", typeof(string) },
            { "OddsFukusyoInfo_10", typeof(string) },
            { "OddsFukusyoInfo_11", typeof(string) },
            { "OddsFukusyoInfo_12", typeof(string) },
            { "OddsFukusyoInfo_13", typeof(string) },
            { "OddsFukusyoInfo_14", typeof(string) },
            { "OddsFukusyoInfo_15", typeof(string) },
            { "OddsFukusyoInfo_16", typeof(string) },
            { "OddsFukusyoInfo_17", typeof(string) },
            { "OddsFukusyoInfo_18", typeof(string) },
            { "OddsFukusyoInfo_19", typeof(string) },
            { "OddsFukusyoInfo_20", typeof(string) },
            { "OddsFukusyoInfo_21", typeof(string) },
            { "OddsFukusyoInfo_22", typeof(string) },
            { "OddsFukusyoInfo_23", typeof(string) },
            { "OddsFukusyoInfo_24", typeof(string) },
            { "OddsFukusyoInfo_25", typeof(string) },
            { "OddsFukusyoInfo_26", typeof(string) },
            { "OddsFukusyoInfo_27", typeof(string) },
            
            // 枠連オッズ（36個）
            { "OddsWakurenInfo_0", typeof(string) },
            { "OddsWakurenInfo_1", typeof(string) },
            { "OddsWakurenInfo_2", typeof(string) },
            { "OddsWakurenInfo_3", typeof(string) },
            { "OddsWakurenInfo_4", typeof(string) },
            { "OddsWakurenInfo_5", typeof(string) },
            { "OddsWakurenInfo_6", typeof(string) },
            { "OddsWakurenInfo_7", typeof(string) },
            { "OddsWakurenInfo_8", typeof(string) },
            { "OddsWakurenInfo_9", typeof(string) },
            { "OddsWakurenInfo_10", typeof(string) },
            { "OddsWakurenInfo_11", typeof(string) },
            { "OddsWakurenInfo_12", typeof(string) },
            { "OddsWakurenInfo_13", typeof(string) },
            { "OddsWakurenInfo_14", typeof(string) },
            { "OddsWakurenInfo_15", typeof(string) },
            { "OddsWakurenInfo_16", typeof(string) },
            { "OddsWakurenInfo_17", typeof(string) },
            { "OddsWakurenInfo_18", typeof(string) },
            { "OddsWakurenInfo_19", typeof(string) },
            { "OddsWakurenInfo_20", typeof(string) },
            { "OddsWakurenInfo_21", typeof(string) },
            { "OddsWakurenInfo_22", typeof(string) },
            { "OddsWakurenInfo_23", typeof(string) },
            { "OddsWakurenInfo_24", typeof(string) },
            { "OddsWakurenInfo_25", typeof(string) },
            { "OddsWakurenInfo_26", typeof(string) },
            { "OddsWakurenInfo_27", typeof(string) },
            { "OddsWakurenInfo_28", typeof(string) },
            { "OddsWakurenInfo_29", typeof(string) },
            { "OddsWakurenInfo_30", typeof(string) },
            { "OddsWakurenInfo_31", typeof(string) },
            { "OddsWakurenInfo_32", typeof(string) },
            { "OddsWakurenInfo_33", typeof(string) },
            { "OddsWakurenInfo_34", typeof(string) },
            { "OddsWakurenInfo_35", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 拡張フィールド
            { "race_id", typeof(string) },
            { "happyo_datetime", typeof(DateTime) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}