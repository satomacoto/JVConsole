namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// SKレコード（産駒マスタ）の型マッピング
    /// </summary>
    public class SkRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "SK";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 産駒情報
            { "KettoNum", typeof(string) },
            { "BirthDate_Year", typeof(string) },
            { "BirthDate_Month", typeof(string) },
            { "BirthDate_Day", typeof(string) },
            { "SexCD", typeof(string) },
            { "HinsyuCD", typeof(string) },
            { "KeiroCD", typeof(string) },
            { "SankuMochiKubun", typeof(string) },
            { "ImportYear", typeof(string) },
            { "BreederCode", typeof(string) },
            { "SanchiName", typeof(string) },
            
            // 3代血統 繁殖登録番号（14個、0-based）
            { "HansyokuNum_0", typeof(string) },
            { "HansyokuNum_1", typeof(string) },
            { "HansyokuNum_2", typeof(string) },
            { "HansyokuNum_3", typeof(string) },
            { "HansyokuNum_4", typeof(string) },
            { "HansyokuNum_5", typeof(string) },
            { "HansyokuNum_6", typeof(string) },
            { "HansyokuNum_7", typeof(string) },
            { "HansyokuNum_8", typeof(string) },
            { "HansyokuNum_9", typeof(string) },
            { "HansyokuNum_10", typeof(string) },
            { "HansyokuNum_11", typeof(string) },
            { "HansyokuNum_12", typeof(string) },
            { "HansyokuNum_13", typeof(string) },
            
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