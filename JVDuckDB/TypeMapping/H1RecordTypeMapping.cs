namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// H1レコード（票数（全掛））の型マッピング
    /// </summary>
    public class H1RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "H1";
        
        private static readonly Dictionary<string, Type> _fieldTypeMappings = new Dictionary<string, Type>
        {
            // レース識別情報
            { "id_Year", typeof(string) },
            { "id_MonthDay", typeof(string) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(string) },
            { "id_Nichiji", typeof(string) },
            { "id_RaceNum", typeof(string) },
            
            // 登録・出走頭数
            { "TorokuTosu", typeof(int) },
            { "SyussoTosu", typeof(int) },
            
            // 発売フラグ配列（0-based）
            { "HatubaiFlag_0", typeof(string) },
            { "HatubaiFlag_1", typeof(string) },
            { "HatubaiFlag_2", typeof(string) },
            { "HatubaiFlag_3", typeof(string) },
            { "HatubaiFlag_4", typeof(string) },
            { "HatubaiFlag_5", typeof(string) },
            { "HatubaiFlag_6", typeof(string) },
            
            // 複勝着払いキー
            { "FukuChakuBaraiKey", typeof(string) },
            
            // 返還馬番配列（0-based、28個）
            { "HenkanUma_0", typeof(int) },
            { "HenkanUma_1", typeof(int) },
            { "HenkanUma_2", typeof(int) },
            { "HenkanUma_3", typeof(int) },
            { "HenkanUma_4", typeof(int) },
            { "HenkanUma_5", typeof(int) },
            { "HenkanUma_6", typeof(int) },
            { "HenkanUma_7", typeof(int) },
            { "HenkanUma_8", typeof(int) },
            { "HenkanUma_9", typeof(int) },
            { "HenkanUma_10", typeof(int) },
            { "HenkanUma_11", typeof(int) },
            { "HenkanUma_12", typeof(int) },
            { "HenkanUma_13", typeof(int) },
            { "HenkanUma_14", typeof(int) },
            { "HenkanUma_15", typeof(int) },
            { "HenkanUma_16", typeof(int) },
            { "HenkanUma_17", typeof(int) },
            { "HenkanUma_18", typeof(int) },
            { "HenkanUma_19", typeof(int) },
            { "HenkanUma_20", typeof(int) },
            { "HenkanUma_21", typeof(int) },
            { "HenkanUma_22", typeof(int) },
            { "HenkanUma_23", typeof(int) },
            { "HenkanUma_24", typeof(int) },
            { "HenkanUma_25", typeof(int) },
            { "HenkanUma_26", typeof(int) },
            { "HenkanUma_27", typeof(int) },
            
            // 返還枠番配列（0-based）
            { "HenkanWaku_0", typeof(int) },
            { "HenkanWaku_1", typeof(int) },
            { "HenkanWaku_2", typeof(int) },
            { "HenkanWaku_3", typeof(int) },
            { "HenkanWaku_4", typeof(int) },
            { "HenkanWaku_5", typeof(int) },
            { "HenkanWaku_6", typeof(int) },
            { "HenkanWaku_7", typeof(int) },
            
            // 返還同枠配列（0-based）
            { "HenkanDoWaku_0", typeof(int) },
            { "HenkanDoWaku_1", typeof(int) },
            { "HenkanDoWaku_2", typeof(int) },
            { "HenkanDoWaku_3", typeof(int) },
            { "HenkanDoWaku_4", typeof(int) },
            { "HenkanDoWaku_5", typeof(int) },
            { "HenkanDoWaku_6", typeof(int) },
            { "HenkanDoWaku_7", typeof(int) },
        };

        static H1RecordTypeMapping()
        {
            var mappings = _fieldTypeMappings;
            
            // 単勝票数配列[28]を展開
            for (int i = 0; i < 28; i++)
            {
                mappings.Add($"HyoTansyo_{i}_Umaban", typeof(string));
                mappings.Add($"HyoTansyo_{i}_Hyo", typeof(long));
                mappings.Add($"HyoTansyo_{i}_Ninki", typeof(string));
            }
            
            // 複勝票数配列[28]を展開
            for (int i = 0; i < 28; i++)
            {
                mappings.Add($"HyoFukusyo_{i}_Umaban", typeof(string));
                mappings.Add($"HyoFukusyo_{i}_Hyo", typeof(long));
                mappings.Add($"HyoFukusyo_{i}_Ninki", typeof(string));
            }
            
            // 枠連票数配列[36]を展開
            for (int i = 0; i < 36; i++)
            {
                mappings.Add($"HyoWakuren_{i}_Kumi", typeof(string));
                mappings.Add($"HyoWakuren_{i}_Hyo", typeof(long));
                mappings.Add($"HyoWakuren_{i}_Ninki", typeof(string));
            }
            
            // 馬連票数配列[153]を展開
            for (int i = 0; i < 153; i++)
            {
                mappings.Add($"HyoUmaren_{i}_Kumi", typeof(string));
                mappings.Add($"HyoUmaren_{i}_Hyo", typeof(long));
                mappings.Add($"HyoUmaren_{i}_Ninki", typeof(string));
            }
            
            // ワイド票数配列[153]を展開
            for (int i = 0; i < 153; i++)
            {
                mappings.Add($"HyoWide_{i}_Kumi", typeof(string));
                mappings.Add($"HyoWide_{i}_Hyo", typeof(long));
                mappings.Add($"HyoWide_{i}_Ninki", typeof(string));
            }
            
            // 馬単票数配列[306]を展開
            for (int i = 0; i < 306; i++)
            {
                mappings.Add($"HyoUmatan_{i}_Kumi", typeof(string));
                mappings.Add($"HyoUmatan_{i}_Hyo", typeof(long));
                mappings.Add($"HyoUmatan_{i}_Ninki", typeof(string));
            }
            
            // 3連複票数配列[816]を展開
            for (int i = 0; i < 816; i++)
            {
                mappings.Add($"HyoSanrenpuku_{i}_Kumi", typeof(string));
                mappings.Add($"HyoSanrenpuku_{i}_Hyo", typeof(long));
                mappings.Add($"HyoSanrenpuku_{i}_Ninki", typeof(string));
            }
            
            // 3連単票数配列[4896]を展開
            for (int i = 0; i < 4896; i++)
            {
                mappings.Add($"HyoSanrentan_{i}_Kumi", typeof(string));
                mappings.Add($"HyoSanrentan_{i}_Hyo", typeof(long));
                mappings.Add($"HyoSanrentan_{i}_Ninki", typeof(string));
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
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}