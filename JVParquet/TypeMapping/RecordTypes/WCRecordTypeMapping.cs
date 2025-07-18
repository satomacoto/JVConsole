using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// WCレコード（ウッドチップ調教）の型マッピング定義
    /// </summary>
    public class WCRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WC";

        public override List<string> IndexColumns => new List<string>
        {
            "TresenKubun",
            "ChokyoDate_Year",
            "ChokyoDate_Month",
            "ChokyoDate_Day",
            "ChokyoTime",
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

            // 調教情報
            { "TresenKubun", typeof(string) },          // トレセン区分
            { "ChokyoDate_Year", typeof(int) },         // 調教年月日（年）
            { "ChokyoDate_Month", typeof(int) },        // 調教年月日（月）
            { "ChokyoDate_Day", typeof(int) },          // 調教年月日（日）
            { "ChokyoTime", typeof(string) },           // 調教時刻
            { "KettoNum", typeof(string) },             // 血統登録番号

            // ハロンタイム情報
            { "HaronTime6", typeof(int) },              // 6ハロンタイム合計(1200M-0M)
            { "LapTime6", typeof(int) },                // ラップタイム(1200M-1000M)
            { "HaronTime5", typeof(int) },              // 5ハロンタイム合計(1000M-0M)
            { "LapTime5", typeof(int) },                // ラップタイム(1000M-800M)
            { "HaronTime4", typeof(int) },              // 4ハロンタイム合計(800M-0M)
            { "LapTime4", typeof(int) },                // ラップタイム(800M-600M)
            { "HaronTime3", typeof(int) },              // 3ハロンタイム合計(600M-0M)
            { "LapTime3", typeof(int) },                // ラップタイム(600M-400M)
            { "HaronTime2", typeof(int) },              // 2ハロンタイム合計(400M-0M)
            { "LapTime2", typeof(int) },                // ラップタイム(400M-200M)
            { "LapTime1", typeof(int) }                 // ラップタイム(200M-0M)
        };
    }
}