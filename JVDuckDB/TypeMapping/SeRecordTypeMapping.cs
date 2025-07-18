namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// SEレコード（出走馬）の型マッピング
    /// </summary>
    public class SeRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "SE";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // 馬基本情報
            { "Wakuban", typeof(int) },
            { "Umaban", typeof(int) },
            { "KettoNum", typeof(string) },
            { "Bamei", typeof(string) },
            { "UmaKigoCD", typeof(string) },
            { "SexCD", typeof(string) },
            { "TozaiCD", typeof(string) },
            { "Barei", typeof(int) },
            { "HinsyuCD", typeof(string) },
            { "KeiroCD", typeof(string) },
            
            // 負担重量・体重情報
            { "Futan", typeof(int) },
            { "FutanBefore", typeof(int) },
            { "Blinker", typeof(string) },
            { "BaTaijyu", typeof(int) },
            { "ZogenFugo", typeof(string) },
            { "ZogenSa", typeof(int) },
            
            // 騎手・調教師情報
            { "KisyuCode", typeof(string) },
            { "KisyuCodeBefore", typeof(string) },
            { "KisyuRyakusyo", typeof(string) },
            { "KisyuRyakusyoBefore", typeof(string) },
            { "MinaraiCD", typeof(string) },
            { "MinaraiCDBefore", typeof(string) },
            { "ChokyosiCode", typeof(string) },
            { "ChokyosiRyakusyo", typeof(string) },
            
            // 馬主情報
            { "BanusiCode", typeof(string) },
            { "BanusiName", typeof(string) },
            
            // 着順・タイム情報
            { "KakuteiJyuni", typeof(int) },
            { "NyusenJyuni", typeof(int) },
            { "DochakuKubun", typeof(string) },
            { "DochakuTosu", typeof(int) },
            { "Time", typeof(int) },
            { "ChakusaCD", typeof(string) },
            { "ChakusaCDP", typeof(string) },
            { "ChakusaCDPP", typeof(string) },
            { "Jyuni1c", typeof(int) },
            { "Jyuni2c", typeof(int) },
            { "Jyuni3c", typeof(int) },
            { "Jyuni4c", typeof(int) },
            
            // オッズ・人気
            { "Odds", typeof(int) },
            { "Ninki", typeof(int) },
            
            // 獲得賞金
            { "Honsyokin", typeof(int) },
            { "Fukasyokin", typeof(int) },
            
            // 客室・異常
            { "KyakusituKubun", typeof(string) },
            { "IJyoCD", typeof(string) },
            { "RecordUpKubun", typeof(string) },
            
            // ハロンタイム
            { "HaronTimeL3", typeof(int) },
            { "HaronTimeL4", typeof(int) },
            
            // タイム指数
            { "TimeDiff", typeof(string) },
            
            // DM（デマ）情報
            { "DMKubun", typeof(string) },
            { "DMTime", typeof(int) },
            { "DMJyuni", typeof(int) },
            { "DMGosaM", typeof(int) },
            { "DMGosaP", typeof(int) },
            
            // 服色情報
            { "Fukusyoku", typeof(string) },
            
            // 着順情報配列（ChakuUmaInfo構造体）[3]を展開
            { "ChakuUmaInfo_0_KettoNum", typeof(string) },
            { "ChakuUmaInfo_0_Bamei", typeof(string) },
            { "ChakuUmaInfo_1_KettoNum", typeof(string) },
            { "ChakuUmaInfo_1_Bamei", typeof(string) },
            { "ChakuUmaInfo_2_KettoNum", typeof(string) },
            { "ChakuUmaInfo_2_Bamei", typeof(string) },
            
            // 予約フィールド
            { "reserved1", typeof(string) },
            { "reserved2", typeof(string) },
            { "reserved3", typeof(string) },
            { "reserved4", typeof(string) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
            
            // 拡張フィールド
            { "race_id", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum", "Umaban" 
        };
    }
}