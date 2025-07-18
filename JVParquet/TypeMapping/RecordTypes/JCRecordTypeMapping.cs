using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// JCレコード（競走馬市場取引価格）の型マッピング定義
    /// </summary>
    public class JCRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "JC";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji"
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

            // 開催情報
            { "HenkoID", typeof(string) },              // 変更識別
            { "WeekCD", typeof(string) },               // 曜日コード
            { "YoubiCD", typeof(string) },              // 曜日コード
            { "JyusyoKaiji", typeof(int) },             // 重賞回次
            { "TsukigoHonsyoHeichi", typeof(int) }, // 月毎平地本賞金合計
            { "TsukigoHonsyoSyogai", typeof(int) }, // 月毎障害本賞金合計
            { "TsukigoFukaHeichi", typeof(int) },   // 月毎平地付加賞金合計
            { "TsukigoFukaSyogai", typeof(int) },   // 月毎障害付加賞金合計
            { "reserved", typeof(string) }              // 予備
        };
    }
}