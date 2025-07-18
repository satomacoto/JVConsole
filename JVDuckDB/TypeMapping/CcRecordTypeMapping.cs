using System;

namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// CCレコード（コース変更）の型マッピング
    /// </summary>
    public class CcRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CC";
        
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
            
            // 変更後情報
            { "CCInfoAfter_Kyori", typeof(int) },
            { "CCInfoAfter_TruckCd", typeof(string) },
            
            // 変更前情報
            { "CCInfoBefore_Kyori", typeof(int) },
            { "CCInfoBefore_TruckCd", typeof(string) },
            
            // 事由コード
            { "JiyuCd", typeof(string) },
            
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