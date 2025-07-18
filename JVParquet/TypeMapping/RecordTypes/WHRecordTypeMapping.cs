using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// WHレコード（WIN5払戻）の型マッピング定義
    /// </summary>
    public class WHRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WH";

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

            // WIN5対象レース情報（最初の数レース分のみ定義）
            { "WHInfo_0__RaceNum", typeof(int) },
            { "WHInfo_0__YoubiCD", typeof(string) },
            { "WHInfo_0__WinUmaban", typeof(string) },
            { "WHInfo_0__reserved", typeof(string) },

            { "WHInfo_1__RaceNum", typeof(int) },
            { "WHInfo_1__YoubiCD", typeof(string) },
            { "WHInfo_1__WinUmaban", typeof(string) },
            { "WHInfo_1__reserved", typeof(string) },

            { "WHInfo_2__RaceNum", typeof(int) },
            { "WHInfo_2__YoubiCD", typeof(string) },
            { "WHInfo_2__WinUmaban", typeof(string) },
            { "WHInfo_2__reserved", typeof(string) },

            // 払戻情報
            { "Pay", typeof(int) },                 // 払戻金
            { "TekichuHyo", typeof(int) },              // 的中票数
            { "reserved", typeof(string) }              // 予備
        };
    }
}