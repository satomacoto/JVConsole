using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// TCレコード（特別登録馬）の型マッピング定義
    /// </summary>
    public class TCRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "TC";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji"
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

            // 特別登録馬情報（最初の数頭分のみ定義）
            { "TCInfo_0__Num", typeof(int) },
            { "TCInfo_0__KettoNum", typeof(string) },
            { "TCInfo_0__Bamei", typeof(string) },
            { "TCInfo_0__UmaKigoCD", typeof(string) },
            { "TCInfo_0__SexCD", typeof(string) },
            { "TCInfo_0__TozaiCD", typeof(string) },
            { "TCInfo_0__ChokyosiCode", typeof(string) },
            { "TCInfo_0__ChokyosiRyakusyo", typeof(string) },
            { "TCInfo_0__Futan", typeof(int) },
            { "TCInfo_0__Koryu", typeof(string) },
            { "TCInfo_0__reserved", typeof(string) },

            { "TCInfo_1__Num", typeof(int) },
            { "TCInfo_1__KettoNum", typeof(string) },
            { "TCInfo_1__Bamei", typeof(string) },
            { "TCInfo_1__UmaKigoCD", typeof(string) },
            { "TCInfo_1__SexCD", typeof(string) },
            { "TCInfo_1__TozaiCD", typeof(string) },
            { "TCInfo_1__ChokyosiCode", typeof(string) },
            { "TCInfo_1__ChokyosiRyakusyo", typeof(string) },
            { "TCInfo_1__Futan", typeof(int) },
            { "TCInfo_1__Koryu", typeof(string) },
            { "TCInfo_1__reserved", typeof(string) },

            { "TCInfo_2__Num", typeof(int) },
            { "TCInfo_2__KettoNum", typeof(string) },
            { "TCInfo_2__Bamei", typeof(string) },
            { "TCInfo_2__UmaKigoCD", typeof(string) },
            { "TCInfo_2__SexCD", typeof(string) },
            { "TCInfo_2__TozaiCD", typeof(string) },
            { "TCInfo_2__ChokyosiCode", typeof(string) },
            { "TCInfo_2__ChokyosiRyakusyo", typeof(string) },
            { "TCInfo_2__Futan", typeof(int) },
            { "TCInfo_2__Koryu", typeof(string) },
            { "TCInfo_2__reserved", typeof(string) }
        };
    }
}