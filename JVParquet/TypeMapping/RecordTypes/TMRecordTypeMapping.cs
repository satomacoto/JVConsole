using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// TMレコード（対戦型データマイニング予想）の型マッピング定義
    /// </summary>
    public class TMRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "TM";

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

            // データ作成時分
            { "MakeHM_Hour", typeof(int) },
            { "MakeHM_Minute", typeof(int) },

            // 対戦型マイニング予想情報（18頭分の配列を展開、最初の数頭分のみ定義）
            { "TMInfo_0__Umaban", typeof(string) },
            { "TMInfo_0__TMScore", typeof(int) },

            { "TMInfo_1__Umaban", typeof(string) },
            { "TMInfo_1__TMScore", typeof(int) },

            { "TMInfo_2__Umaban", typeof(string) },
            { "TMInfo_2__TMScore", typeof(int) },

            { "TMInfo_3__Umaban", typeof(string) },
            { "TMInfo_3__TMScore", typeof(int) },

            { "TMInfo_4__Umaban", typeof(string) },
            { "TMInfo_4__TMScore", typeof(int) },

            { "TMInfo_5__Umaban", typeof(string) },
            { "TMInfo_5__TMScore", typeof(int) }
        };
    }
}