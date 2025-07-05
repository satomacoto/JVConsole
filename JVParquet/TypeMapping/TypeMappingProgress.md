# JVParquet 型マッピング実装進捗

## 完了したタスク
- [x] 型マッピングの基盤クラス（IRecordTypeMapping）を作成
  - IRecordTypeMapping.cs
  - RecordTypeMappingBase.cs
  - TypeMappingManager.cs

## 残りのレコード種別（37種類）

### A-C
- [x] AV (JV_AV_INFO) - 出走取消・競走除外
- [x] BN (JV_BN_BANUSI) - 馬主情報
- [x] BR (JV_BR_BREEDER) - 生産者情報
- [ ] BT (JV_BT_KEITO) - 血統情報
- [ ] CC (JV_CC_INFO) - コース情報変更
- [ ] CH (JV_CH_CHOKYOSI) - 調教師情報
- [ ] CK (JV_CK_CHAKU) - チャック情報
- [ ] CS (JV_CS_COURSE) - コース情報

### D-H
- [ ] DM (JV_DM_INFO) - 競走馬市場取引情報
- [ ] H1 (JV_H1_HYOSU_ZENKAKE) - 票数（全確定）
- [ ] H6 (JV_H6_HYOSU_SANRENTAN) - 票数（3連単）
- [ ] HC (JV_HC_HANRO) - 繁路情報
- [ ] HN (JV_HN_HANSYOKU) - 繁殖情報
- [ ] HR (JV_HR_PAY) - 払戻情報
- [ ] HS (JV_HS_SALE) - 販売情報
- [ ] HY (JV_HY_BAMEIORIGIN) - 馬名由来情報

### J-O
- [ ] JC (JV_JC_INFO) - 情報変更
- [ ] JG (JV_JG_JOGAIBA) - 除外馬情報
- [ ] KS (JV_KS_KISYU) - 騎手情報
- [ ] O1 (JV_O1_ODDS_TANFUKUWAKU) - オッズ（単複枠）
- [ ] O2 (JV_O2_ODDS_UMAREN) - オッズ（馬連）
- [ ] O3 (JV_O3_ODDS_WIDE) - オッズ（ワイド）
- [ ] O4 (JV_O4_ODDS_UMATAN) - オッズ（馬単）
- [ ] O5 (JV_O5_ODDS_SANREN) - オッズ（3連複）
- [ ] O6 (JV_O6_ODDS_SANRENTAN) - オッズ（3連単）

### R-Y
- [ ] RA (JV_RA_RACE) - レース情報
- [ ] RC (JV_RC_RECORD) - レコード情報
- [ ] SE (JV_SE_RACE_UMA) - レース馬情報
- [ ] SK (JV_SK_SANKU) - 産駒情報
- [ ] TC (JV_TC_INFO) - 時計情報変更
- [ ] TK (JV_TK_TOKUUMA) - 特別登録馬情報
- [ ] TM (JV_TM_INFO) - タイム型情報
- [ ] UM (JV_UM_UMA) - 競走馬情報
- [ ] WC (JV_WC_WOOD) - ウッドトレーニング情報
- [ ] WE (JV_WE_WEATHER) - 天候情報
- [ ] WF (JV_WF_INFO) - Wi-Fi情報
- [ ] WH (JV_WH_BATAIJYU) - 馬体重情報
- [ ] YS (JV_YS_SCHEDULE) - 開催スケジュール

## 実装後のタスク
- [ ] ParquetWriterManagerの修正（型付きスキーマの生成）
- [ ] ReflectionFlattenerの修正（型情報の保持）
- [ ] 型変換ロジックのテスト