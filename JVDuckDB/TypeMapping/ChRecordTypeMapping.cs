namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// CHレコード（調教師マスタ）の型マッピング
    /// </summary>
    public class ChRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CH";
        
        private static readonly Dictionary<string, Type> _fieldTypeMappings = new Dictionary<string, Type>
        {
            // 調教師情報
            { "ChokyosiCode", typeof(string) },
            { "DelKubun", typeof(string) },
            { "IssueDate_Year", typeof(string) },
            { "IssueDate_Month", typeof(string) },
            { "IssueDate_Day", typeof(string) },
            { "DelDate_Year", typeof(string) },
            { "DelDate_Month", typeof(string) },
            { "DelDate_Day", typeof(string) },
            { "BirthDate_Year", typeof(string) },
            { "BirthDate_Month", typeof(string) },
            { "BirthDate_Day", typeof(string) },
            { "ChokyosiName", typeof(string) },
            { "ChokyosiNameKana", typeof(string) },
            { "ChokyosiRyakusyo", typeof(string) },
            { "ChokyosiNameEng", typeof(string) },
            { "SexCD", typeof(string) },
            { "TozaiCD", typeof(string) },
            { "Syotai", typeof(string) },
            
            // 最近重賞勝利情報[3]を展開
            { "SaikinJyusyo_0_SaikinJyusyoid_Year", typeof(string) },
            { "SaikinJyusyo_0_SaikinJyusyoid_MonthDay", typeof(string) },
            { "SaikinJyusyo_0_SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_0_SaikinJyusyoid_Kaiji", typeof(string) },
            { "SaikinJyusyo_0_SaikinJyusyoid_Nichiji", typeof(string) },
            { "SaikinJyusyo_0_SaikinJyusyoid_RaceNum", typeof(string) },
            { "SaikinJyusyo_0_Hondai", typeof(string) },
            { "SaikinJyusyo_0_Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_0_Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_0_Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_0_GradeCD", typeof(string) },
            { "SaikinJyusyo_0_SyussoTosu", typeof(int) },
            { "SaikinJyusyo_0_KettoNum", typeof(string) },
            { "SaikinJyusyo_0_Bamei", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_Year", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_MonthDay", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_Kaiji", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_Nichiji", typeof(string) },
            { "SaikinJyusyo_1_SaikinJyusyoid_RaceNum", typeof(string) },
            { "SaikinJyusyo_1_Hondai", typeof(string) },
            { "SaikinJyusyo_1_Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_1_Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_1_Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_1_GradeCD", typeof(string) },
            { "SaikinJyusyo_1_SyussoTosu", typeof(int) },
            { "SaikinJyusyo_1_KettoNum", typeof(string) },
            { "SaikinJyusyo_1_Bamei", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_Year", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_MonthDay", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_Kaiji", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_Nichiji", typeof(string) },
            { "SaikinJyusyo_2_SaikinJyusyoid_RaceNum", typeof(string) },
            { "SaikinJyusyo_2_Hondai", typeof(string) },
            { "SaikinJyusyo_2_Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_2_Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_2_Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_2_GradeCD", typeof(string) },
            { "SaikinJyusyo_2_SyussoTosu", typeof(int) },
            { "SaikinJyusyo_2_KettoNum", typeof(string) },
            { "SaikinJyusyo_2_Bamei", typeof(string) },
        };
        
        static ChRecordTypeMapping()
        {
            var mappings = _fieldTypeMappings;
            
            // 本年・前年・累計成績情報[3]を展開
            for (int i = 0; i < 3; i++)
            {
                string prefix = $"HonZenRuikei_{i}";
                mappings.Add($"{prefix}_SetYear", typeof(string));
                mappings.Add($"{prefix}_HonSyokinHeichi", typeof(long));
                mappings.Add($"{prefix}_HonSyokinSyogai", typeof(long));
                mappings.Add($"{prefix}_FukaSyokinHeichi", typeof(long));
                mappings.Add($"{prefix}_FukaSyokinSyogai", typeof(long));
                
                // 平地着回数
                for (int j = 0; j < 6; j++)
                {
                    mappings.Add($"{prefix}_ChakuKaisuHeichi_ChakuKaisu_{j}", typeof(int));
                }
                
                // 障害着回数
                for (int j = 0; j < 6; j++)
                {
                    mappings.Add($"{prefix}_ChakuKaisuSyogai_ChakuKaisu_{j}", typeof(int));
                }
                
                // 競馬場別着回数[20]
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        mappings.Add($"{prefix}_ChakuKaisuJyo_{j}_ChakuKaisu_{k}", typeof(int));
                    }
                }
                
                // 距離別着回数[6]
                for (int j = 0; j < 6; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        mappings.Add($"{prefix}_ChakuKaisuKyori_{j}_ChakuKaisu_{k}", typeof(int));
                    }
                }
            }
            
            // ヘッダー情報
            mappings.Add("head_RecordSpec", typeof(string));
            mappings.Add("head_DataKubun", typeof(string));
            mappings.Add("head_MakeDate_Year", typeof(string));
            mappings.Add("head_MakeDate_Month", typeof(string));
            mappings.Add("head_MakeDate_Day", typeof(string));
        }
        
        public override Dictionary<string, Type> FieldTypeMappings => _fieldTypeMappings;
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "ChokyosiCode"
        };
    }
}