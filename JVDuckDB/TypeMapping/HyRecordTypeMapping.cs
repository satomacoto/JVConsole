namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// HYレコード（馬名意味由来）の型マッピング
    /// </summary>
    public class HyRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HY";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 馬情報
            { "KettoNum", typeof(string) },
            { "Bamei", typeof(string) },
            { "Origin", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "KettoNum"
        };
    }
}