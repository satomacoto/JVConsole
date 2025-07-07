using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// O4レコード（オッズ（馬単））の型マッピング定義
    /// </summary>
    public class O4RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "O4";

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
            { "UmatanFlag", typeof(string) },           // 発売フラグ 馬単

            // 馬単オッズ情報（最初の数組分のみ定義）
            { "OddsUmatanInfo_0__Kumi", typeof(string) },
            { "OddsUmatanInfo_0__Odds", typeof(int) },
            { "OddsUmatanInfo_0__Ninki", typeof(int) },
            
            { "OddsUmatanInfo_1__Kumi", typeof(string) },
            { "OddsUmatanInfo_1__Odds", typeof(int) },
            { "OddsUmatanInfo_1__Ninki", typeof(int) },
            
            { "OddsUmatanInfo_2__Kumi", typeof(string) },
            { "OddsUmatanInfo_2__Odds", typeof(int) },
            { "OddsUmatanInfo_2__Ninki", typeof(int) },

            // 票数合計
            { "TotalHyosuUmatan", typeof(int) }     // 馬単票数合計
        };
    }
}