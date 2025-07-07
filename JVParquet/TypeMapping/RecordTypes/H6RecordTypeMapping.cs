using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// H6レコード（票数（3連単））の型マッピング定義
    /// </summary>
    public class H6RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "H6";

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

            // 基本情報
            { "TorokuTosu", typeof(int) },           // 登録頭数
            { "SyussoTosu", typeof(int) },           // 出走頭数
            { "HatubaiFlag", typeof(string) },       // 発売フラグ

            // 返還馬番情報（簡略化）
            { "HenkanUma_0", typeof(string) },
            { "HenkanUma_1", typeof(string) },
            { "HenkanUma_2", typeof(string) },

            // 票数合計
            { "HyoTotal_0", typeof(int) },       // 票数合計[0]
            { "HyoTotal_1", typeof(int) }        // 票数合計[1]

            // 3連単票数情報は膨大なため省略（必要に応じて追加）
        };
    }
}