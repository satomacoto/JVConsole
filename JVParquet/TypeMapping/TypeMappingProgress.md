# JVParquet 型マッピング実装進捗

## 完了したタスク
- [x] 型マッピングの基盤クラス（IRecordTypeMapping）を作成
  - IRecordTypeMapping.cs
  - RecordTypeMappingBase.cs
  - TypeMappingManager.cs

## 完了：レコード種別（37種類）

### A-C
- [x] AV (JV_AV_INFO) - 出走取消・競走除外
- [x] BN (JV_BN_BANUSI) - 馬主情報
- [x] BR (JV_BR_BREEDER) - 生産者情報
- [x] BT (JV_BT_KEITO) - 系統情報
- [x] CC (JV_CC_INFO) - コース変更
- [x] CH (JV_CH_CHOKYOSI) - 調教師情報
- [x] CK (JV_CK_CHAKU) - 出走別着度数
- [x] CS (JV_CS_COURSE) - コース情報

### D-H
- [x] DM (JV_DM_INFO) - タイム型データマイニング予想
- [x] H1 (JV_H1_HYOSU_ZENKAKE) - 票数（全掛け）
- [x] H6 (JV_H6_HYOSU_SANRENTAN) - 票数（3連単）
- [x] HC (JV_HC_HANRO) - 坂路調教
- [x] HN (JV_HN_HANSYOKU) - 繁殖馬マスタ
- [x] HR (JV_HR_PAY) - 払戻情報
- [x] HS (JV_HS_SALE) - 馬主データ
- [x] HY (JV_HY_BAMEIORIGIN) - 馬名意味由来

### J-O
- [x] JC (JV_JC_INFO) - 競走馬市場取引価格
- [x] JG (JV_JG_JOGAIBA) - 除外馬情報
- [x] KS (JV_KS_KISYU) - 騎手情報
- [x] O1 (JV_O1_ODDS_TANFUKUWAKU) - オッズ（単複枠）
- [x] O2 (JV_O2_ODDS_UMAREN) - オッズ（馬連）
- [x] O3 (JV_O3_ODDS_WIDE) - オッズ（ワイド）
- [x] O4 (JV_O4_ODDS_UMATAN) - オッズ（馬単）
- [x] O5 (JV_O5_ODDS_SANREN) - オッズ（3連複）
- [x] O6 (JV_O6_ODDS_SANRENTAN) - オッズ（3連単）

### R-Y
- [x] RA (JV_RA_RACE) - レース詳細
- [x] RC (JV_RC_RECORD) - レースコメント
- [x] SE (JV_SE_RACE_UMA) - レース成績
- [x] SK (JV_SK_SANKU) - 産駒マスタ
- [x] TC (JV_TC_INFO) - 特別登録馬
- [x] TK (JV_TK_TOKUUMA) - 特別登録（抽選番号）
- [x] TM (JV_TM_INFO) - 対戦型データマイニング予想
- [x] UM (JV_UM_UMA) - 馬マスタ
- [x] WC (JV_WC_WOOD) - ウッドチップ調教
- [x] WE (JV_WE_WEATHER) - 天候・馬場状態
- [x] WF (JV_WF_INFO) - WIN5
- [x] WH (JV_WH_BATAIJYU) - WIN5払戻
- [x] YS (JV_YS_SCHEDULE) - 予想スケジュール

## 実装後のタスク
- [ ] ParquetWriterManagerの修正（型付きスキーマの生成）
- [ ] ReflectionFlattenerの修正（型情報の保持）
- [ ] 型変換ロジックのテスト