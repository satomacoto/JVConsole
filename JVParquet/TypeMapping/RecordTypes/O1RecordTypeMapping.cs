using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// O1レコード（オッズ（単複枠））の型マッピング定義
    /// </summary>
    public class O1RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O1";

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
            { "TansyoFlag", typeof(string) },           // 発売フラグ 単勝
            { "FukusyoFlag", typeof(string) },          // 発売フラグ 複勝
            { "WakurenFlag", typeof(string) },          // 発売フラグ 枠連
            { "FukuChakuBaraiKey", typeof(string) },    // 複勝着払キー

            // オッズ情報（最初の数頭分のみ定義）
            // 単勝オッズ
            { "OddsTansyoInfo_0__Umaban", typeof(string) },
            { "OddsTansyoInfo_0__Odds", typeof(int) },
            { "OddsTansyoInfo_0__Ninki", typeof(int) },
            
            { "OddsTansyoInfo_1__Umaban", typeof(string) },
            { "OddsTansyoInfo_1__Odds", typeof(int) },
            { "OddsTansyoInfo_1__Ninki", typeof(int) },

            // 複勝オッズ
            { "OddsFukusyoInfo_0__Umaban", typeof(string) },
            { "OddsFukusyoInfo_0__OddsLow", typeof(int) },
            { "OddsFukusyoInfo_0__OddsHigh", typeof(int) },
            { "OddsFukusyoInfo_0__Ninki", typeof(int) },
            
            { "OddsFukusyoInfo_1__Umaban", typeof(string) },
            { "OddsFukusyoInfo_1__OddsLow", typeof(int) },
            { "OddsFukusyoInfo_1__OddsHigh", typeof(int) },
            { "OddsFukusyoInfo_1__Ninki", typeof(int) },

            // 枠連オッズ
            { "OddsWakurenInfo_0__Kumi", typeof(string) },
            { "OddsWakurenInfo_0__Odds", typeof(int) },
            { "OddsWakurenInfo_0__Ninki", typeof(int) },

            // 票数合計
            { "TotalHyosuTansyo", typeof(int) },    // 単勝票数合計
            { "TotalHyosuFukusyo", typeof(int) },   // 複勝票数合計
            { "TotalHyosuWakuren", typeof(int) }    // 枠連票数合計
        };
    }
}