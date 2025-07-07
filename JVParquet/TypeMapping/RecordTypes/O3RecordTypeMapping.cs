using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// O3レコード（オッズ（ワイド））の型マッピング定義
    /// </summary>
    public class O3RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O3";

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

            // 発表時刻
            { "HappyoTime_Hour", typeof(int) },
            { "HappyoTime_Minute", typeof(int) },

            // 基本情報
            { "TorokuTosu", typeof(int) },              // 登録頭数
            { "SyussoTosu", typeof(int) },              // 出走頭数
            { "WideFlag", typeof(string) },             // 発売フラグ ワイド

            // ワイドオッズ情報（最初の数組分のみ定義）
            { "OddsWideInfo_0__Kumi", typeof(string) },
            { "OddsWideInfo_0__OddsLow", typeof(int) },
            { "OddsWideInfo_0__OddsHigh", typeof(int) },
            { "OddsWideInfo_0__Ninki", typeof(int) },
            
            { "OddsWideInfo_1__Kumi", typeof(string) },
            { "OddsWideInfo_1__OddsLow", typeof(int) },
            { "OddsWideInfo_1__OddsHigh", typeof(int) },
            { "OddsWideInfo_1__Ninki", typeof(int) },
            
            { "OddsWideInfo_2__Kumi", typeof(string) },
            { "OddsWideInfo_2__OddsLow", typeof(int) },
            { "OddsWideInfo_2__OddsHigh", typeof(int) },
            { "OddsWideInfo_2__Ninki", typeof(int) },

            // 票数合計
            { "TotalHyosuWide", typeof(int) }       // ワイド票数合計
        };
    }
}