using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// O6レコード（オッズ（3連単））の型マッピング定義
    /// </summary>
    public class O6RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O6";

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
            { "SanrentanFlag", typeof(string) },        // 発売フラグ 3連単

            // 3連単オッズ情報（最初の数組分のみ定義）
            { "OddsSanrentanInfo_0__Kumi", typeof(string) },
            { "OddsSanrentanInfo_0__Odds", typeof(int) },
            { "OddsSanrentanInfo_0__Ninki", typeof(int) },
            
            { "OddsSanrentanInfo_1__Kumi", typeof(string) },
            { "OddsSanrentanInfo_1__Odds", typeof(int) },
            { "OddsSanrentanInfo_1__Ninki", typeof(int) },
            
            { "OddsSanrentanInfo_2__Kumi", typeof(string) },
            { "OddsSanrentanInfo_2__Odds", typeof(int) },
            { "OddsSanrentanInfo_2__Ninki", typeof(int) },

            // 票数合計
            { "TotalHyosuSanrentan", typeof(int) }  // 3連単票数合計
        };
    }
}