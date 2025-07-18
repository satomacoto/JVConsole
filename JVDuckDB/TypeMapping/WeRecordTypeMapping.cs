using System;

namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// WEレコード（天候・馬場状態）の型マッピング
    /// </summary>
    public class WeRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WE";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報２
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            
            // 発表時刻
            { "HappyoTime_Month", typeof(string) },
            { "HappyoTime_Day", typeof(string) },
            { "HappyoTime_Hour", typeof(string) },
            { "HappyoTime_Minute", typeof(string) },
            
            // 変更識別
            { "HenkoID", typeof(string) },
            
            // 現在状態情報（構造体として文字列保存）
            { "TenkoBaba", typeof(string) },
            
            // 変更前状態情報（構造体として文字列保存）
            { "TenkoBabaBefore", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 拡張フィールド
            { "happyo_datetime", typeof(DateTime) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji"
        };
    }
}