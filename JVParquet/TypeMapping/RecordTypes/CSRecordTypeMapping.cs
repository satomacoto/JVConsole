using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// CSレコード（コース情報）の型マッピング定義
    /// </summary>
    public class CSRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CS";

        public override List<string> IndexColumns => new List<string>
        {
            "JyoCD",
            "Kyori",
            "TrackCD",
            "KaishuDate_Year",
            "KaishuDate_Month",
            "KaishuDate_Day"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // コース情報
            { "JyoCD", typeof(string) },              // 競馬場コード
            { "Kyori", typeof(int) },                 // 距離
            { "TrackCD", typeof(string) },            // トラックコード
            { "KaishuDate_Year", typeof(int) },       // コース改修年月日（年）
            { "KaishuDate_Month", typeof(int) },      // コース改修年月日（月）
            { "KaishuDate_Day", typeof(int) },        // コース改修年月日（日）
            { "CourseEx", typeof(string) }            // コース説明
        };
    }
}