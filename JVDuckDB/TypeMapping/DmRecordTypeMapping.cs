namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// DMレコード（データマイニング予想）の型マッピング
    /// </summary>
    public class DmRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "DM";
        
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
            { "DMInfo_0_Umaban", typeof(string) },
            { "DMInfo_0_DMTime", typeof(string) },
            { "DMInfo_0_DMGosaP", typeof(string) },
            { "DMInfo_0_DMGosaM", typeof(string) },
            { "DMInfo_1_Umaban", typeof(string) },
            { "DMInfo_1_DMTime", typeof(string) },
            { "DMInfo_1_DMGosaP", typeof(string) },
            { "DMInfo_1_DMGosaM", typeof(string) },
            { "DMInfo_2_Umaban", typeof(string) },
            { "DMInfo_2_DMTime", typeof(string) },
            { "DMInfo_2_DMGosaP", typeof(string) },
            { "DMInfo_2_DMGosaM", typeof(string) },
            { "DMInfo_3_Umaban", typeof(string) },
            { "DMInfo_3_DMTime", typeof(string) },
            { "DMInfo_3_DMGosaP", typeof(string) },
            { "DMInfo_3_DMGosaM", typeof(string) },
            { "DMInfo_4_Umaban", typeof(string) },
            { "DMInfo_4_DMTime", typeof(string) },
            { "DMInfo_4_DMGosaP", typeof(string) },
            { "DMInfo_4_DMGosaM", typeof(string) },
            { "DMInfo_5_Umaban", typeof(string) },
            { "DMInfo_5_DMTime", typeof(string) },
            { "DMInfo_5_DMGosaP", typeof(string) },
            { "DMInfo_5_DMGosaM", typeof(string) },
            { "DMInfo_6_Umaban", typeof(string) },
            { "DMInfo_6_DMTime", typeof(string) },
            { "DMInfo_6_DMGosaP", typeof(string) },
            { "DMInfo_6_DMGosaM", typeof(string) },
            { "DMInfo_7_Umaban", typeof(string) },
            { "DMInfo_7_DMTime", typeof(string) },
            { "DMInfo_7_DMGosaP", typeof(string) },
            { "DMInfo_7_DMGosaM", typeof(string) },
            { "DMInfo_8_Umaban", typeof(string) },
            { "DMInfo_8_DMTime", typeof(string) },
            { "DMInfo_8_DMGosaP", typeof(string) },
            { "DMInfo_8_DMGosaM", typeof(string) },
            { "DMInfo_9_Umaban", typeof(string) },
            { "DMInfo_9_DMTime", typeof(string) },
            { "DMInfo_9_DMGosaP", typeof(string) },
            { "DMInfo_9_DMGosaM", typeof(string) },
            { "DMInfo_10_Umaban", typeof(string) },
            { "DMInfo_10_DMTime", typeof(string) },
            { "DMInfo_10_DMGosaP", typeof(string) },
            { "DMInfo_10_DMGosaM", typeof(string) },
            { "DMInfo_11_Umaban", typeof(string) },
            { "DMInfo_11_DMTime", typeof(string) },
            { "DMInfo_11_DMGosaP", typeof(string) },
            { "DMInfo_11_DMGosaM", typeof(string) },
            { "DMInfo_12_Umaban", typeof(string) },
            { "DMInfo_12_DMTime", typeof(string) },
            { "DMInfo_12_DMGosaP", typeof(string) },
            { "DMInfo_12_DMGosaM", typeof(string) },
            { "DMInfo_13_Umaban", typeof(string) },
            { "DMInfo_13_DMTime", typeof(string) },
            { "DMInfo_13_DMGosaP", typeof(string) },
            { "DMInfo_13_DMGosaM", typeof(string) },
            { "DMInfo_14_Umaban", typeof(string) },
            { "DMInfo_14_DMTime", typeof(string) },
            { "DMInfo_14_DMGosaP", typeof(string) },
            { "DMInfo_14_DMGosaM", typeof(string) },
            { "DMInfo_15_Umaban", typeof(string) },
            { "DMInfo_15_DMTime", typeof(string) },
            { "DMInfo_15_DMGosaP", typeof(string) },
            { "DMInfo_15_DMGosaM", typeof(string) },
            { "DMInfo_16_Umaban", typeof(string) },
            { "DMInfo_16_DMTime", typeof(string) },
            { "DMInfo_16_DMGosaP", typeof(string) },
            { "DMInfo_16_DMGosaM", typeof(string) },
            { "DMInfo_17_Umaban", typeof(string) },
            { "DMInfo_17_DMTime", typeof(string) },
            { "DMInfo_17_DMGosaP", typeof(string) },
            { "DMInfo_17_DMGosaM", typeof(string) },
            
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