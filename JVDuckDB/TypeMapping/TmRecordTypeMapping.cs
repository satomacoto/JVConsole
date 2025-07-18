namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// TMレコード（タイムマイニング予想）の型マッピング
    /// </summary>
    public class TmRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "TM";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // データ作成時分
            { "MakeHM_Hour", typeof(string) },
            { "MakeHM_Minute", typeof(string) },
            
            // マイニング予想情報（18頭分、0-based）
            { "TMInfo_0_Umaban", typeof(string) },
            { "TMInfo_0_TMScore", typeof(string) },
            { "TMInfo_1_Umaban", typeof(string) },
            { "TMInfo_1_TMScore", typeof(string) },
            { "TMInfo_2_Umaban", typeof(string) },
            { "TMInfo_2_TMScore", typeof(string) },
            { "TMInfo_3_Umaban", typeof(string) },
            { "TMInfo_3_TMScore", typeof(string) },
            { "TMInfo_4_Umaban", typeof(string) },
            { "TMInfo_4_TMScore", typeof(string) },
            { "TMInfo_5_Umaban", typeof(string) },
            { "TMInfo_5_TMScore", typeof(string) },
            { "TMInfo_6_Umaban", typeof(string) },
            { "TMInfo_6_TMScore", typeof(string) },
            { "TMInfo_7_Umaban", typeof(string) },
            { "TMInfo_7_TMScore", typeof(string) },
            { "TMInfo_8_Umaban", typeof(string) },
            { "TMInfo_8_TMScore", typeof(string) },
            { "TMInfo_9_Umaban", typeof(string) },
            { "TMInfo_9_TMScore", typeof(string) },
            { "TMInfo_10_Umaban", typeof(string) },
            { "TMInfo_10_TMScore", typeof(string) },
            { "TMInfo_11_Umaban", typeof(string) },
            { "TMInfo_11_TMScore", typeof(string) },
            { "TMInfo_12_Umaban", typeof(string) },
            { "TMInfo_12_TMScore", typeof(string) },
            { "TMInfo_13_Umaban", typeof(string) },
            { "TMInfo_13_TMScore", typeof(string) },
            { "TMInfo_14_Umaban", typeof(string) },
            { "TMInfo_14_TMScore", typeof(string) },
            { "TMInfo_15_Umaban", typeof(string) },
            { "TMInfo_15_TMScore", typeof(string) },
            { "TMInfo_16_Umaban", typeof(string) },
            { "TMInfo_16_TMScore", typeof(string) },
            { "TMInfo_17_Umaban", typeof(string) },
            { "TMInfo_17_TMScore", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}