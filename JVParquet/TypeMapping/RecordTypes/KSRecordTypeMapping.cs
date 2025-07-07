using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// KSレコード（騎手マスタ）の型マッピング定義
    /// </summary>
    public class KSRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "KS";

        public override List<string> IndexColumns => new List<string>
        {
            "KisyuCode"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 騎手情報
            { "KisyuCode", typeof(string) },            // 騎手コード
            { "DelKubun", typeof(string) },             // 削除区分
            { "IssueDate_Year", typeof(int) },          // 騎手免許交付年月日（年）
            { "IssueDate_Month", typeof(int) },         // 騎手免許交付年月日（月）
            { "IssueDate_Day", typeof(int) },           // 騎手免許交付年月日（日）
            { "DelDate_Year", typeof(int) },            // 騎手免許抹消年月日（年）
            { "DelDate_Month", typeof(int) },           // 騎手免許抹消年月日（月）
            { "DelDate_Day", typeof(int) },             // 騎手免許抹消年月日（日）
            { "BirthDate_Year", typeof(int) },          // 生年月日（年）
            { "BirthDate_Month", typeof(int) },         // 生年月日（月）
            { "BirthDate_Day", typeof(int) },           // 生年月日（日）
            { "KisyuName", typeof(string) },            // 騎手名
            { "reserved", typeof(string) },             // 予備
            { "KisyuNameKana", typeof(string) },        // 騎手名カナ
            { "KisyuRyakusyo", typeof(string) },        // 騎手名略称
            { "KisyuNameEng", typeof(string) },         // 騎手名欧字
            { "SexCD", typeof(string) },                // 性別コード
            { "SikakuCD", typeof(string) },             // 騎乗資格コード
            { "MinaraiCD", typeof(string) },            // 騎手見習コード
            { "TozaiCD", typeof(string) },              // 騎手東西所属コード
            { "Syotai", typeof(string) },               // 招待地域名
            { "MiddleYear", typeof(int) },              // 中央騎乗開始年
            { "MiddleKisyuCD", typeof(string) },        // 中央騎手コード
            { "HatuKiJyo_Year", typeof(int) },          // 初騎乗年月日（年）
            { "HatuKiJyo_Month", typeof(int) },         // 初騎乗年月日（月）
            { "HatuKiJyo_Day", typeof(int) },           // 初騎乗年月日（日）
            { "HatuKiJyoJyusyo_Year", typeof(int) },    // 初勝利年月日（年）
            { "HatuKiJyoJyusyo_Month", typeof(int) },   // 初勝利年月日（月）
            { "HatuKiJyoJyusyo_Day", typeof(int) },     // 初勝利年月日（日）
            { "HatuKiJyoFP", typeof(string) },          // 初騎乗冠名
            { "HatuKiJyoFPJyusyo", typeof(string) },    // 初勝利冠名
            { "GenYearKisyu", typeof(int) },            // 現在年騎手
            { "GenSaijiKisyu", typeof(int) },           // 現在歳騎手
            { "reserved2", typeof(string) }             // 予備
        };
    }
}