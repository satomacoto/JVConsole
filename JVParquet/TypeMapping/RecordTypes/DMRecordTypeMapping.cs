using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// DMレコード（タイム型データマイニング予想）の型マッピング定義
    /// </summary>
    public class DMRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "DM";

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

            // マイニング予想情報（18頭分の配列を展開）
            // 簡略化のため、最初の数頭分のみ定義
            { "DMInfo_0__Umaban", typeof(string) },
            { "DMInfo_0__DMTime", typeof(int) },
            { "DMInfo_0__DMGosaP", typeof(int) },
            { "DMInfo_0__DMGosaM", typeof(int) },

            { "DMInfo_1__Umaban", typeof(string) },
            { "DMInfo_1__DMTime", typeof(int) },
            { "DMInfo_1__DMGosaP", typeof(int) },
            { "DMInfo_1__DMGosaM", typeof(int) },

            { "DMInfo_2__Umaban", typeof(string) },
            { "DMInfo_2__DMTime", typeof(int) },
            { "DMInfo_2__DMGosaP", typeof(int) },
            { "DMInfo_2__DMGosaM", typeof(int) }
        };
    }
}