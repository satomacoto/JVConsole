using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// HRレコード（払戻）の型マッピング定義
    /// </summary>
    public class HRRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HR";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji",
            "id_RaceNum"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 競走識別情報
            { "id_Year", typeof(int) },
            { "id_MonthDay", typeof(int) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(int) },
            { "id_Nichiji", typeof(int) },
            { "id_RaceNum", typeof(int) },

            // 払戻情報（各賭式の最初の数個のみ定義）
            // 単勝払戻
            { "PayTansyo_0__Umaban", typeof(string) },
            { "PayTansyo_0__Pay", typeof(int) },
            { "PayTansyo_0__Ninki", typeof(int) },
            
            // 複勝払戻
            { "PayFukusyo_0__Umaban", typeof(string) },
            { "PayFukusyo_0__Pay", typeof(int) },
            { "PayFukusyo_0__Ninki", typeof(int) },
            
            // 枠連払戻
            { "PayWakuren_0__Kumi", typeof(string) },
            { "PayWakuren_0__Pay", typeof(int) },
            { "PayWakuren_0__Ninki", typeof(int) },
            
            // 馬連払戻
            { "PayUmaren_0__Kumi", typeof(string) },
            { "PayUmaren_0__Pay", typeof(int) },
            { "PayUmaren_0__Ninki", typeof(int) },
            
            // ワイド払戻
            { "PayWide_0__Kumi", typeof(string) },
            { "PayWide_0__Pay", typeof(int) },
            { "PayWide_0__Ninki", typeof(int) },
            
            // 馬単払戻
            { "PayUmatan_0__Kumi", typeof(string) },
            { "PayUmatan_0__Pay", typeof(int) },
            { "PayUmatan_0__Ninki", typeof(int) },
            
            // 3連複払戻
            { "PaySanrenpuku_0__Kumi", typeof(string) },
            { "PaySanrenpuku_0__Pay", typeof(int) },
            { "PaySanrenpuku_0__Ninki", typeof(int) },
            
            // 3連単払戻
            { "PaySanrentan_0__Kumi", typeof(string) },
            { "PaySanrentan_0__Pay", typeof(int) },
            { "PaySanrentan_0__Ninki", typeof(int) }
        };
    }
}