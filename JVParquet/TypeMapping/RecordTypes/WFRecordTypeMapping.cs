using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// WFレコード（WIN5）の型マッピング定義
    /// </summary>
    public class WFRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WF";

        public override List<string> IndexColumns => new List<string>
        {
            "KaisaiDate_Year",
            "KaisaiDate_Month",
            "KaisaiDate_Day"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // WIN5情報
            { "KaisaiDate_Year", typeof(int) },         // 開催年月日（年）
            { "KaisaiDate_Month", typeof(int) },        // 開催年月日（月）
            { "KaisaiDate_Day", typeof(int) },          // 開催年月日（日）
            { "reserved", typeof(string) },             // 予備

            // WIN5レース情報（5レース分、簡略化のため最初の3レース分のみ定義）
            { "WFRaceInfo_0__JyoCD", typeof(string) },
            { "WFRaceInfo_0__Kaiji", typeof(int) },
            { "WFRaceInfo_0__Nichiji", typeof(int) },
            { "WFRaceInfo_0__RaceNum", typeof(int) },
            { "WFRaceInfo_0__YoubiCD", typeof(string) },
            { "WFRaceInfo_0__WinUmaban", typeof(string) },
            { "WFRaceInfo_0__reserved", typeof(string) },

            { "WFRaceInfo_1__JyoCD", typeof(string) },
            { "WFRaceInfo_1__Kaiji", typeof(int) },
            { "WFRaceInfo_1__Nichiji", typeof(int) },
            { "WFRaceInfo_1__RaceNum", typeof(int) },
            { "WFRaceInfo_1__YoubiCD", typeof(string) },
            { "WFRaceInfo_1__WinUmaban", typeof(string) },
            { "WFRaceInfo_1__reserved", typeof(string) },

            { "WFRaceInfo_2__JyoCD", typeof(string) },
            { "WFRaceInfo_2__Kaiji", typeof(int) },
            { "WFRaceInfo_2__Nichiji", typeof(int) },
            { "WFRaceInfo_2__RaceNum", typeof(int) },
            { "WFRaceInfo_2__YoubiCD", typeof(string) },
            { "WFRaceInfo_2__WinUmaban", typeof(string) },
            { "WFRaceInfo_2__reserved", typeof(string) },

            // 払戻情報
            { "PayoutInfo_Pay", typeof(int) },      // 払戻金
            { "PayoutInfo_Tekichuhyo", typeof(int) },   // 的中票数
            { "PayoutInfo_reserved", typeof(string) },

            // 発売情報
            { "SaleInfo_HatubaiFlag", typeof(string) }, // 発売フラグ
            { "SaleInfo_Syussoumei", typeof(string) },  // 出走取消頭数
            { "SaleInfo_Henkanumaban", typeof(string) },// 返還馬番
            { "SaleInfo_reserved", typeof(string) },
            { "SaleInfo_Carryhyosu", typeof(int) },     // キャリーオーバー票数
            { "SaleInfo_Totalhyosu", typeof(int) },     // 発売票数合計
            { "SaleInfo_reserved2", typeof(string) }
        };
    }
}