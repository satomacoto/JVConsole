namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// HNレコード（繁殖馬）の型マッピング
    /// </summary>
    public class HnRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HN";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 基本情報
            { "HansyokuNum", typeof(string) },
            { "reserved", typeof(string) },
            { "KettoNum", typeof(string) },
            { "DelKubun", typeof(string) },
            { "Bamei", typeof(string) },
            { "BameiKana", typeof(string) },
            { "BameiEng", typeof(string) },
            { "BirthYear", typeof(string) },
            { "SexCD", typeof(string) },
            { "HinsyuCD", typeof(string) },
            { "KeiroCD", typeof(string) },
            { "HansyokuMochiKubun", typeof(string) },
            { "ImportYear", typeof(string) },
            { "SanchiName", typeof(string) },
            { "HansyokuFNum", typeof(string) },
            { "HansyokuMNum", typeof(string) },
            
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