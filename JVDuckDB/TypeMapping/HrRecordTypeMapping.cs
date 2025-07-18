namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// HRレコード（払戻）の型マッピング
    /// </summary>
    public class HrRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HR";
        
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
            
            // 不成立フラグ配列（0-based）
            { "FuseirituFlag_0", typeof(string) },
            { "FuseirituFlag_1", typeof(string) },
            { "FuseirituFlag_2", typeof(string) },
            { "FuseirituFlag_3", typeof(string) },
            { "FuseirituFlag_4", typeof(string) },
            { "FuseirituFlag_5", typeof(string) },
            { "FuseirituFlag_6", typeof(string) },
            { "FuseirituFlag_7", typeof(string) },
            { "FuseirituFlag_8", typeof(string) },
            
            // 特払フラグ配列（0-based）
            { "TokubaraiFlag_0", typeof(string) },
            { "TokubaraiFlag_1", typeof(string) },
            { "TokubaraiFlag_2", typeof(string) },
            { "TokubaraiFlag_3", typeof(string) },
            { "TokubaraiFlag_4", typeof(string) },
            { "TokubaraiFlag_5", typeof(string) },
            { "TokubaraiFlag_6", typeof(string) },
            { "TokubaraiFlag_7", typeof(string) },
            { "TokubaraiFlag_8", typeof(string) },
            
            // 返還フラグ配列（0-based）
            { "HenkanFlag_0", typeof(string) },
            { "HenkanFlag_1", typeof(string) },
            { "HenkanFlag_2", typeof(string) },
            { "HenkanFlag_3", typeof(string) },
            { "HenkanFlag_4", typeof(string) },
            { "HenkanFlag_5", typeof(string) },
            { "HenkanFlag_6", typeof(string) },
            { "HenkanFlag_7", typeof(string) },
            { "HenkanFlag_8", typeof(string) },
            
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
            
            // 単勝払戻情報[3]を展開
            { "PayTansyo_0_Umaban", typeof(string) },
            { "PayTansyo_0_Pay", typeof(long) },
            { "PayTansyo_0_Ninki", typeof(string) },
            { "PayTansyo_1_Umaban", typeof(string) },
            { "PayTansyo_1_Pay", typeof(long) },
            { "PayTansyo_1_Ninki", typeof(string) },
            { "PayTansyo_2_Umaban", typeof(string) },
            { "PayTansyo_2_Pay", typeof(long) },
            { "PayTansyo_2_Ninki", typeof(string) },
            
            // 複勝払戻情報[5]を展開
            { "PayFukusyo_0_Umaban", typeof(string) },
            { "PayFukusyo_0_Pay", typeof(long) },
            { "PayFukusyo_0_Ninki", typeof(string) },
            { "PayFukusyo_1_Umaban", typeof(string) },
            { "PayFukusyo_1_Pay", typeof(long) },
            { "PayFukusyo_1_Ninki", typeof(string) },
            { "PayFukusyo_2_Umaban", typeof(string) },
            { "PayFukusyo_2_Pay", typeof(long) },
            { "PayFukusyo_2_Ninki", typeof(string) },
            { "PayFukusyo_3_Umaban", typeof(string) },
            { "PayFukusyo_3_Pay", typeof(long) },
            { "PayFukusyo_3_Ninki", typeof(string) },
            { "PayFukusyo_4_Umaban", typeof(string) },
            { "PayFukusyo_4_Pay", typeof(long) },
            { "PayFukusyo_4_Ninki", typeof(string) },
            
            // 枠連払戻情報[3]を展開
            { "PayWakuren_0_Umaban", typeof(string) },
            { "PayWakuren_0_Pay", typeof(long) },
            { "PayWakuren_0_Ninki", typeof(string) },
            { "PayWakuren_1_Umaban", typeof(string) },
            { "PayWakuren_1_Pay", typeof(long) },
            { "PayWakuren_1_Ninki", typeof(string) },
            { "PayWakuren_2_Umaban", typeof(string) },
            { "PayWakuren_2_Pay", typeof(long) },
            { "PayWakuren_2_Ninki", typeof(string) },
            
            // 馬連払戻情報[3]を展開
            { "PayUmaren_0_Kumi", typeof(string) },
            { "PayUmaren_0_Pay", typeof(long) },
            { "PayUmaren_0_Ninki", typeof(string) },
            { "PayUmaren_1_Kumi", typeof(string) },
            { "PayUmaren_1_Pay", typeof(long) },
            { "PayUmaren_1_Ninki", typeof(string) },
            { "PayUmaren_2_Kumi", typeof(string) },
            { "PayUmaren_2_Pay", typeof(long) },
            { "PayUmaren_2_Ninki", typeof(string) },
            
            // ワイド払戻情報[7]を展開
            { "PayWide_0_Kumi", typeof(string) },
            { "PayWide_0_Pay", typeof(long) },
            { "PayWide_0_Ninki", typeof(string) },
            { "PayWide_1_Kumi", typeof(string) },
            { "PayWide_1_Pay", typeof(long) },
            { "PayWide_1_Ninki", typeof(string) },
            { "PayWide_2_Kumi", typeof(string) },
            { "PayWide_2_Pay", typeof(long) },
            { "PayWide_2_Ninki", typeof(string) },
            { "PayWide_3_Kumi", typeof(string) },
            { "PayWide_3_Pay", typeof(long) },
            { "PayWide_3_Ninki", typeof(string) },
            { "PayWide_4_Kumi", typeof(string) },
            { "PayWide_4_Pay", typeof(long) },
            { "PayWide_4_Ninki", typeof(string) },
            { "PayWide_5_Kumi", typeof(string) },
            { "PayWide_5_Pay", typeof(long) },
            { "PayWide_5_Ninki", typeof(string) },
            { "PayWide_6_Kumi", typeof(string) },
            { "PayWide_6_Pay", typeof(long) },
            { "PayWide_6_Ninki", typeof(string) },
            
            // 予備払戻情報[3]を展開
            { "PayReserved1_0_Kumi", typeof(string) },
            { "PayReserved1_0_Pay", typeof(long) },
            { "PayReserved1_0_Ninki", typeof(string) },
            { "PayReserved1_1_Kumi", typeof(string) },
            { "PayReserved1_1_Pay", typeof(long) },
            { "PayReserved1_1_Ninki", typeof(string) },
            { "PayReserved1_2_Kumi", typeof(string) },
            { "PayReserved1_2_Pay", typeof(long) },
            { "PayReserved1_2_Ninki", typeof(string) },
            
            // 馬単払戻情報[6]を展開
            { "PayUmatan_0_Kumi", typeof(string) },
            { "PayUmatan_0_Pay", typeof(long) },
            { "PayUmatan_0_Ninki", typeof(string) },
            { "PayUmatan_1_Kumi", typeof(string) },
            { "PayUmatan_1_Pay", typeof(long) },
            { "PayUmatan_1_Ninki", typeof(string) },
            { "PayUmatan_2_Kumi", typeof(string) },
            { "PayUmatan_2_Pay", typeof(long) },
            { "PayUmatan_2_Ninki", typeof(string) },
            { "PayUmatan_3_Kumi", typeof(string) },
            { "PayUmatan_3_Pay", typeof(long) },
            { "PayUmatan_3_Ninki", typeof(string) },
            { "PayUmatan_4_Kumi", typeof(string) },
            { "PayUmatan_4_Pay", typeof(long) },
            { "PayUmatan_4_Ninki", typeof(string) },
            { "PayUmatan_5_Kumi", typeof(string) },
            { "PayUmatan_5_Pay", typeof(long) },
            { "PayUmatan_5_Ninki", typeof(string) },
            
            // 3連複払戻情報[3]を展開
            { "PaySanrenpuku_0_Kumi", typeof(string) },
            { "PaySanrenpuku_0_Pay", typeof(long) },
            { "PaySanrenpuku_0_Ninki", typeof(string) },
            { "PaySanrenpuku_1_Kumi", typeof(string) },
            { "PaySanrenpuku_1_Pay", typeof(long) },
            { "PaySanrenpuku_1_Ninki", typeof(string) },
            { "PaySanrenpuku_2_Kumi", typeof(string) },
            { "PaySanrenpuku_2_Pay", typeof(long) },
            { "PaySanrenpuku_2_Ninki", typeof(string) },
            
            // 3連単払戻情報[6]を展開
            { "PaySanrentan_0_Kumi", typeof(string) },
            { "PaySanrentan_0_Pay", typeof(long) },
            { "PaySanrentan_0_Ninki", typeof(string) },
            { "PaySanrentan_1_Kumi", typeof(string) },
            { "PaySanrentan_1_Pay", typeof(long) },
            { "PaySanrentan_1_Ninki", typeof(string) },
            { "PaySanrentan_2_Kumi", typeof(string) },
            { "PaySanrentan_2_Pay", typeof(long) },
            { "PaySanrentan_2_Ninki", typeof(string) },
            { "PaySanrentan_3_Kumi", typeof(string) },
            { "PaySanrentan_3_Pay", typeof(long) },
            { "PaySanrentan_3_Ninki", typeof(string) },
            { "PaySanrentan_4_Kumi", typeof(string) },
            { "PaySanrentan_4_Pay", typeof(long) },
            { "PaySanrentan_4_Ninki", typeof(string) },
            { "PaySanrentan_5_Kumi", typeof(string) },
            { "PaySanrentan_5_Pay", typeof(long) },
            { "PaySanrentan_5_Ninki", typeof(string) },
            
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
            "id_Year", "id_MonthDay", "id_JyoCD", "id_Kaiji", "id_Nichiji", "id_RaceNum" 
        };
    }
}