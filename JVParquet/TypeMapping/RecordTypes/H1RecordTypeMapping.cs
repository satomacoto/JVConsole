using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// H1レコード（票数（全掛け））の型マッピング定義
    /// </summary>
    public class H1RecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "H1";

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
            { "TorokuTosu", typeof(int) },              // 登録頭数
            { "SyussoTosu", typeof(int) },              // 出走頭数
            { "FukuChakuBaraiKey", typeof(string) },    // 複勝着払キー

            // 発売フラグ（7種類の賭式）
            { "HatubaiFlag_0", typeof(string) },  // 単勝
            { "HatubaiFlag_1", typeof(string) },  // 複勝
            { "HatubaiFlag_2", typeof(string) },  // 枠連
            { "HatubaiFlag_3", typeof(string) },  // 馬連
            { "HatubaiFlag_4", typeof(string) },  // ワイド
            { "HatubaiFlag_5", typeof(string) },  // 馬単
            { "HatubaiFlag_6", typeof(string) },  // 3連複

            // 返還情報（簡略化）
            { "HenkanUma_0", typeof(string) },   // 返還馬番情報[0]
            { "HenkanUma_1", typeof(string) },   // 返還馬番情報[1]
            { "HenkanWaku_0", typeof(string) },  // 返還枠番情報[0]
            { "HenkanWaku_1", typeof(string) },  // 返還枠番情報[1]
            { "HenkanDoWaku_0", typeof(string) }, // 返還同枠情報[0]
            { "HenkanDoWaku_1", typeof(string) }, // 返還同枠情報[1]

            // 票数合計（14種類）
            { "HyoTotal_0", typeof(int) },   // 単勝票数合計
            { "HyoTotal_1", typeof(int) },   // 複勝票数合計
            { "HyoTotal_2", typeof(int) },   // 枠連票数合計
            { "HyoTotal_3", typeof(int) },   // 馬連票数合計
            { "HyoTotal_4", typeof(int) },   // ワイド票数合計
            { "HyoTotal_5", typeof(int) },   // 馬単票数合計
            { "HyoTotal_6", typeof(int) }    // 3連複票数合計

            // 個別票数情報は膨大なため省略（必要に応じて追加）
        };
    }
}