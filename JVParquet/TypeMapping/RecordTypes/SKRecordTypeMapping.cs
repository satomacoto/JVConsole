using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// SKレコード（産駒マスタ）の型マッピング定義
    /// </summary>
    public class SKRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "SK";

        public override List<string> IndexColumns => new List<string>
        {
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

            // 産駒情報
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "DelKubun", typeof(string) },             // 削除区分
            { "JGDate_Year", typeof(int) },             // JG施行年月日（年）
            { "JGDate_Month", typeof(int) },            // JG施行年月日（月）
            { "JGDate_Day", typeof(int) },              // JG施行年月日（日）
            { "BirthDate_Year", typeof(int) },          // 生年月日（年）
            { "BirthDate_Month", typeof(int) },         // 生年月日（月）
            { "BirthDate_Day", typeof(int) },           // 生年月日（日）
            { "Bamei", typeof(string) },                // 馬名
            { "BameiKana", typeof(string) },            // 馬名カナ
            { "BameiEng", typeof(string) },             // 馬名欧字
            { "ZaikyuFlag", typeof(string) },           // 在厩フラグ
            { "Reserved", typeof(string) },             // 予備
            { "UmaKigoCD", typeof(string) },            // 馬記号コード
            { "SexCD", typeof(string) },                // 性別コード
            { "HinsyuCD", typeof(string) },             // 品種コード
            { "KeiroCD", typeof(string) },              // 毛色コード
            { "Ketto3InfoKei_0__HansyokuNum", typeof(string) },  // 3代血統 父馬 繁殖登録番号
            { "Ketto3InfoKei_0__Bamei", typeof(string) },        // 3代血統 父馬 馬名
            { "Ketto3InfoBoba_0__HansyokuNum", typeof(string) }, // 3代血統 母馬 繁殖登録番号
            { "Ketto3InfoBoba_0__Bamei", typeof(string) },       // 3代血統 母馬 馬名
            { "TozaiCD", typeof(string) },              // 調教師東西コード
            { "ChokyosiCode", typeof(string) },         // 調教師コード
            { "ChokyosiRyakusyo", typeof(string) },     // 調教師名略称
            { "BreederCode", typeof(string) },          // 生産者コード
            { "BreederName", typeof(string) },          // 生産者名(法人格有)
            { "SanchiName", typeof(string) },           // 産地名
            { "BanusiCode", typeof(string) },           // 馬主コード
            { "BanusiName", typeof(string) },           // 馬主名(法人格有)
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
            { "ChuoChakukaisu_1", typeof(int) }         // 中央着回数[1] - 2着
        };
    }
}