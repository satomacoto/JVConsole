namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// UMレコード（馬マスタ）の型マッピング
    /// </summary>
    public class UmRecordTypeMapping : RecordTypeMappingBase
    {
        public override string RecordSpec => "UM";
        
        public override Dictionary<string, Type> FieldTypeMappings => new Dictionary<string, Type>
        {
            // 基本情報
            { "KettoNum", typeof(string) },
            { "DelKubun", typeof(string) },
            { "RegDate", typeof(string) },
            { "DelDate", typeof(string) },
            { "BirthDate", typeof(string) },
            { "Bamei", typeof(string) },
            { "BameiKana", typeof(string) },
            { "BameiEng", typeof(string) },
            { "ZenBamei", typeof(string) },
            { "UmaKigoCD", typeof(string) },
            { "SexCD", typeof(string) },
            { "HinsyuCD", typeof(string) },
            { "KeiroCD", typeof(string) },
            { "HansyokuMochiKubun", typeof(string) },
            { "ImportYear", typeof(int) },
            { "SanchiName", typeof(string) },
            { "BreederCode", typeof(string) },
            { "BreederName", typeof(string) },
            { "HansyokuFKettoNum", typeof(string) },
            { "HansyokuMKettoNum", typeof(string) },
            { "HansyokuFF", typeof(string) },
            { "HansyokuFM", typeof(string) },
            { "HansyokuMF", typeof(string) },
            { "HansyokuMM", typeof(string) },
            { "BanusiCode", typeof(string) },
            { "BanusiName", typeof(string) },
            { "RuikeiHonsyokin", typeof(int) },
            { "RuikeiSyutokuPt", typeof(int) },
            { "RuikeiSyussoKaisu", typeof(int) },
            // 着回数情報を適切に展開
            // 総合着回数（ChakuSogo）
            { "ChakuSogo_ChakuKaisu_0", typeof(int) },  // 1着回数
            { "ChakuSogo_ChakuKaisu_1", typeof(int) },  // 2着回数
            { "ChakuSogo_ChakuKaisu_2", typeof(int) },  // 3着回数
            { "ChakuSogo_ChakuKaisu_3", typeof(int) },  // 4着回数
            { "ChakuSogo_ChakuKaisu_4", typeof(int) },  // 5着回数
            { "ChakuSogo_ChakuKaisu_5", typeof(int) },  // 出走回数
            
            // 中央合計着回数（ChakuChuo）
            { "ChakuChuo_ChakuKaisu_0", typeof(int) },  // 1着回数
            { "ChakuChuo_ChakuKaisu_1", typeof(int) },  // 2着回数
            { "ChakuChuo_ChakuKaisu_2", typeof(int) },  // 3着回数
            { "ChakuChuo_ChakuKaisu_3", typeof(int) },  // 4着回数
            { "ChakuChuo_ChakuKaisu_4", typeof(int) },  // 5着回数
            { "ChakuChuo_ChakuKaisu_5", typeof(int) },  // 出走回数
            
            // 馬場別着回数（ChakuKaisuBa[7]）
            { "ChakuKaisuBa_0_ChakuKaisu_0", typeof(int) },  // 芝1着
            { "ChakuKaisuBa_0_ChakuKaisu_1", typeof(int) },  // 芝2着
            { "ChakuKaisuBa_0_ChakuKaisu_2", typeof(int) },  // 芝3着
            { "ChakuKaisuBa_0_ChakuKaisu_3", typeof(int) },  // 芝4着
            { "ChakuKaisuBa_0_ChakuKaisu_4", typeof(int) },  // 芝5着
            { "ChakuKaisuBa_0_ChakuKaisu_5", typeof(int) },  // 芝出走
            { "ChakuKaisuBa_1_ChakuKaisu_0", typeof(int) },  // ダート1着
            { "ChakuKaisuBa_1_ChakuKaisu_1", typeof(int) },  // ダート2着
            { "ChakuKaisuBa_1_ChakuKaisu_2", typeof(int) },  // ダート3着
            { "ChakuKaisuBa_1_ChakuKaisu_3", typeof(int) },  // ダート4着
            { "ChakuKaisuBa_1_ChakuKaisu_4", typeof(int) },  // ダート5着
            { "ChakuKaisuBa_1_ChakuKaisu_5", typeof(int) },  // ダート出走
            { "ChakuKaisuBa_2_ChakuKaisu_0", typeof(int) },  // 障害1着
            { "ChakuKaisuBa_2_ChakuKaisu_1", typeof(int) },  // 障害2着
            { "ChakuKaisuBa_2_ChakuKaisu_2", typeof(int) },  // 障害3着
            { "ChakuKaisuBa_2_ChakuKaisu_3", typeof(int) },  // 障害4着
            { "ChakuKaisuBa_2_ChakuKaisu_4", typeof(int) },  // 障害5着
            { "ChakuKaisuBa_2_ChakuKaisu_5", typeof(int) },  // 障害出走
            { "ChakuKaisuBa_3_ChakuKaisu_0", typeof(int) },  // 馬場別3-1着
            { "ChakuKaisuBa_3_ChakuKaisu_1", typeof(int) },  // 馬場別3-2着
            { "ChakuKaisuBa_3_ChakuKaisu_2", typeof(int) },  // 馬場別3-3着
            { "ChakuKaisuBa_3_ChakuKaisu_3", typeof(int) },  // 馬場別3-4着
            { "ChakuKaisuBa_3_ChakuKaisu_4", typeof(int) },  // 馬場別3-5着
            { "ChakuKaisuBa_3_ChakuKaisu_5", typeof(int) },  // 馬場別3-出走
            { "ChakuKaisuBa_4_ChakuKaisu_0", typeof(int) },  // 馬場別4-1着
            { "ChakuKaisuBa_4_ChakuKaisu_1", typeof(int) },  // 馬場別4-2着
            { "ChakuKaisuBa_4_ChakuKaisu_2", typeof(int) },  // 馬場別4-3着
            { "ChakuKaisuBa_4_ChakuKaisu_3", typeof(int) },  // 馬場別4-4着
            { "ChakuKaisuBa_4_ChakuKaisu_4", typeof(int) },  // 馬場別4-5着
            { "ChakuKaisuBa_4_ChakuKaisu_5", typeof(int) },  // 馬場別4-出走
            { "ChakuKaisuBa_5_ChakuKaisu_0", typeof(int) },  // 馬場別5-1着
            { "ChakuKaisuBa_5_ChakuKaisu_1", typeof(int) },  // 馬場別5-2着
            { "ChakuKaisuBa_5_ChakuKaisu_2", typeof(int) },  // 馬場別5-3着
            { "ChakuKaisuBa_5_ChakuKaisu_3", typeof(int) },  // 馬場別5-4着
            { "ChakuKaisuBa_5_ChakuKaisu_4", typeof(int) },  // 馬場別5-5着
            { "ChakuKaisuBa_5_ChakuKaisu_5", typeof(int) },  // 馬場別5-出走
            { "ChakuKaisuBa_6_ChakuKaisu_0", typeof(int) },  // 馬場別6-1着
            { "ChakuKaisuBa_6_ChakuKaisu_1", typeof(int) },  // 馬場別6-2着
            { "ChakuKaisuBa_6_ChakuKaisu_2", typeof(int) },  // 馬場別6-3着
            { "ChakuKaisuBa_6_ChakuKaisu_3", typeof(int) },  // 馬場別6-4着
            { "ChakuKaisuBa_6_ChakuKaisu_4", typeof(int) },  // 馬場別6-5着
            { "ChakuKaisuBa_6_ChakuKaisu_5", typeof(int) },  // 馬場別6-出走
            
            // 馬場状態別着回数（ChakuKaisuJyotai[12]）
            { "ChakuKaisuJyotai_0_ChakuKaisu_0", typeof(int) },  // 状態0-1着
            { "ChakuKaisuJyotai_0_ChakuKaisu_1", typeof(int) },  // 状態0-2着
            { "ChakuKaisuJyotai_0_ChakuKaisu_2", typeof(int) },  // 状態0-3着
            { "ChakuKaisuJyotai_0_ChakuKaisu_3", typeof(int) },  // 状態0-4着
            { "ChakuKaisuJyotai_0_ChakuKaisu_4", typeof(int) },  // 状態0-5着
            { "ChakuKaisuJyotai_0_ChakuKaisu_5", typeof(int) },  // 状態0-出走
            { "ChakuKaisuJyotai_1_ChakuKaisu_0", typeof(int) },  // 状態1-1着
            { "ChakuKaisuJyotai_1_ChakuKaisu_1", typeof(int) },  // 状態1-2着
            { "ChakuKaisuJyotai_1_ChakuKaisu_2", typeof(int) },  // 状態1-3着
            { "ChakuKaisuJyotai_1_ChakuKaisu_3", typeof(int) },  // 状態1-4着
            { "ChakuKaisuJyotai_1_ChakuKaisu_4", typeof(int) },  // 状態1-5着
            { "ChakuKaisuJyotai_1_ChakuKaisu_5", typeof(int) },  // 状態1-出走
            { "ChakuKaisuJyotai_2_ChakuKaisu_0", typeof(int) },  // 状態2-1着
            { "ChakuKaisuJyotai_2_ChakuKaisu_1", typeof(int) },  // 状態2-2着
            { "ChakuKaisuJyotai_2_ChakuKaisu_2", typeof(int) },  // 状態2-3着
            { "ChakuKaisuJyotai_2_ChakuKaisu_3", typeof(int) },  // 状態2-4着
            { "ChakuKaisuJyotai_2_ChakuKaisu_4", typeof(int) },  // 状態2-5着
            { "ChakuKaisuJyotai_2_ChakuKaisu_5", typeof(int) },  // 状態2-出走
            { "ChakuKaisuJyotai_3_ChakuKaisu_0", typeof(int) },  // 状態3-1着
            { "ChakuKaisuJyotai_3_ChakuKaisu_1", typeof(int) },  // 状態3-2着
            { "ChakuKaisuJyotai_3_ChakuKaisu_2", typeof(int) },  // 状態3-3着
            { "ChakuKaisuJyotai_3_ChakuKaisu_3", typeof(int) },  // 状態3-4着
            { "ChakuKaisuJyotai_3_ChakuKaisu_4", typeof(int) },  // 状態3-5着
            { "ChakuKaisuJyotai_3_ChakuKaisu_5", typeof(int) },  // 状態3-出走
            { "ChakuKaisuJyotai_4_ChakuKaisu_0", typeof(int) },  // 状態4-1着
            { "ChakuKaisuJyotai_4_ChakuKaisu_1", typeof(int) },  // 状態4-2着
            { "ChakuKaisuJyotai_4_ChakuKaisu_2", typeof(int) },  // 状態4-3着
            { "ChakuKaisuJyotai_4_ChakuKaisu_3", typeof(int) },  // 状態4-4着
            { "ChakuKaisuJyotai_4_ChakuKaisu_4", typeof(int) },  // 状態4-5着
            { "ChakuKaisuJyotai_4_ChakuKaisu_5", typeof(int) },  // 状態4-出走
            { "ChakuKaisuJyotai_5_ChakuKaisu_0", typeof(int) },  // 状態5-1着
            { "ChakuKaisuJyotai_5_ChakuKaisu_1", typeof(int) },  // 状態5-2着
            { "ChakuKaisuJyotai_5_ChakuKaisu_2", typeof(int) },  // 状態5-3着
            { "ChakuKaisuJyotai_5_ChakuKaisu_3", typeof(int) },  // 状態5-4着
            { "ChakuKaisuJyotai_5_ChakuKaisu_4", typeof(int) },  // 状態5-5着
            { "ChakuKaisuJyotai_5_ChakuKaisu_5", typeof(int) },  // 状態5-出走
            { "ChakuKaisuJyotai_6_ChakuKaisu_0", typeof(int) },  // 状態6-1着
            { "ChakuKaisuJyotai_6_ChakuKaisu_1", typeof(int) },  // 状態6-2着
            { "ChakuKaisuJyotai_6_ChakuKaisu_2", typeof(int) },  // 状態6-3着
            { "ChakuKaisuJyotai_6_ChakuKaisu_3", typeof(int) },  // 状態6-4着
            { "ChakuKaisuJyotai_6_ChakuKaisu_4", typeof(int) },  // 状態6-5着
            { "ChakuKaisuJyotai_6_ChakuKaisu_5", typeof(int) },  // 状態6-出走
            { "ChakuKaisuJyotai_7_ChakuKaisu_0", typeof(int) },  // 状態7-1着
            { "ChakuKaisuJyotai_7_ChakuKaisu_1", typeof(int) },  // 状態7-2着
            { "ChakuKaisuJyotai_7_ChakuKaisu_2", typeof(int) },  // 状態7-3着
            { "ChakuKaisuJyotai_7_ChakuKaisu_3", typeof(int) },  // 状態7-4着
            { "ChakuKaisuJyotai_7_ChakuKaisu_4", typeof(int) },  // 状態7-5着
            { "ChakuKaisuJyotai_7_ChakuKaisu_5", typeof(int) },  // 状態7-出走
            { "ChakuKaisuJyotai_8_ChakuKaisu_0", typeof(int) },  // 状態8-1着
            { "ChakuKaisuJyotai_8_ChakuKaisu_1", typeof(int) },  // 状態8-2着
            { "ChakuKaisuJyotai_8_ChakuKaisu_2", typeof(int) },  // 状態8-3着
            { "ChakuKaisuJyotai_8_ChakuKaisu_3", typeof(int) },  // 状態8-4着
            { "ChakuKaisuJyotai_8_ChakuKaisu_4", typeof(int) },  // 状態8-5着
            { "ChakuKaisuJyotai_8_ChakuKaisu_5", typeof(int) },  // 状態8-出走
            { "ChakuKaisuJyotai_9_ChakuKaisu_0", typeof(int) },  // 状態9-1着
            { "ChakuKaisuJyotai_9_ChakuKaisu_1", typeof(int) },  // 状態9-2着
            { "ChakuKaisuJyotai_9_ChakuKaisu_2", typeof(int) },  // 状態9-3着
            { "ChakuKaisuJyotai_9_ChakuKaisu_3", typeof(int) },  // 状態9-4着
            { "ChakuKaisuJyotai_9_ChakuKaisu_4", typeof(int) },  // 状態9-5着
            { "ChakuKaisuJyotai_9_ChakuKaisu_5", typeof(int) },  // 状態9-出走
            { "ChakuKaisuJyotai_10_ChakuKaisu_0", typeof(int) }, // 状態10-1着
            { "ChakuKaisuJyotai_10_ChakuKaisu_1", typeof(int) }, // 状態10-2着
            { "ChakuKaisuJyotai_10_ChakuKaisu_2", typeof(int) }, // 状態10-3着
            { "ChakuKaisuJyotai_10_ChakuKaisu_3", typeof(int) }, // 状態10-4着
            { "ChakuKaisuJyotai_10_ChakuKaisu_4", typeof(int) }, // 状態10-5着
            { "ChakuKaisuJyotai_10_ChakuKaisu_5", typeof(int) }, // 状態10-出走
            { "ChakuKaisuJyotai_11_ChakuKaisu_0", typeof(int) }, // 状態11-1着
            { "ChakuKaisuJyotai_11_ChakuKaisu_1", typeof(int) }, // 状態11-2着
            { "ChakuKaisuJyotai_11_ChakuKaisu_2", typeof(int) }, // 状態11-3着
            { "ChakuKaisuJyotai_11_ChakuKaisu_3", typeof(int) }, // 状態11-4着
            { "ChakuKaisuJyotai_11_ChakuKaisu_4", typeof(int) }, // 状態11-5着
            { "ChakuKaisuJyotai_11_ChakuKaisu_5", typeof(int) }, // 状態11-出走
            
            // 距離別着回数（ChakuKaisuKyori[6]）
            { "ChakuKaisuKyori_0_ChakuKaisu_0", typeof(int) },   // 距離0-1着
            { "ChakuKaisuKyori_0_ChakuKaisu_1", typeof(int) },   // 距離0-2着
            { "ChakuKaisuKyori_0_ChakuKaisu_2", typeof(int) },   // 距離0-3着
            { "ChakuKaisuKyori_0_ChakuKaisu_3", typeof(int) },   // 距離0-4着
            { "ChakuKaisuKyori_0_ChakuKaisu_4", typeof(int) },   // 距離0-5着
            { "ChakuKaisuKyori_0_ChakuKaisu_5", typeof(int) },   // 距離0-出走
            { "ChakuKaisuKyori_1_ChakuKaisu_0", typeof(int) },   // 距離1-1着
            { "ChakuKaisuKyori_1_ChakuKaisu_1", typeof(int) },   // 距離1-2着
            { "ChakuKaisuKyori_1_ChakuKaisu_2", typeof(int) },   // 距離1-3着
            { "ChakuKaisuKyori_1_ChakuKaisu_3", typeof(int) },   // 距離1-4着
            { "ChakuKaisuKyori_1_ChakuKaisu_4", typeof(int) },   // 距離1-5着
            { "ChakuKaisuKyori_1_ChakuKaisu_5", typeof(int) },   // 距離1-出走
            { "ChakuKaisuKyori_2_ChakuKaisu_0", typeof(int) },   // 距離2-1着
            { "ChakuKaisuKyori_2_ChakuKaisu_1", typeof(int) },   // 距離2-2着
            { "ChakuKaisuKyori_2_ChakuKaisu_2", typeof(int) },   // 距離2-3着
            { "ChakuKaisuKyori_2_ChakuKaisu_3", typeof(int) },   // 距離2-4着
            { "ChakuKaisuKyori_2_ChakuKaisu_4", typeof(int) },   // 距離2-5着
            { "ChakuKaisuKyori_2_ChakuKaisu_5", typeof(int) },   // 距離2-出走
            { "ChakuKaisuKyori_3_ChakuKaisu_0", typeof(int) },   // 距離3-1着
            { "ChakuKaisuKyori_3_ChakuKaisu_1", typeof(int) },   // 距離3-2着
            { "ChakuKaisuKyori_3_ChakuKaisu_2", typeof(int) },   // 距離3-3着
            { "ChakuKaisuKyori_3_ChakuKaisu_3", typeof(int) },   // 距離3-4着
            { "ChakuKaisuKyori_3_ChakuKaisu_4", typeof(int) },   // 距離3-5着
            { "ChakuKaisuKyori_3_ChakuKaisu_5", typeof(int) },   // 距離3-出走
            { "ChakuKaisuKyori_4_ChakuKaisu_0", typeof(int) },   // 距離4-1着
            { "ChakuKaisuKyori_4_ChakuKaisu_1", typeof(int) },   // 距離4-2着
            { "ChakuKaisuKyori_4_ChakuKaisu_2", typeof(int) },   // 距離4-3着
            { "ChakuKaisuKyori_4_ChakuKaisu_3", typeof(int) },   // 距離4-4着
            { "ChakuKaisuKyori_4_ChakuKaisu_4", typeof(int) },   // 距離4-5着
            { "ChakuKaisuKyori_4_ChakuKaisu_5", typeof(int) },   // 距離4-出走
            { "ChakuKaisuKyori_5_ChakuKaisu_0", typeof(int) },   // 距離5-1着
            { "ChakuKaisuKyori_5_ChakuKaisu_1", typeof(int) },   // 距離5-2着
            { "ChakuKaisuKyori_5_ChakuKaisu_2", typeof(int) },   // 距離5-3着
            { "ChakuKaisuKyori_5_ChakuKaisu_3", typeof(int) },   // 距離5-4着
            { "ChakuKaisuKyori_5_ChakuKaisu_4", typeof(int) },   // 距離5-5着
            { "ChakuKaisuKyori_5_ChakuKaisu_5", typeof(int) },   // 距離5-出走
            
            // 脚質傾向（Kyakusitu[4]）
            { "Kyakusitu_0", typeof(string) },                  // 逃げ傾向
            { "Kyakusitu_1", typeof(string) },                  // 先行傾向
            { "Kyakusitu_2", typeof(string) },                  // 差し傾向
            { "Kyakusitu_3", typeof(string) },                  // 追込傾向
            
            // ヘッダー情報
            { "head_RecordSpec", typeof(string) },
            { "head_DataKubun", typeof(string) },
            { "head_MakeDate_Year", typeof(int) },
            { "head_MakeDate_Month", typeof(int) },
            { "head_MakeDate_Day", typeof(int) },
        };
        
        public override List<string> IndexColumns => new List<string> 
        { 
            "KettoNum"
        };
    }
}