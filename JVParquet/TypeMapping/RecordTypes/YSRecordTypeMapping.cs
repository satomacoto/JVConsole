using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// YSレコード（予想スケジュール）の型マッピング定義
    /// </summary>
    public class YSRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "YS";

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

            // 開催情報
            { "MidashiKaisaiDate_Year", typeof(int) },  // 見出し用開催年月日（年）
            { "MidashiKaisaiDate_Month", typeof(int) }, // 見出し用開催年月日（月）
            { "MidashiKaisaiDate_Day", typeof(int) },   // 見出し用開催年月日（日）
            { "MidashiJyoCD", typeof(string) },         // 見出し用場コード
            { "MidashiJyoName", typeof(string) },       // 見出し用場名
            { "MidashiKaiji", typeof(int) },            // 見出し用回次
            { "MidashiNichiji", typeof(int) },          // 見出し用日次
            { "YoubiCD", typeof(string) },              // 曜日コード
            { "HenkoID", typeof(string) },              // 変更識別

            // レース情報（12レース分の配列、簡略化のため最初の数レース分のみ定義）
            { "YSInfo_0__RaceNum", typeof(int) },
            { "YSInfo_0__TokuNum", typeof(string) },
            { "YSInfo_0__Hondai", typeof(string) },
            { "YSInfo_0__Ryakusyo10", typeof(string) },
            { "YSInfo_0__Ryakusyo6", typeof(string) },
            { "YSInfo_0__Ryakusyo3", typeof(string) },
            { "YSInfo_0__Nkai", typeof(string) },
            { "YSInfo_0__GradeCD", typeof(string) },
            { "YSInfo_0__GradeCDBefore", typeof(string) },
            { "YSInfo_0__SyubetuCD", typeof(string) },
            { "YSInfo_0__KigoCD", typeof(string) },
            { "YSInfo_0__JyuryoCD", typeof(string) },
            { "YSInfo_0__JyokenCD_0", typeof(string) },
            { "YSInfo_0__JyokenCD_1", typeof(string) },
            { "YSInfo_0__JyokenCD_2", typeof(string) },
            { "YSInfo_0__JyokenCD_3", typeof(string) },
            { "YSInfo_0__JyokenCD_4", typeof(string) },
            { "YSInfo_0__Kyori", typeof(int) },
            { "YSInfo_0__KyoriBefore", typeof(int) },
            { "YSInfo_0__TrackCD", typeof(string) },
            { "YSInfo_0__TrackCDBefore", typeof(string) },
            { "YSInfo_0__CourseKubunCD", typeof(string) },
            { "YSInfo_0__CourseKubunCDBefore", typeof(string) },
            { "YSInfo_0__HassoTime_Hour", typeof(int) },
            { "YSInfo_0__HassoTime_Minute", typeof(int) },
            { "YSInfo_0__HassoTimeBefore_Hour", typeof(int) },
            { "YSInfo_0__HassoTimeBefore_Minute", typeof(int) },
            { "YSInfo_0__TorokuTosu", typeof(int) },
            { "YSInfo_0__SyussoTosu", typeof(int) },
            { "YSInfo_0__SyussoTosuBefore", typeof(int) },
            { "YSInfo_0__TokuninKisya", typeof(string) },
            { "YSInfo_0__reserved", typeof(string) },

            { "YSInfo_1__RaceNum", typeof(int) },
            { "YSInfo_1__TokuNum", typeof(string) },
            { "YSInfo_1__Hondai", typeof(string) },
            { "YSInfo_1__GradeCD", typeof(string) },
            { "YSInfo_1__SyubetuCD", typeof(string) },
            { "YSInfo_1__Kyori", typeof(int) },
            { "YSInfo_1__TrackCD", typeof(string) },
            { "YSInfo_1__HassoTime_Hour", typeof(int) },
            { "YSInfo_1__HassoTime_Minute", typeof(int) },
            { "YSInfo_1__TorokuTosu", typeof(int) },
            { "YSInfo_1__SyussoTosu", typeof(int) }
        };
    }
}