using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// HSレコード（馬主データ）の型マッピング定義
    /// </summary>
    public class HSRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HS";

        public override List<string> IndexColumns => new List<string>
        {
            "KettoNum",
            "SaleCode",
            "FromDate_Year",
            "FromDate_Month",
            "FromDate_Day"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 馬主情報
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "SaleCode", typeof(string) },             // セール情報
            { "HansyokuHanryoCD", typeof(string) },     // 繁殖・現役区分
            { "Bamei", typeof(string) },                // 馬名
            { "UmaKigoCD", typeof(string) },            // 馬記号コード
            { "SexCD", typeof(string) },                // 性別コード
            { "TozaiCD", typeof(string) },              // 調教師東西コード
            { "ChokyosiCode", typeof(string) },         // 調教師コード
            { "ChokyosiRyakusyo", typeof(string) },     // 調教師名略称
            { "Syotai", typeof(string) },               // 招待地域名
            { "BreederCode", typeof(string) },          // 生産者コード
            { "BreederName", typeof(string) },          // 生産者名
            { "SanchiName", typeof(string) },           // 産地名
            { "BanusiCode", typeof(string) },           // 馬主コード
            { "BanusiName", typeof(string) },           // 馬主名
            { "RuikeiHonsyoHeiti", typeof(int) },   // 平地本賞金累計
            { "RuikeiHonsyoSyogai", typeof(int) },  // 障害本賞金累計
            { "RuikeiFukaHeichi", typeof(int) },    // 平地付加賞金累計
            { "RuikeiFukaSyogai", typeof(int) },    // 障害付加賞金累計
            { "RuikeiSyutokuHeichi", typeof(int) }, // 平地収得賞金累計
            { "RuikeiSyutokuSyogai", typeof(int) }, // 障害収得賞金累計
            { "SogoChakukaisu_0", typeof(int) },        // 総合着回数[0] - 1着
            { "SogoChakukaisu_1", typeof(int) },        // 総合着回数[1] - 2着
            { "SogoChakukaisu_2", typeof(int) },        // 総合着回数[2] - 3着
            { "SogoChakukaisu_3", typeof(int) },        // 総合着回数[3] - 4着
            { "SogoChakukaisu_4", typeof(int) },        // 総合着回数[4] - 5着
            { "SogoChakukaisu_5", typeof(int) },        // 総合着回数[5] - 着外
            { "ChuoChakukaisu_0", typeof(int) },        // 中央着回数[0] - 1着
            { "ChuoChakukaisu_1", typeof(int) },        // 中央着回数[1] - 2着

            // 日付情報
            { "FromDate_Year", typeof(int) },           // 開始年月日（年）
            { "FromDate_Month", typeof(int) },          // 開始年月日（月）
            { "FromDate_Day", typeof(int) },            // 開始年月日（日）

            // その他
            { "SaleHostName", typeof(string) },         // 主催者名
            { "SaleHostCode", typeof(string) },         // 主催者コード
            { "SaleName", typeof(string) }              // セール名
        };
    }
}