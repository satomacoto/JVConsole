namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// WCレコード（ウッド調教タイム）の型マッピング
    /// </summary>
    public class WcRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WC";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // トレセン区分
            { "TresenKubun", typeof(string) },
            
            // 調教年月日
            { "ChokyoDate_Year", typeof(string) },
            { "ChokyoDate_Month", typeof(string) },
            { "ChokyoDate_Day", typeof(string) },
            
            // 調教情報
            { "ChokyoTime", typeof(string) },
            { "KettoNum", typeof(string) },
            { "Course", typeof(string) },
            { "BabaAround", typeof(string) },
            { "reserved", typeof(string) },
            
            // ハロンタイム（10～1ハロン）
            { "HaronTime10", typeof(string) },
            { "LapTime10", typeof(string) },
            { "HaronTime9", typeof(string) },
            { "LapTime9", typeof(string) },
            { "HaronTime8", typeof(string) },
            { "LapTime8", typeof(string) },
            { "HaronTime7", typeof(string) },
            { "LapTime7", typeof(string) },
            { "HaronTime6", typeof(string) },
            { "LapTime6", typeof(string) },
            { "HaronTime5", typeof(string) },
            { "LapTime5", typeof(string) },
            { "HaronTime4", typeof(string) },
            { "LapTime4", typeof(string) },
            { "HaronTime3", typeof(string) },
            { "LapTime3", typeof(string) },
            { "HaronTime2", typeof(string) },
            { "LapTime2", typeof(string) },
            { "LapTime1", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "TresenKubun", "ChokyoDate_Year", "ChokyoDate_Month", "ChokyoDate_Day", "KettoNum"
        };
    }
}