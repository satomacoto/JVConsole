using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// RCレコード（レースコメント）の型マッピング定義
    /// </summary>
    public class RCRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "RC";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji",
            "id_RaceNum",
            "TokuNum",
            "SyubetuCD",
            "Kyori",
            "TrackCD"
        };

        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // レコードヘッダー
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },

            // 競走識別情報
            { "id_Year", typeof(int) },
            { "id_MonthDay", typeof(int) },
            { "id_JyoCD", typeof(string) },
            { "id_Kaiji", typeof(int) },
            { "id_Nichiji", typeof(int) },
            { "id_RaceNum", typeof(int) },

            // レース情報
            { "TokuNum", typeof(string) },              // 特別競走番号
            { "SyubetuCD", typeof(string) },            // 競走種別コード
            { "Kyori", typeof(int) },                   // 距離
            { "TrackCD", typeof(string) },              // トラックコード

            // コメント
            { "RaceComment", typeof(string) },          // レースコメント
            { "HaronTimeL4", typeof(int) },             // ハロンタイム（ラスト4ハロン）
            { "HaronTimeL3", typeof(int) },             // ハロンタイム（ラスト3ハロン）
            { "reserved", typeof(string) }              // 予備
        };
    }
}