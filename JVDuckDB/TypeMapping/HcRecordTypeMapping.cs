namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// HCレコード（ハンロ調教タイム）の型マッピング
    /// </summary>
    public class HcRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HC";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // トレセン区分
            { "TresenKubun", typeof(string) },
            
            // 調教年月日
            { "ChokyoDate_Year", typeof(string) },
            { "ChokyoDate_Month", typeof(string) },
            { "ChokyoDate_Day", typeof(string) },
            
            // 調教時刻
            { "ChokyoTime", typeof(string) },
            
            // 血統登録番号
            { "KettoNum", typeof(string) },
            
            // ハロンタイム
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