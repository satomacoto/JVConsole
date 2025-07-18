namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// RAレコード（レース詳細）の型マッピング
    /// </summary>
    public class RaRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "RA";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // レース基本情報
            { "GradeCD", typeof(string) },
            { "GradeCDBefore", typeof(string) },
            { "JyokenName", typeof(string) },
            { "Kyori", typeof(int) },
            { "KyoriBefore", typeof(int) },
            { "TrackCD", typeof(string) },
            { "TrackCDBefore", typeof(string) },
            { "CourseKubunCD", typeof(string) },
            { "CourseKubunCDBefore", typeof(string) },
            { "TorokuTosu", typeof(int) },
            { "SyussoTosu", typeof(int) },
            { "NyusenTosu", typeof(int) },
            { "RecordUpKubun", typeof(string) },
            
            // 時間情報
            { "HassoTime", typeof(string) },
            { "HassoTimeBefore", typeof(string) },
            { "HaronTimeS3", typeof(int) },
            { "HaronTimeS4", typeof(int) },
            { "HaronTimeL3", typeof(int) },
            { "HaronTimeL4", typeof(int) },
            { "SyogaiMileTime", typeof(int) },
            
            // 本賞金配列（0-based index）
            { "Honsyokin_0", typeof(int) },
            { "Honsyokin_1", typeof(int) },
            { "Honsyokin_2", typeof(int) },
            { "Honsyokin_3", typeof(int) },
            { "Honsyokin_4", typeof(int) },
            { "Honsyokin_5", typeof(int) },
            { "Honsyokin_6", typeof(int) },
            
            // 変更前本賞金配列（0-based index）
            { "HonsyokinBefore_0", typeof(int) },
            { "HonsyokinBefore_1", typeof(int) },
            { "HonsyokinBefore_2", typeof(int) },
            { "HonsyokinBefore_3", typeof(int) },
            { "HonsyokinBefore_4", typeof(int) },
            
            // 付加賞金配列（0-based index）
            { "Fukasyokin_0", typeof(int) },
            { "Fukasyokin_1", typeof(int) },
            { "Fukasyokin_2", typeof(int) },
            { "Fukasyokin_3", typeof(int) },
            { "Fukasyokin_4", typeof(int) },
            
            // 変更前付加賞金配列（0-based index）
            { "FukasyokinBefore_0", typeof(int) },
            { "FukasyokinBefore_1", typeof(int) },
            { "FukasyokinBefore_2", typeof(int) },
            
            // ラップタイム配列（0-based index）
            { "LapTime_0", typeof(int) },
            { "LapTime_1", typeof(int) },
            { "LapTime_2", typeof(int) },
            { "LapTime_3", typeof(int) },
            { "LapTime_4", typeof(int) },
            { "LapTime_5", typeof(int) },
            { "LapTime_6", typeof(int) },
            { "LapTime_7", typeof(int) },
            { "LapTime_8", typeof(int) },
            { "LapTime_9", typeof(int) },
            { "LapTime_10", typeof(int) },
            { "LapTime_11", typeof(int) },
            { "LapTime_12", typeof(int) },
            { "LapTime_13", typeof(int) },
            { "LapTime_14", typeof(int) },
            { "LapTime_15", typeof(int) },
            { "LapTime_16", typeof(int) },
            { "LapTime_17", typeof(int) },
            { "LapTime_18", typeof(int) },
            { "LapTime_19", typeof(int) },
            { "LapTime_20", typeof(int) },
            { "LapTime_21", typeof(int) },
            { "LapTime_22", typeof(int) },
            { "LapTime_23", typeof(int) },
            { "LapTime_24", typeof(int) },
            
            // レース情報（RaceInfo構造体）
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
            
            // 条件情報（JyokenInfo構造体）
            { "JyokenInfo_SyubetuCD", typeof(string) },
            { "JyokenInfo_KigoCD", typeof(string) },
            { "JyokenInfo_JyuryoCD", typeof(string) },
            { "JyokenInfo_JyokenCD_0", typeof(string) },
            { "JyokenInfo_JyokenCD_1", typeof(string) },
            { "JyokenInfo_JyokenCD_2", typeof(string) },
            { "JyokenInfo_JyokenCD_3", typeof(string) },
            { "JyokenInfo_JyokenCD_4", typeof(string) },
            
            // 天候馬場情報（TenkoBaba構造体）
            { "TenkoBaba_TenkoCD", typeof(string) },
            { "TenkoBaba_SibaBabaCD", typeof(string) },
            { "TenkoBaba_DirtBabaCD", typeof(string) },
            
            // コーナー情報（CornerInfo構造体配列[4]） - 適切に展開
            // Corner1 (CornerInfo[0])
            { "CornerInfo_0_Corner", typeof(string) },
            { "CornerInfo_0_Syukaisu", typeof(string) },
            { "CornerInfo_0_Jyuni", typeof(string) },
            
            // Corner2 (CornerInfo[1])
            { "CornerInfo_1_Corner", typeof(string) },
            { "CornerInfo_1_Syukaisu", typeof(string) },
            { "CornerInfo_1_Jyuni", typeof(string) },
            
            // Corner3 (CornerInfo[2])
            { "CornerInfo_2_Corner", typeof(string) },
            { "CornerInfo_2_Syukaisu", typeof(string) },
            { "CornerInfo_2_Jyuni", typeof(string) },
            
            // Corner4 (CornerInfo[3])
            { "CornerInfo_3_Corner", typeof(string) },
            { "CornerInfo_3_Syukaisu", typeof(string) },
            { "CornerInfo_3_Jyuni", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 拡張フィールド
            { "race_id", typeof(string) },
            { "race_datetime", typeof(DateTime) },
            { "track_type", typeof(int) },
            { "jyoken_win", typeof(bool) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}