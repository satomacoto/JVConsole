using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// CCレコード（コース変更）の型マッピング定義
    /// </summary>
    public class CCRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CC";

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
            { "HappyoTime_Month", typeof(int) },
            { "HappyoTime_Day", typeof(int) },
            { "HappyoTime_Hour", typeof(int) },
            { "HappyoTime_Minute", typeof(int) },

            // 変更後情報
            { "CCInfoAfter_Kyori", typeof(int) },        // 距離（メートル）
            { "CCInfoAfter_TruckCd", typeof(string) },    // トラックコード

            // 変更前情報
            { "CCInfoBefore_Kyori", typeof(int) },       // 距離（メートル）
            { "CCInfoBefore_TruckCd", typeof(string) },   // トラックコード

            // 事由
            { "JiyuCd", typeof(string) }                  // 事由区分
        };
    }
}