using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping.RecordTypes
{
    /// <summary>
    /// CKレコード（出走別着度数）の型マッピング定義
    /// </summary>
    public class CKRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "CK";

        public override List<string> IndexColumns => new List<string>
        {
            "id_Year",
            "id_MonthDay",
            "id_JyoCD",
            "id_Kaiji",
            "id_Nichiji",
            "id_RaceNum",
            "UmaChaku_KettoNum"
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

            // 馬着度数情報
            { "UmaChaku_KettoNum", typeof(string) },
            { "UmaChaku_Bamei", typeof(string) },
            { "UmaChaku_RuikeiHonsyoHeiti", typeof(int) },
            { "UmaChaku_RuikeiHonsyoSyogai", typeof(int) },
            
            // 着回数総合（ChakuSogo）
            { "UmaChaku_ChakuSogo_ChakuKaisu_0", typeof(int) },
            { "UmaChaku_ChakuSogo_ChakuKaisu_1", typeof(int) },
            { "UmaChaku_ChakuSogo_ChakuKaisu_2", typeof(int) },
            { "UmaChaku_ChakuSogo_ChakuKaisu_3", typeof(int) },
            { "UmaChaku_ChakuSogo_ChakuKaisu_4", typeof(int) },
            { "UmaChaku_ChakuSogo_ChakuKaisu_5", typeof(int) },

            // 着回数中央（ChakuChuo）
            { "UmaChaku_ChakuChuo_ChakuKaisu_0", typeof(int) },
            { "UmaChaku_ChakuChuo_ChakuKaisu_1", typeof(int) },
            { "UmaChaku_ChakuChuo_ChakuKaisu_2", typeof(int) },
            { "UmaChaku_ChakuChuo_ChakuKaisu_3", typeof(int) },
            { "UmaChaku_ChakuChuo_ChakuKaisu_4", typeof(int) },
            { "UmaChaku_ChakuChuo_ChakuKaisu_5", typeof(int) },

            // 脚質
            { "UmaChaku_Kyakusitu_0", typeof(string) },
            { "UmaChaku_Kyakusitu_1", typeof(string) },
            { "UmaChaku_Kyakusitu_2", typeof(string) },
            { "UmaChaku_Kyakusitu_3", typeof(string) },

            // 騎手着度数情報
            { "KisyuChaku_KisyuCode", typeof(string) },
            { "KisyuChaku_KisyuName", typeof(string) },

            // 調教師着度数情報
            { "ChokyoChaku_ChokyosiCode", typeof(string) },
            { "ChokyoChaku_ChokyosiName", typeof(string) },

            // 馬主着度数情報
            { "BanusiChaku_BanusiCode", typeof(string) },
            { "BanusiChaku_BanusiName_Co", typeof(string) },
            { "BanusiChaku_BanusiName", typeof(string) },

            // 生産者着度数情報
            { "BreederChaku_BreederCode", typeof(string) },
            { "BreederChaku_BreederName_Co", typeof(string) },
            { "BreederChaku_BreederName", typeof(string) }
        };
    }
}