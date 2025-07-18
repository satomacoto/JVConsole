using System;

namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// WHレコード（馬体重）の型マッピング
    /// </summary>
    public class WhRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WH";
        
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
            
            // 馬体重情報（18頭分、構造体として文字列保存）
            { "BataijyuInfo_0", typeof(string) },
            { "BataijyuInfo_1", typeof(string) },
            { "BataijyuInfo_2", typeof(string) },
            { "BataijyuInfo_3", typeof(string) },
            { "BataijyuInfo_4", typeof(string) },
            { "BataijyuInfo_5", typeof(string) },
            { "BataijyuInfo_6", typeof(string) },
            { "BataijyuInfo_7", typeof(string) },
            { "BataijyuInfo_8", typeof(string) },
            { "BataijyuInfo_9", typeof(string) },
            { "BataijyuInfo_10", typeof(string) },
            { "BataijyuInfo_11", typeof(string) },
            { "BataijyuInfo_12", typeof(string) },
            { "BataijyuInfo_13", typeof(string) },
            { "BataijyuInfo_14", typeof(string) },
            { "BataijyuInfo_15", typeof(string) },
            { "BataijyuInfo_16", typeof(string) },
            { "BataijyuInfo_17", typeof(string) },
            
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