using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// CHレコード（調教師マスタ）の型マッピング定義
    /// </summary>
    public class CHRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CH";

        public override List<string> IndexColumns => new List<string>
        {
            "ChokyosiCode"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 調教師情報
            { "ChokyosiCode", typeof(string) },             // 調教師コード
            { "DelKubun", typeof(string) },                 // 調教師抹消区分
            { "IssueDate_Year", typeof(int) },              // 免許交付年
            { "IssueDate_Month", typeof(int) },             // 免許交付月
            { "IssueDate_Day", typeof(int) },               // 免許交付日
            { "DelDate_Year", typeof(int) },                // 免許抹消年
            { "DelDate_Month", typeof(int) },               // 免許抹消月
            { "DelDate_Day", typeof(int) },                 // 免許抹消日
            { "BirthDate_Year", typeof(int) },              // 生年
            { "BirthDate_Month", typeof(int) },             // 生月
            { "BirthDate_Day", typeof(int) },               // 生日
            { "ChokyosiName", typeof(string) },             // 調教師名漢字
            { "ChokyosiNameKana", typeof(string) },         // 調教師名半角カナ
            { "ChokyosiRyakusyo", typeof(string) },         // 調教師名略称
            { "ChokyosiNameEng", typeof(string) },          // 調教師名欧字
            { "SexCD", typeof(string) },                    // 性別区分
            { "TozaiCD", typeof(string) },                  // 調教師東西所属コード
            { "Syotai", typeof(string) },                   // 招待地域名

            // 最近重賞勝利情報（3個分）
            { "SaikinJyusyo_0__SaikinJyusyoid_Year", typeof(int) },
            { "SaikinJyusyo_0__SaikinJyusyoid_MonthDay", typeof(int) },
            { "SaikinJyusyo_0__SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_0__SaikinJyusyoid_Kaiji", typeof(int) },
            { "SaikinJyusyo_0__SaikinJyusyoid_Nichiji", typeof(int) },
            { "SaikinJyusyo_0__SaikinJyusyoid_RaceNum", typeof(int) },
            { "SaikinJyusyo_0__Hondai", typeof(string) },
            { "SaikinJyusyo_0__Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_0__Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_0__Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_0__GradeCD", typeof(string) },
            { "SaikinJyusyo_0__SyussoTosu", typeof(int) },
            { "SaikinJyusyo_0__KettoNum", typeof(string) },
            { "SaikinJyusyo_0__Bamei", typeof(string) },

            { "SaikinJyusyo_1__SaikinJyusyoid_Year", typeof(int) },
            { "SaikinJyusyo_1__SaikinJyusyoid_MonthDay", typeof(int) },
            { "SaikinJyusyo_1__SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_1__SaikinJyusyoid_Kaiji", typeof(int) },
            { "SaikinJyusyo_1__SaikinJyusyoid_Nichiji", typeof(int) },
            { "SaikinJyusyo_1__SaikinJyusyoid_RaceNum", typeof(int) },
            { "SaikinJyusyo_1__Hondai", typeof(string) },
            { "SaikinJyusyo_1__Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_1__Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_1__Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_1__GradeCD", typeof(string) },
            { "SaikinJyusyo_1__SyussoTosu", typeof(int) },
            { "SaikinJyusyo_1__KettoNum", typeof(string) },
            { "SaikinJyusyo_1__Bamei", typeof(string) },

            { "SaikinJyusyo_2__SaikinJyusyoid_Year", typeof(int) },
            { "SaikinJyusyo_2__SaikinJyusyoid_MonthDay", typeof(int) },
            { "SaikinJyusyo_2__SaikinJyusyoid_JyoCD", typeof(string) },
            { "SaikinJyusyo_2__SaikinJyusyoid_Kaiji", typeof(int) },
            { "SaikinJyusyo_2__SaikinJyusyoid_Nichiji", typeof(int) },
            { "SaikinJyusyo_2__SaikinJyusyoid_RaceNum", typeof(int) },
            { "SaikinJyusyo_2__Hondai", typeof(string) },
            { "SaikinJyusyo_2__Ryakusyo10", typeof(string) },
            { "SaikinJyusyo_2__Ryakusyo6", typeof(string) },
            { "SaikinJyusyo_2__Ryakusyo3", typeof(string) },
            { "SaikinJyusyo_2__GradeCD", typeof(string) },
            { "SaikinJyusyo_2__SyussoTosu", typeof(int) },
            { "SaikinJyusyo_2__KettoNum", typeof(string) },
            { "SaikinJyusyo_2__Bamei", typeof(string) },

            // 本年・前年・累計成績情報
            { "HonZenRuikei_0__SetYear", typeof(int) },
            { "HonZenRuikei_0__HonSyokinTotal", typeof(int) },
            { "HonZenRuikei_0__FukaSyokin", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_0", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_1", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_2", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_3", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_4", typeof(int) },
            { "HonZenRuikei_0__ChakuKaisu_5", typeof(int) },

            { "HonZenRuikei_1__SetYear", typeof(int) },
            { "HonZenRuikei_1__HonSyokinTotal", typeof(int) },
            { "HonZenRuikei_1__FukaSyokin", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_0", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_1", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_2", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_3", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_4", typeof(int) },
            { "HonZenRuikei_1__ChakuKaisu_5", typeof(int) },

            { "HonZenRuikei_2__SetYear", typeof(int) },
            { "HonZenRuikei_2__HonSyokinTotal", typeof(int) },
            { "HonZenRuikei_2__FukaSyokin", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_0", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_1", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_2", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_3", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_4", typeof(int) },
            { "HonZenRuikei_2__ChakuKaisu_5", typeof(int) }
        };
    }
}