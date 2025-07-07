using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// HYレコード（馬名意味由来）の型マッピング定義
    /// </summary>
    public class HYRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "HY";

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

            // 馬名意味由来情報
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "DelKubun", typeof(string) },             // 削除区分
            { "Bamei", typeof(string) },                // 馬名
            { "BameiKana", typeof(string) },            // 馬名カナ
            { "BameiEng", typeof(string) },             // 馬名欧字
            { "ZaikyuFlag", typeof(string) },           // 在厩フラグ
            { "Reserved", typeof(string) },             // 予備
            { "Origin", typeof(string) }                // 馬名の意味由来
        };
    }
}