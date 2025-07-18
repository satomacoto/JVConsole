using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// UMレコード（馬マスタ）の型マッピング定義
    /// </summary>
    public class UMRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "UM";

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

            // 馬マスタ情報
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "DelKubun", typeof(string) },             // 削除区分
            { "RegDate_Year", typeof(int) },            // 登録年月日（年）
            { "RegDate_Month", typeof(int) },           // 登録年月日（月）
            { "RegDate_Day", typeof(int) },             // 登録年月日（日）
            { "DelDate_Year", typeof(int) },            // 抹消年月日（年）
            { "DelDate_Month", typeof(int) },           // 抹消年月日（月）
            { "DelDate_Day", typeof(int) },             // 抹消年月日（日）
            { "BirthDate_Year", typeof(int) },          // 生年月日（年）
            { "BirthDate_Month", typeof(int) },         // 生年月日（月）
            { "BirthDate_Day", typeof(int) },           // 生年月日（日）
            { "Bamei", typeof(string) },                // 馬名
            { "BameiKana", typeof(string) },            // 馬名カナ
            { "BameiEng", typeof(string) },             // 馬名欧字
            { "UmaKigoCD", typeof(string) },            // 馬記号コード
            { "SexCD", typeof(string) },                // 性別コード
            { "HinsyuCD", typeof(string) },             // 品種コード
            { "KeiroCD", typeof(string) },              // 毛色コード

            // 血統情報
            { "Ketto3Info_0__HansyokuNum", typeof(string) },  // 3代血統[0] 繁殖登録番号
            { "Ketto3Info_0__Bamei", typeof(string) },        // 3代血統[0] 馬名
            { "Ketto3Info_1__HansyokuNum", typeof(string) },  // 3代血統[1] 繁殖登録番号
            { "Ketto3Info_1__Bamei", typeof(string) },        // 3代血統[1] 馬名
            { "Ketto3Info_2__HansyokuNum", typeof(string) },  // 3代血統[2] 繁殖登録番号
            { "Ketto3Info_2__Bamei", typeof(string) },        // 3代血統[2] 馬名
            { "Ketto3Info_3__HansyokuNum", typeof(string) },  // 3代血統[3] 繁殖登録番号
            { "Ketto3Info_3__Bamei", typeof(string) },        // 3代血統[3] 馬名
            { "Ketto3Info_4__HansyokuNum", typeof(string) },  // 3代血統[4] 繁殖登録番号
            { "Ketto3Info_4__Bamei", typeof(string) },        // 3代血統[4] 馬名
            { "Ketto3Info_5__HansyokuNum", typeof(string) },  // 3代血統[5] 繁殖登録番号
            { "Ketto3Info_5__Bamei", typeof(string) },        // 3代血統[5] 馬名
            { "Ketto3Info_6__HansyokuNum", typeof(string) },  // 3代血統[6] 繁殖登録番号
            { "Ketto3Info_6__Bamei", typeof(string) },        // 3代血統[6] 馬名

            // その他情報
            { "TozaiCD", typeof(string) },              // 調教師東西コード
            { "ChokyosiCode", typeof(string) },         // 調教師コード
            { "ChokyosiRyakusyo", typeof(string) },     // 調教師名略称
            { "Syotai", typeof(string) },               // 招待地域名
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
            { "SogoChakukaisu_5", typeof(int) }         // 総合着回数[5] - 着外
        };
    }
}