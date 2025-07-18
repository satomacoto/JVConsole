using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// JGレコード（除外馬）の型マッピング定義
    /// </summary>
    public class JGRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "JG";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji",
            "id_RaceNum",
            "KettoNum",
            "ShutsubaTohyoJun"
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

            // 除外馬情報
            { "KettoNum", typeof(string) },             // 血統登録番号
            { "Bamei", typeof(string) },                // 馬名
            { "ShutsubaTohyoJun", typeof(int) },        // 出走投票順位
            { "GradeCD", typeof(string) },              // グレードコード
            { "JyokenCD1", typeof(string) },            // 条件コード1
            { "JyokenCD2", typeof(string) },            // 条件コード2
            { "JyokenCD3", typeof(string) },            // 条件コード3
            { "JyokenCD4", typeof(string) },            // 条件コード4
            { "JyokenCD5", typeof(string) },            // 条件コード5
            { "ChakusaTotalCD", typeof(string) },       // 着差コード合計
            { "Kyakusitu1", typeof(string) },           // 客質指数1
            { "Kyakusitu2", typeof(string) },           // 客質指数2
            { "Kyakusitu3", typeof(string) },           // 客質指数3
            { "Kyakusitu4", typeof(string) }            // 客質指数4
        };
    }
}