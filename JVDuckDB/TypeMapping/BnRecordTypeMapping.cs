namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// BNレコード（馬主マスタ）の型マッピング
    /// </summary>
    public class BnRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "BN";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 馬主情報
            { "BanusiCode", typeof(string) },
            { "BanusiName_Co", typeof(string) },
            { "BanusiName", typeof(string) },
            { "BanusiNameKana", typeof(string) },
            { "BanusiNameEng", typeof(string) },
            { "Fukusyoku", typeof(string) },
            
            // 本年成績情報（HonRuikei[0]）
            { "HonRuikei_0_SetYear", typeof(string) },
            { "HonRuikei_0_HonSyokinTotal", typeof(long) },
            { "HonRuikei_0_FukaSyokin", typeof(long) },
            { "HonRuikei_0_ChakuKaisu_0", typeof(int) },
            { "HonRuikei_0_ChakuKaisu_1", typeof(int) },
            { "HonRuikei_0_ChakuKaisu_2", typeof(int) },
            { "HonRuikei_0_ChakuKaisu_3", typeof(int) },
            { "HonRuikei_0_ChakuKaisu_4", typeof(int) },
            { "HonRuikei_0_ChakuKaisu_5", typeof(int) },
            
            // 累計成績情報（HonRuikei[1]）
            { "HonRuikei_1_SetYear", typeof(string) },
            { "HonRuikei_1_HonSyokinTotal", typeof(long) },
            { "HonRuikei_1_FukaSyokin", typeof(long) },
            { "HonRuikei_1_ChakuKaisu_0", typeof(int) },
            { "HonRuikei_1_ChakuKaisu_1", typeof(int) },
            { "HonRuikei_1_ChakuKaisu_2", typeof(int) },
            { "HonRuikei_1_ChakuKaisu_3", typeof(int) },
            { "HonRuikei_1_ChakuKaisu_4", typeof(int) },
            { "HonRuikei_1_ChakuKaisu_5", typeof(int) },
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(string) },
            { "head_MakeDate_Month", typeof(string) },
            { "head_MakeDate_Day", typeof(string) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "BanusiCode"
        };
    }
}