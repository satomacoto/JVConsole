using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// RAレコード（レース詳細）の型マッピング定義
    /// </summary>
    public class RARecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "RA";

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

            // レース情報
            { "RaceInfo_YoubiCD", typeof(string) },      // 曜日コード
            { "RaceInfo_TokuNum", typeof(string) },      // 特別競走番号
            { "RaceInfo_Hondai", typeof(string) },       // 競走名本題
            { "RaceInfo_Fukudai", typeof(string) },      // 競走名副題
            { "RaceInfo_Kakko", typeof(string) },        // 競走名カッコ内
            { "RaceInfo_HondaiEng", typeof(string) },    // 競走名本題欧字
            { "RaceInfo_FukudaiEng", typeof(string) },   // 競走名副題欧字
            { "RaceInfo_KakkoEng", typeof(string) },     // 競走名カッコ内欧字
            { "RaceInfo_Ryakusyo10", typeof(string) },   // 競走名略称10字
            { "RaceInfo_Ryakusyo6", typeof(string) },    // 競走名略称6字
            { "RaceInfo_Ryakusyo3", typeof(string) },    // 競走名略称3字
            { "RaceInfo_Kubun", typeof(string) },        // 競走名区分
            { "RaceInfo_Nkai", typeof(string) },         // 重賞回次

            // グレード情報
            { "GradeCD", typeof(string) },               // グレードコード
            { "GradeCDBefore", typeof(string) },         // 変更前グレードコード

            // 競走条件情報
            { "JyokenInfo_SyubetuCD", typeof(string) },  // 競走種別コード
            { "JyokenInfo_KigoCD", typeof(string) },     // 競走記号コード
            { "JyokenInfo_JyuryoCD", typeof(string) },   // 重量種別コード
            { "JyokenInfo_JyokenCD_0", typeof(string) }, // 競走条件コード[0]
            { "JyokenInfo_JyokenCD_1", typeof(string) }, // 競走条件コード[1]
            { "JyokenInfo_JyokenCD_2", typeof(string) }, // 競走条件コード[2]
            { "JyokenInfo_JyokenCD_3", typeof(string) }, // 競走条件コード[3]
            { "JyokenInfo_JyokenCD_4", typeof(string) }, // 競走条件コード[4]
            
            // 距離・コース情報
            { "JyokenInfo_Kyori", typeof(int) },         // 距離
            { "JyokenInfo_TrackCD", typeof(string) },    // トラックコード
            { "CourseKubunCD", typeof(string) },         // コース区分
            { "Honsyokin_0", typeof(int) },          // 本賞金[0] - 1着
            { "Honsyokin_1", typeof(int) },          // 本賞金[1] - 2着
            { "Honsyokin_2", typeof(int) },          // 本賞金[2] - 3着
            { "Honsyokin_3", typeof(int) },          // 本賞金[3] - 4着
            { "Honsyokin_4", typeof(int) },          // 本賞金[4] - 5着
            { "HonsyokinBefore_0", typeof(int) },    // 変更前本賞金[0] - 1着
            { "Fukasyokin_0", typeof(int) },         // 付加賞金[0] - 1着
            { "Fukasyokin_1", typeof(int) },         // 付加賞金[1] - 2着
            { "Fukasyokin_2", typeof(int) },         // 付加賞金[2] - 3着
            { "FukasyokinBefore_0", typeof(int) },   // 変更前付加賞金[0] - 1着

            // 発走時刻
            { "HassoTime_Hour", typeof(int) },           // 発走時刻（時）
            { "HassoTime_Minute", typeof(int) },         // 発走時刻（分）
            { "HassoTimeBefore_Hour", typeof(int) },     // 変更前発走時刻（時）
            { "HassoTimeBefore_Minute", typeof(int) },   // 変更前発走時刻（分）

            // その他
            { "TorokuTosu", typeof(int) },               // 登録頭数
            { "SyussoTosu", typeof(int) },               // 出走頭数
            { "NyusenTosu", typeof(int) }                // 入線頭数
        };
    }
}