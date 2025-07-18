namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// RCレコード（レコード）の型マッピング
    /// </summary>
    public class RcRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "RC";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコード識別区分
            { "RecInfoKubun", typeof(string) },
            
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // レース情報
            { "TokuNum", typeof(string) },
            { "Hondai", typeof(string) },
            { "GradeCD", typeof(string) },
            { "SyubetuCD", typeof(string) },
            { "Kyori", typeof(int) },
            { "TrackCD", typeof(string) },
            { "RecKubun", typeof(string) },
            { "RecTime", typeof(string) },
            
            // 天候・馬場状態（構造体として文字列保存）
            { "TenkoBaba", typeof(string) },
            
            // レコード保持馬情報（構造体として文字列保存）
            { "RecUmaInfo_0", typeof(string) },
            { "RecUmaInfo_1", typeof(string) },
            { "RecUmaInfo_2", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}