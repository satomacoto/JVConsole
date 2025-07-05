using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// BTレコード（系統情報）の型マッピング定義
    /// </summary>
    public class BTRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "BT";

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

            // 系統情報
            { "HansyokuNum", typeof(string) },     // 繁殖登録番号
            { "KeitoId", typeof(string) },         // 系統ID
            { "KeitoName", typeof(string) },       // 系統名
            { "KeitoEx", typeof(string) }          // 系統説明
        };
    }
}