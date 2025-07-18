using System;
using System.Collections.Generic;

namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// O3レコード（オッズ（ワイド））の型マッピング
    /// </summary>
    public class O3RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O3";
        
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
            { "WideFlag", typeof(string) },
            
            // 票数
            { "TotalHyosuWide", typeof(long) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 注：OddsWideInfo配列（153個）は構造体として文字列保存される
            
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