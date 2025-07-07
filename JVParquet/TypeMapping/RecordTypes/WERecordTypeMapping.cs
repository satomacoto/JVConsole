using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// WEレコード（天候・馬場状態）の型マッピング定義
    /// </summary>
    public class WERecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "WE";

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

            // 天候・馬場状態情報
            { "HenkoID", typeof(string) },              // 変更識別
            { "YoubiCD", typeof(string) },              // 曜日コード

            // 天候（発表時刻と天候を9回分、簡略化のため最初の数回分のみ定義）
            { "TenkoBaba_0__HappyoTime_Hour", typeof(int) },
            { "TenkoBaba_0__HappyoTime_Minute", typeof(int) },
            { "TenkoBaba_0__TenkoCD", typeof(string) },
            { "TenkoBaba_0__SibaBabaCD", typeof(string) },
            { "TenkoBaba_0__DirtBabaCD", typeof(string) },

            { "TenkoBaba_1__HappyoTime_Hour", typeof(int) },
            { "TenkoBaba_1__HappyoTime_Minute", typeof(int) },
            { "TenkoBaba_1__TenkoCD", typeof(string) },
            { "TenkoBaba_1__SibaBabaCD", typeof(string) },
            { "TenkoBaba_1__DirtBabaCD", typeof(string) },

            { "TenkoBaba_2__HappyoTime_Hour", typeof(int) },
            { "TenkoBaba_2__HappyoTime_Minute", typeof(int) },
            { "TenkoBaba_2__TenkoCD", typeof(string) },
            { "TenkoBaba_2__SibaBabaCD", typeof(string) },
            { "TenkoBaba_2__DirtBabaCD", typeof(string) }
        };
    }
}