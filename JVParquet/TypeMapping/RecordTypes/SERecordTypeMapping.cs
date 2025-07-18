using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// SEレコード（レース成績）の型マッピング定義
    /// </summary>
    public class SERecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "SE";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji",
            "id_RaceNum",
            "Umaban",
            "KettoNum"
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

            // 馬毎成績情報
            { "Wakuban", typeof(int) },                 // 枠番
            { "Umaban", typeof(int) },                  // 馬番
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "Bamei", typeof(string) },                // 馬名
            { "UmaKigoCD", typeof(string) },            // 馬記号コード
            { "SexCD", typeof(string) },                // 性別コード
            { "Barei", typeof(int) },                   // 馬齢
            { "TozaiCD", typeof(string) },              // 調教師東西コード
            { "ChokyosiCode", typeof(string) },         // 調教師コード
            { "ChokyosiRyakusyo", typeof(string) },     // 調教師名略称
            { "BanusiCode", typeof(string) },           // 馬主コード
            { "BanusiName", typeof(string) },           // 馬主名
            { "Fukusyoku", typeof(string) },            // 服色標示
            { "reserved1", typeof(string) },            // 予備
            { "Futan", typeof(int) },                   // 負担重量
            { "FutanBefore", typeof(int) },             // 変更前負担重量
            { "Blinker", typeof(string) },              // ブリンカー使用区分
            { "reserved2", typeof(string) },            // 予備
            { "KisyuCode", typeof(string) },            // 騎手コード
            { "KisyuCodeBefore", typeof(string) },      // 変更前騎手コード
            { "KisyuRyakusyo", typeof(string) },        // 騎手名略称
            { "KisyuRyakusyoBefore", typeof(string) },  // 変更前騎手名略称
            { "MinaraiCD", typeof(string) },            // 騎手見習コード
            { "MinaraiCDBefore", typeof(string) },      // 変更前騎手見習コード
            { "BaTaijyu", typeof(int) },                // 馬体重
            { "ZogenFugo", typeof(string) },            // 増減符号
            { "ZogenSa", typeof(int) },                 // 増減差
            { "IJyoCD", typeof(string) },               // 異常区分コード
            { "NyusenJyuni", typeof(int) },             // 入線順位
            { "KakuteiJyuni", typeof(int) },            // 確定着順
            { "DochakuKubun", typeof(string) },         // 同着区分
            { "DochakuTosu", typeof(int) },             // 同着頭数
            { "Time", typeof(int) },                    // 走破タイム
            { "ChakusaCD", typeof(string) },            // 着差コード
            { "ChakusaCDP", typeof(string) },           // 着差コード(+1馬身)
            { "ChakusaCDPP", typeof(string) },          // 着差コード(+2馬身)
            { "Jyuni1c", typeof(int) },                 // 1コーナーでの順位
            { "Jyuni2c", typeof(int) },                 // 2コーナーでの順位
            { "Jyuni3c", typeof(int) },                 // 3コーナーでの順位
            { "Jyuni4c", typeof(int) },                 // 4コーナーでの順位
            { "Odds", typeof(int) },                // 単勝オッズ
            { "Ninki", typeof(int) },                   // 単勝人気順
            { "Honsyokin", typeof(int) },           // 獲得本賞金
            { "Fukasyokin", typeof(int) },          // 獲得付加賞金
            { "reserved3", typeof(string) },            // 予備
            { "reserved4", typeof(string) },            // 予備
            { "HaronTimeL4", typeof(int) },             // 後4ハロンタイム
            { "HaronTimeL3", typeof(int) },             // 後3ハロンタイム
            { "KettoNum1", typeof(string) },            // 1着馬(相手馬)血統登録番号
            { "KettoNum2", typeof(string) },            // 2着馬(相手馬)血統登録番号
            { "KettoNum3", typeof(string) },            // 3着馬(相手馬)血統登録番号
            { "TimeDiff", typeof(int) },                // 1着とのタイム差
            { "RecordUpKubun", typeof(string) }         // レコード更新区分
        };
    }
}