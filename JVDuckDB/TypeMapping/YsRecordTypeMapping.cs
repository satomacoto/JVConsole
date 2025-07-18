namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// YSレコード（開催日程）の型マッピング
    /// </summary>
    public class YsRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "YS";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報２
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            
            // 曜日コード
            { "YoubiCD", typeof(string) },
            
            // 重賞案内（3レース分、構造体として文字列保存）
            { "JyusyoInfo_0", typeof(string) },
            { "JyusyoInfo_1", typeof(string) },
            { "JyusyoInfo_2", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji"
        };
    }
}