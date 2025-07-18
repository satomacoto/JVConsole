namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// BTレコード（系統情報）の型マッピング
    /// </summary>
    public class BtRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "BT";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 系統情報
            { "HansyokuNum", typeof(string) },
            { "KeitoId", typeof(string) },
            { "KeitoName", typeof(string) },
            { "KeitoEx", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "HansyokuNum", "KeitoId"
        };
    }
}