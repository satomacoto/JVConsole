namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// TKレコード（特別登録馬）の型マッピング
    /// </summary>
    public class TkRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "TK";
        
        public override Dictionary<string, Type> FieldTypeMappings 
        {
            get
            {
                var mappings = new Dictionary<string, Type>
                {
                    // レース識別情報
                    { "id_Year", typeof(string) },
                    { "id_MonthDay", typeof(string) },
                    { "id_JyoCD", typeof(string) },
                    { "id_Kaiji", typeof(string) },
                    { "id_Nichiji", typeof(string) },
                    { "id_RaceNum", typeof(string) },
                    
                    // レース情報（構造体を展開）
                    { "RaceInfo_YoubiCD", typeof(string) },
                    { "RaceInfo_TokuNum", typeof(string) },
                    { "RaceInfo_Hondai", typeof(string) },
                    { "RaceInfo_Fukudai", typeof(string) },
                    { "RaceInfo_Kakko", typeof(string) },
                    { "RaceInfo_HondaiEng", typeof(string) },
                    { "RaceInfo_FukudaiEng", typeof(string) },
                    { "RaceInfo_KakkoEng", typeof(string) },
                    { "RaceInfo_Ryakusyo10", typeof(string) },
                    { "RaceInfo_Ryakusyo6", typeof(string) },
                    { "RaceInfo_Ryakusyo3", typeof(string) },
                    { "RaceInfo_Kubun", typeof(string) },
                    { "RaceInfo_Nkai", typeof(string) },
                    
                    // グレード・条件
                    { "GradeCD", typeof(string) },
                    
                    // 競走条件（構造体を展開）
                    { "JyokenInfo_SyubetuCD", typeof(string) },
                    { "JyokenInfo_KigoCD", typeof(string) },
                    { "JyokenInfo_JyuryoCD", typeof(string) },
                    { "JyokenInfo_JyokenCD_0", typeof(string) },
                    { "JyokenInfo_JyokenCD_1", typeof(string) },
                    { "JyokenInfo_JyokenCD_2", typeof(string) },
                    { "JyokenInfo_JyokenCD_3", typeof(string) },
                    { "JyokenInfo_JyokenCD_4", typeof(string) },
                    
                    // コース情報
                    { "Kyori", typeof(int) },
                    { "TrackCD", typeof(string) },
                    { "CourseKubunCD", typeof(string) },
                    
                    // ハンデ発表日
                    { "HandiDate_Year", typeof(string) },
                    { "HandiDate_Month", typeof(string) },
                    { "HandiDate_Day", typeof(string) },
                    
                    // 登録頭数
                    { "TorokuTosu", typeof(int) },
                    
                    // ヘッダー情報
                    { "head_RecordSpec", typeof(string) },
                    { "head_DataKubun", typeof(string) },
                    { "head_MakeDate_Year", typeof(string) },
                    { "head_MakeDate_Month", typeof(string) },
                    { "head_MakeDate_Day", typeof(string) }
                };
                
                // 300頭分の登録馬情報を展開（TokuUmaInfo[300]）
                for (int i = 0; i < 300; i++)
                {
                    mappings.Add($"TokuUmaInfo_{i}_Num", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_KettoNum", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_Bamei", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_UmaKigoCD", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_SexCD", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_TozaiCD", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_ChokyosiCode", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_ChokyosiRyakusyo", typeof(string));
                    mappings.Add($"TokuUmaInfo_{i}_Futan", typeof(int));  // 負担重量は数値型
                    mappings.Add($"TokuUmaInfo_{i}_Koryu", typeof(string));
                }
                
                return mappings;
            }
        }
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}