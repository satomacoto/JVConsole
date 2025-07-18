using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// HNレコード（繁殖馬マスタ）の型マッピング定義
    /// </summary>
    public class HNRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HN";

        public override List<string> IndexColumns => new List<string>
        {
            "HansyokuNum"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 繁殖馬情報
            { "HansyokuNum", typeof(string) },          // 繁殖登録番号
            { "reserved", typeof(string) },             // 予備
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "DelKubun", typeof(string) },             // 削除区分
            { "Bamei", typeof(string) },                // 馬名
            { "BameiKana", typeof(string) },            // 馬名カナ
            { "BameiEng", typeof(string) },             // 馬名欧字
            { "BirthYear", typeof(int) },               // 生年
            { "SexCD", typeof(string) },                // 性別コード
            { "HansyokuMochiKubun", typeof(string) },   // 繁殖馬持込区分
            { "KeiroCD", typeof(string) },              // 毛色コード
            { "HansyokuFNum", typeof(string) },         // 繁殖登録番号(父馬)
            { "HansyokuMNum", typeof(string) }          // 繁殖登録番号(母馬)
        };
    }
}