namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// HSレコード（馬市取引価格）の型マッピング
    /// </summary>
    public class HsRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HS";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 血統情報
            { "KettoNum", typeof(string) },
            { "HansyokuFNum", typeof(string) },
            { "HansyokuMNum", typeof(string) },
            { "BirthYear", typeof(string) },
            
            // 市場情報
            { "SaleCode", typeof(string) },
            { "SaleHostName", typeof(string) },
            { "SaleName", typeof(string) },
            
            // 開催期間
            { "FromDate_Year", typeof(string) },
            { "FromDate_Month", typeof(string) },
            { "FromDate_Day", typeof(string) },
            { "ToDate_Year", typeof(string) },
            { "ToDate_Month", typeof(string) },
            { "ToDate_Day", typeof(string) },
            
            // 取引情報
            { "Barei", typeof(string) },
            { "Price", typeof(long) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "KettoNum", "SaleCode"
        };
    }
}