namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// CSレコード（コース情報）の型マッピング
    /// </summary>
    public class CsRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CS";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // コース情報
            { "JyoCD", typeof(string) },
            { "Kyori", typeof(int) },
            { "TrackCD", typeof(string) },
            
            // コース改修年月日
            { "KaishuDate_Year", typeof(string) },
            { "KaishuDate_Month", typeof(string) },
            { "KaishuDate_Day", typeof(string) },
            
            // コース説明
            { "CourseEx", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "JyoCD", "Kyori", "TrackCD"
        };
    }
}