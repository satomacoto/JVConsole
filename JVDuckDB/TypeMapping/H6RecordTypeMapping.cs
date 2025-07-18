namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// H6レコード（票数（３連単））の型マッピング
    /// </summary>
    public class H6RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "H6";
        
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
            { "TorokuTosu", typeof(int) },
            { "SyussoTosu", typeof(int) },
            { "HatubaiFlag", typeof(string) },
            
            // 返還馬番（18個、0-based）
            { "HenkanUma_0", typeof(string) },
            { "HenkanUma_1", typeof(string) },
            { "HenkanUma_2", typeof(string) },
            { "HenkanUma_3", typeof(string) },
            { "HenkanUma_4", typeof(string) },
            { "HenkanUma_5", typeof(string) },
            { "HenkanUma_6", typeof(string) },
            { "HenkanUma_7", typeof(string) },
            { "HenkanUma_8", typeof(string) },
            { "HenkanUma_9", typeof(string) },
            { "HenkanUma_10", typeof(string) },
            { "HenkanUma_11", typeof(string) },
            { "HenkanUma_12", typeof(string) },
            { "HenkanUma_13", typeof(string) },
            { "HenkanUma_14", typeof(string) },
            { "HenkanUma_15", typeof(string) },
            { "HenkanUma_16", typeof(string) },
            { "HenkanUma_17", typeof(string) },
            
            // 票数合計
            { "HyoTotal_0", typeof(long) },
            { "HyoTotal_1", typeof(long) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 注：HyoSanrentan配列（4896個）は構造体として文字列保存される
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}