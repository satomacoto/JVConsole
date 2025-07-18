namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// JGレコード（除外馬）の型マッピング
    /// </summary>
    public class JgRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "JG";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // 基本情報
            { "KettoNum", typeof(string) },
            { "Bamei", typeof(string) },
            { "ShutsubaTohyoJun", typeof(int) },
            { "ShussoKubun", typeof(string) },
            { "JogaiJotaiKubun", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "KettoNum" 
        };
    }
}