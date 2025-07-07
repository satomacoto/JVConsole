using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// BRレコード（生産者マスタ）の型マッピング定義
    /// </summary>
    public class BRRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "BR";

        public override List<string> IndexColumns => new List<string>
        {
            "BreederCode"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 生産者情報
            { "BreederCode", typeof(string) },          // 生産者コード
            { "BreederName_Co", typeof(string) },       // 生産者名(法人格有)
            { "BreederName", typeof(string) },          // 生産者名(法人格無)
            { "BreederNameKana", typeof(string) },      // 生産者名半角カナ
            { "BreederNameEng", typeof(string) },       // 生産者名欧字
            { "Address", typeof(string) },              // 生産者住所自治省名

            // 本年・累計成績情報（配列展開される）
            // HonRuikei[0] - 本年成績
            { "HonRuikei_0__SetYear", typeof(int) },              // 設定年
            { "HonRuikei_0__HonSyokinTotal", typeof(int) },   // 本賞金合計
            { "HonRuikei_0__FukaSyokin", typeof(int) },       // 付加賞金合計
            { "HonRuikei_0__ChakuKaisu_0", typeof(int) },         // 着回数[0] - 1着
            { "HonRuikei_0__ChakuKaisu_1", typeof(int) },         // 着回数[1] - 2着
            { "HonRuikei_0__ChakuKaisu_2", typeof(int) },         // 着回数[2] - 3着
            { "HonRuikei_0__ChakuKaisu_3", typeof(int) },         // 着回数[3] - 4着
            { "HonRuikei_0__ChakuKaisu_4", typeof(int) },         // 着回数[4] - 5着
            { "HonRuikei_0__ChakuKaisu_5", typeof(int) },         // 着回数[5] - 着外

            // HonRuikei[1] - 累計成績
            { "HonRuikei_1__SetYear", typeof(int) },              // 設定年
            { "HonRuikei_1__HonSyokinTotal", typeof(int) },   // 本賞金合計
            { "HonRuikei_1__FukaSyokin", typeof(int) },       // 付加賞金合計
            { "HonRuikei_1__ChakuKaisu_0", typeof(int) },         // 着回数[0] - 1着
            { "HonRuikei_1__ChakuKaisu_1", typeof(int) },         // 着回数[1] - 2着
            { "HonRuikei_1__ChakuKaisu_2", typeof(int) },         // 着回数[2] - 3着
            { "HonRuikei_1__ChakuKaisu_3", typeof(int) },         // 着回数[3] - 4着
            { "HonRuikei_1__ChakuKaisu_4", typeof(int) },         // 着回数[4] - 5着
            { "HonRuikei_1__ChakuKaisu_5", typeof(int) }          // 着回数[5] - 着外
        };
    }
}