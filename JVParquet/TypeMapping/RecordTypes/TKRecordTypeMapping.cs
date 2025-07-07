using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// TKレコード（特別登録（抽選番号））の型マッピング定義
    /// </summary>
    public class TKRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "TK";

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

            // 特別登録（抽選番号）情報（最初の数頭分のみ定義）
            { "TKInfo_0__Num", typeof(int) },
            { "TKInfo_0__KettoNum", typeof(string) },
            { "TKInfo_0__Bamei", typeof(string) },
            { "TKInfo_0__UmaKigoCD", typeof(string) },
            { "TKInfo_0__SexCD", typeof(string) },
            { "TKInfo_0__TozaiCD", typeof(string) },
            { "TKInfo_0__ChokyosiCode", typeof(string) },
            { "TKInfo_0__ChokyosiRyakusyo", typeof(string) },
            { "TKInfo_0__Futan", typeof(int) },
            { "TKInfo_0__Koryu", typeof(string) },
            { "TKInfo_0__BanusiName", typeof(string) },
            { "TKInfo_0__Honsyokin", typeof(int) },
            { "TKInfo_0__Prize", typeof(int) },
            { "TKInfo_0__Jyoken", typeof(string) },
            { "TKInfo_0__Syokin", typeof(int) },

            { "TKInfo_1__Num", typeof(int) },
            { "TKInfo_1__KettoNum", typeof(string) },
            { "TKInfo_1__Bamei", typeof(string) },
            { "TKInfo_1__UmaKigoCD", typeof(string) },
            { "TKInfo_1__SexCD", typeof(string) },
            { "TKInfo_1__TozaiCD", typeof(string) },
            { "TKInfo_1__ChokyosiCode", typeof(string) },
            { "TKInfo_1__ChokyosiRyakusyo", typeof(string) },
            { "TKInfo_1__Futan", typeof(int) },
            { "TKInfo_1__Koryu", typeof(string) },
            { "TKInfo_1__BanusiName", typeof(string) },
            { "TKInfo_1__Honsyokin", typeof(int) },
            { "TKInfo_1__Prize", typeof(int) },
            { "TKInfo_1__Jyoken", typeof(string) },
            { "TKInfo_1__Syokin", typeof(int) }
        };
    }
}