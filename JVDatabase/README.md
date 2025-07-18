# JVDatabase - DuckDBベースのJRA-VANデータベース管理ツール

JRA-VANデータをダウンロードし、DuckDBを使用して高速にParquet形式へ変換・管理するツールです。

## 特徴

- **高速変換**: DuckDBによる並列処理でJVデータを高速にParquet形式へ変換
- **効率的なストレージ**: Parquet形式とHiveスタイルのパーティショニングで効率的なデータ保存
- **重複排除**: 各レコードタイプのプライマリキーに基づいた自動重複排除
- **インクリメンタル更新**: 最終更新タイムスタンプを記録し、差分更新に対応
- **37種類のレコードタイプ**: JRA-VANの全レコードタイプに対応
- **リアルタイムデータ対応**: 速報系データ（オッズ、票数、結果）の取得に対応
- **時系列データ管理**: オッズの時系列変化を追跡可能

## インストール

### 前提条件

- .NET 8.0 SDK
- JRA-VAN Data Lab（インストール・初期設定済み）
- JVDownloader.exe（同梱、要ビルド）
- JVDuckDB.exe（同梱、要ビルド）
- Windows OS（JRA-VAN Data LabのCOM制約）

### ビルド手順

```bash
# ソリューション全体をビルド（推奨）
dotnet build JVConsole.sln

# または、個別にビルド
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Release
dotnet build JVDuckDB/JVDuckDB.csproj -c Release
dotnet build JVDatabase/JVDatabase.csproj -c Release
```

## 実行方法

### 基本的な実行方法

```bash
# プロジェクトディレクトリから実行
dotnet run --project JVDatabase/JVDatabase.csproj [コマンド] [オプション]

# ビルド済みの実行ファイルを直接実行
cd JVDatabase/bin/Release/net8.0
./JVDatabase.exe [コマンド] [オプション]
```

### 利用可能なコマンド

- `config` - 設定の表示・変更
- `init` - データベースの初期化
- `update` - データベースの更新
- `realtime` - リアルタイムデータの取得
- `fetch-odds` - 時系列オッズデータの取得
- `analyze` - データ分析（要Python環境）

## 使用方法

### 1. 設定管理（config）

現在の設定を表示したり、デフォルト値を変更します。

#### オプション
- `-l, --list` - 現在の設定を表示

#### 実行例

```bash
# 現在の設定を表示
dotnet run --project JVDatabase/JVDatabase.csproj config -l

# 出力例：
# 現在の設定:
#   デフォルトパス: C:\Users\user\Documents\JVDatabase
#   デフォルトデータ種別: TOKU,RACE,SLOP,WOOD,YSCH,HOYU,MING,SNPN,HOSN,DIFN
#   スキップレコード: H6,O6
#   自動バックアップ: 無効
#   設定ファイル: C:\Users\user\AppData\Roaming\JVDatabase\config.json

# 対話的に設定を変更
dotnet run --project JVDatabase/JVDatabase.csproj config

# 対話例：
# JVDatabase設定を変更します（Enterでスキップ）
# 
# デフォルトデータベースパス [C:\Users\user\Documents\JVDatabase]:
# D:\JVData
# デフォルトデータ種別 [TOKU,RACE,SLOP,WOOD,YSCH,HOYU,MING,SNPN,HOSN,DIFN]:
# RACE,DIFN
# スキップするレコード種別 [H6,O6]:
# （Enter）
# 自動バックアップ (y/n) [n]:
# y
# 
# 設定を保存しました。
```

### 2. 初期化（init）

データベースを初期化し、指定期間のデータをダウンロード・変換します。

#### オプション
- `-i, --interactive` - 対話モードで設定
- `-p, --path` - データベースの保存パス（デフォルト: ./jvdb）
- `-s, --start` - 開始日（YYYYMMDD形式）
- `-e, --end` - 終了日（YYYYMMDD形式）
- `-d, --dataspec` - 取得するデータ種別（カンマ区切り）
- `--skip` - スキップするレコード種別（デフォルト: H6,O6）

#### 実行例

```bash
# 1. 対話モードで初期化（初心者向け）
dotnet run --project JVDatabase/JVDatabase.csproj init -i

# 対話例：
# JVDatabase初期化設定
# ===================
# 
# データベースの保存場所 [C:\Users\user\Documents\JVDatabase]:
# D:\JVData
# 
# データ取得開始日 (YYYYMMDD) [20231216]:
# 20240101
# 
# データ取得終了日 (YYYYMMDD) [20241216]:
# 20241231
# 
# 取得するデータ種別（カンマ区切り）:
# [TOKU,RACE,SLOP,WOOD,YSCH,HOYU,MING,SNPN,HOSN,DIFN]
#   TOKU: 特別登録馬
#   RACE: レース情報（必須）
#   SLOP: 坂路調教
#   WOOD: ウッドチップ調教
#   YSCH: 予想（騎手、厩舎、馬、総合）
#   HOYU: 馬主
#   MING: マイニング予想
#   SNPN: SNAP
#   HOSN: 競走馬市場取引価格
#   DIFN: マスター系
#   その他: BLDN（血統）など
# RACE,DIFN
# 
# 設定内容の確認:
#   保存場所: D:\JVData
#   期間: 20240101 ～ 20241231
#   データ種別: RACE, DIFN
#   推定サイズ: 約5GB
# この設定で初期化を開始しますか？ (y/N):
# y

# 2. コマンドラインで初期化（2024年のレースデータ）
dotnet run --project JVDatabase/JVDatabase.csproj init -p D:\JVData -s 20240101 -e 20241231 -d RACE

# 3. 最小限のデータで初期化（直近3ヶ月のレース情報のみ）
$startDate = (Get-Date).AddMonths(-3).ToString("yyyyMMdd")
$endDate = (Get-Date).ToString("yyyyMMdd")
dotnet run --project JVDatabase/JVDatabase.csproj init -s $startDate -e $endDate -d RACE

# 4. 全データ種別を取得（大容量注意）
dotnet run --project JVDatabase/JVDatabase.csproj init -s 20230101 -e 20241231 -d TOKU,RACE,DIFN,BLDN,SNPN,SLOP,WOOD,YSCH,HOSN,HOYU,MING
```

### 3. データベース更新（update）

データベースを最新の状態に更新します。前回の更新以降の差分データのみをダウンロード・変換します。

#### オプション
- `-p, --path` - データベースのパス（デフォルト: ./jvdb）
- `-e, --end` - 終了日（YYYYMMDD形式、省略時は今日）

#### 実行例

```bash
# 1. デフォルトパスのデータベースを最新に更新
dotnet run --project JVDatabase/JVDatabase.csproj update

# 出力例：
# 最終更新日時: 2024-12-10 05:30:00 UTC
# 
# [1/3] 差分データをダウンロード中...
# 
#   RACEのデータを確認中...
#     最終タイムスタンプ: 20241210053000
#     新規ファイル: 5個
# 
#   DIFNのデータを確認中...
#     最終タイムスタンプ: 20241209120000
#     新規ファイル: 1個
# 
# [2/3] 6個の新しいファイルをParquet形式に変換中...
# 
# 変換中: 1/6 - JV-RACE-20241210-0600.txt
#   → 完了 (2.3秒)
# ...
# 
# [3/3] メタデータを更新中...
# 
# データベース更新が完了しました。
# 更新ファイル数: 6

# 2. 特定のパスのデータベースを更新
dotnet run --project JVDatabase/JVDatabase.csproj update -p D:\JVData

# 3. 特定の日付まで更新（過去データの取得）
dotnet run --project JVDatabase/JVDatabase.csproj update -e 20240630
```

### 4. リアルタイムデータ取得（realtime）

速報系データ（成績、オッズ、票数など）を取得します。開催当日のデータ取得に使用します。

#### オプション
- `-p, --path` - データベースのパス（デフォルト: ./jvdb）
- `-t, --target` - 対象日（YYYYMMDD形式、省略時は今日）
- `-d, --dataspec` - 取得するデータ種別（カンマ区切り、デフォルト: 0B15,0B30,0B11）

#### データ種別（速報系）
- `0B11` - 票数（全賭式）
- `0B12` - 票数（単複枠）
- `0B15` - レース結果（速報）
- `0B30` - 馬体重（速報）
- `0B31` - 単複枠最終オッズ
- `0B32` - 馬連最終オッズ
- `0B33` - ワイド最終オッズ
- `0B34` - 馬単最終オッズ
- `0B35` - 三連複最終オッズ
- `0B36` - 三連単最終オッズ

#### 実行例

```bash
# 1. 今日の速報データを取得（デフォルト）
dotnet run --project JVDatabase/JVDatabase.csproj realtime

# 2. 特定日の全オッズデータを取得
dotnet run --project JVDatabase/JVDatabase.csproj realtime -t 20241215 -d 0B31,0B32,0B33,0B34,0B35,0B36

# 3. レース結果と馬体重のみ取得
dotnet run --project JVDatabase/JVDatabase.csproj realtime -t 20241215 -d 0B15,0B30

# 出力例：
# リアルタイムデータ取得を開始します...
# 対象日: 20241215
# データ種別: 0B15, 0B30
# 
# [0B15] データを取得中...
#   実行コマンド: JVDownloader.exe jvrt --dataspec 0B15 --key 20241215 --outputDir D:\JVData\realtime\raw
#   → 36レース分のデータを取得しました
# 
# [0B30] データを取得中...
#   実行コマンド: JVDownloader.exe jvrt --dataspec 0B30 --key 20241215 --outputDir D:\JVData\realtime\raw
#   → 36レース分のデータを取得しました
# 
# 72個のリアルタイムファイルをParquet形式に変換中...
# 変換完了: 72/72
# 
# リアルタイムデータ取得が完了しました。
# 保存先: D:\JVData\realtime\parquet
```

### 5. 時系列オッズデータ取得（fetch-odds）

指定日の時系列オッズデータを取得します。オッズの推移を分析する際に使用します。

#### オプション
- `-p, --path` - データベースのパス（デフォルト: ./jvdb）
- `-d, --date` - 対象日（YYYYMMDD形式）**必須**
- `-s, --spec` - データ種別（デフォルト: 0B41）

#### 時系列データ種別
- `0B11` - 票数時系列（全賭式）
- `0B12` - 票数時系列（単複枠）
- `0B41` - 単複枠時系列オッズ
- `0B42` - 馬連時系列オッズ

#### 実行例

```bash
# 1. 指定日の単複枠時系列オッズを取得（デフォルト）
dotnet run --project JVDatabase/JVDatabase.csproj fetch-odds -d 20241215

# 出力例：
# 指定日のオッズデータを取得します: 20241215
# 
# 3個のRAファイルが見つかりました。
# 
# 開催場数: 3
#   05: 東京 (12レース)
#   09: 阪神 (12レース)
#   10: 小倉 (12レース)
# 
# [東京] 0B41データを取得中...
#   → 12レース分のデータを取得しました
# 
# [阪神] 0B41データを取得中...
#   → 12レース分のデータを取得しました
# 
# [小倉] 0B41データを取得中...
#   → 12レース分のデータを取得しました
# 
# 36個のファイルをParquet形式に変換中...
# 変換完了: 36/36
# 
# 20241215のオッズデータ取得が完了しました。

# 2. 馬連時系列オッズを取得
dotnet run --project JVDatabase/JVDatabase.csproj fetch-odds -d 20241215 -s 0B42

# 3. 票数時系列データを取得（投票動向分析用）
dotnet run --project JVDatabase/JVDatabase.csproj fetch-odds -d 20241215 -s 0B11
```

## クイックスタート

### 初回セットアップの完全な例

```bash
# 1. プロジェクトルートに移動
cd C:\Users\user\ghq\github.com\satomacoto\JVConsole

# 2. ソリューション全体をビルド
dotnet build JVConsole.sln -c Release

# 3. 対話モードで初期化（推奨）
dotnet run --project JVDatabase/JVDatabase.csproj init -i

# 4. 最新データに更新
dotnet run --project JVDatabase/JVDatabase.csproj update

# 5. 今日のリアルタイムデータを取得
dotnet run --project JVDatabase/JVDatabase.csproj realtime
```

### 日次運用の例

```bash
# 毎朝：前日までのデータを更新
dotnet run --project JVDatabase/JVDatabase.csproj update

# レース当日：リアルタイムデータを取得
dotnet run --project JVDatabase/JVDatabase.csproj realtime

# レース後：時系列オッズデータを取得
$today = (Get-Date).ToString("yyyyMMdd")
dotnet run --project JVDatabase/JVDatabase.csproj fetch-odds -d $today
```

### 6. 分析（analyze）

データベースの内容を分析します。Pythonスクリプトとの連携が必要です。

#### オプション
- `-p, --path` - データベースのパス（デフォルト: ./jvdb）
- `-c, --command` - 分析コマンド（必須）
  - `realtime-odds` - リアルタイムオッズの分析
  - `race-stats` - レース統計の生成
- `-s, --start` - 開始日（YYYYMMDD形式）
- `-e, --end` - 終了日（YYYYMMDD形式）
- `--spec` - レコード種別（O1, O2等）
- `-m, --minutes` - レース発走何分前のデータか（デフォルト: 10）
- `-o, --output` - 出力ファイルパス

#### 実行例

```bash
# 1. リアルタイムオッズの分析（発走5分前）
dotnet run --project JVDatabase/JVDatabase.csproj analyze -c realtime-odds --spec O1 -m 5

# 2. レース統計の生成（2024年11月〜12月）
dotnet run --project JVDatabase/JVDatabase.csproj analyze -c race-stats -s 20241101 -e 20241231 -o stats_2024_nov_dec.csv

# 3. 月間レース分析レポート
dotnet run --project JVDatabase/JVDatabase.csproj analyze -c race-stats -s 20241201 -e 20241231 -o december_report.csv
```

## データ形式

### 出力ディレクトリ構成

```
jvdb/
├── .jvdb/                    # メタデータ
│   ├── metadata.json         # データベース情報
│   └── jvdb.duckdb          # DuckDB作業用データベース
├── raw/                      # ダウンロードした生データ
│   ├── JV-RACE-*.txt
│   └── JV-SLOP-*.txt
├── parquet/                  # Parquet形式データ
│   ├── RA/                   # レース情報
│   │   └── head_MakeDate_Year=2024/
│   │       └── head_MakeDate_Month=12/
│   │           └── head_MakeDate_Day=25/
│   │               └── data_0.parquet
│   ├── SE/                   # 馬毎成績
│   ├── UM/                   # 競走馬マスタ
│   └── _index/               # インデックステーブル
│       └── jv_partition_index.parquet
└── realtime/                 # リアルタイムデータ
    ├── raw/
    └── parquet/
```

### パーティショニング

データは`MakeDate`（データ作成日）に基づいてHiveスタイルでパーティショニングされます：
- `head_MakeDate_Year=YYYY`
- `head_MakeDate_Month=MM`
- `head_MakeDate_Day=DD`

### インデックステーブル

`_index/jv_partition_index.parquet`には各パーティションの情報が記録されています：
- `record_spec`: レコードタイプ
- `partition_path`: パーティションのパス
- `record_count`: レコード数
- `last_updated`: 最終更新日時

## データ種別

### 蓄積系データ（dataspec）

#### 必須データ
- `RACE` - レース関連情報
  - RA: レース詳細
  - SE: 馬毎レース情報
  - HR: 払戻
  - H1: 単勝・複勝票数
  - H6: 全賭式票数（※大容量）
  - O1-O6: 各種オッズ（※大容量）
  - WF: ワイド払戻
  - JG: 重勝式
- `DIFN` - マスター系データ
  - UM: 競走馬マスタ
  - KS: 騎手マスタ
  - CH: 調教師マスタ
  - BR: 生産者マスタ
  - BN: 馬主マスタ
  - RC: レコードマスタ

#### オプションデータ
- `TOKU` - 特別登録馬（TK）
- `SLOP` - 坂路調教（HC）
- `WOOD` - ウッドチップ調教（WC）
- `BLDN` - 血統情報（BT,HN,SK）
- `YSCH` - 予想（YS）
- `HOYU` - 馬主（HY）
- `MING` - マイニング（TM,DM）
- `SNPN` - SNAP（SN）
- `HOSN` - 競走馬市場取引価格（HS）

### 速報系データ（リアルタイム）

#### レース情報
- `0B15` - レース結果（速報）
- `0B30` - 馬体重（速報）

#### オッズ・票数（最終）
- `0B31` - 単複枠最終オッズ
- `0B32` - 馬連最終オッズ
- `0B33` - ワイド最終オッズ
- `0B34` - 馬単最終オッズ
- `0B35` - 三連複最終オッズ
- `0B36` - 三連単最終オッズ

#### 時系列データ
- `0B11` - 票数時系列（全賭式）
- `0B12` - 票数時系列（単複枠）
- `0B41` - 単複枠時系列オッズ
- `0B42` - 馬連時系列オッズ

## レコードタイプ一覧

### レース関連（13種類）
- `RA` - レース詳細（開催情報、出走条件、賞金等）
- `SE` - 馬毎レース情報（出走馬、騎手、負担重量等）
- `HR` - 払戻（単勝・複勝・枠連・馬連）
- `H1` - 単勝・複勝・枠連票数
- `H6` - 全賭式票数（馬連・ワイド・馬単・三連複・三連単）
- `O1` - 単勝・複勝・枠連オッズ
- `O2` - 馬連オッズ
- `O3` - ワイドオッズ
- `O4` - 馬単オッズ
- `O5` - 三連複オッズ
- `O6` - 三連単オッズ
- `WF` - ワイド払戻
- `JG` - 重勝式（WIN5等）

### マスター系（9種類）
- `UM` - 競走馬マスタ（馬名、生年月日、血統等）
- `KS` - 騎手マスタ（騎手名、所属等）
- `CH` - 調教師マスタ（調教師名、所属等）
- `BR` - 生産者マスタ
- `BN` - 馬主マスタ
- `RC` - レコードマスタ（各競馬場のレコードタイム）
- `HN` - 繁殖牝馬マスタ
- `SK` - 産駒マスタ
- `BT` - 血統マスタ

### 調教・予想系（8種類）
- `HC` - 坂路調教
- `WC` - ウッドチップ調教
- `CC` - 併せ馬結果
- `TC` - 過去調教成績（開催週）
- `TK` - 特別登録馬
- `TM` - タイムランク（マイニング）
- `DM` - データマイニング予想
- `YS` - 予想（騎手・厩舎・馬・総合）

### その他（7種類）
- `AV` - IPAT購入済み情報
- `JC` - 成績（競走馬・騎手・調教師・馬主・生産者）
- `WH` - 出走取消・競走除外
- `WE` - 天候・馬場状態変更
- `HS` - 競走馬市場取引価格
- `HY` - 馬主
- `SN` - SNAP

## 高度な使用方法

### Pythonでの分析

```python
import pandas as pd
import duckdb

# 1. DuckDBで直接クエリ
con = duckdb.connect()

# 東京競馬場の2024年のレース一覧
races = con.execute("""
    SELECT * 
    FROM read_parquet('jvdb/parquet/RA/**/*.parquet')
    WHERE id_JyoCD = '05'  -- 東京競馬場
      AND id_Year = '2024'
    ORDER BY id_Year, id_MonthDay, id_JyoCD, id_RaceNum
""").fetchdf()

# 2. 特定レースの出走馬情報を取得
race_id = '2024123105010'
horses = con.execute(f"""
    SELECT se.*, um.Bamei, um.BamieiKana
    FROM read_parquet('jvdb/parquet/SE/**/*.parquet') se
    LEFT JOIN read_parquet('jvdb/parquet/UM/**/*.parquet') um
      ON se.KettoNum = um.KettoNum
    WHERE se.id_Year || se.id_MonthDay || se.id_JyoCD || 
          LPAD(se.id_RaceNum, 2, '0') || se.Umaban = '{race_id}'
""").fetchdf()

# 3. オッズの時系列変化を分析
odds_history = con.execute("""
    SELECT 
        MakeTime,
        TansyoOdds,
        FukusyoOdds
    FROM read_parquet('jvdb/realtime/parquet/O1/**/*.parquet')
    WHERE id_Year = '2024' 
      AND id_MonthDay = '1215'
      AND id_JyoCD = '05'
      AND id_RaceNum = '11'
      AND Umaban = '01'
    ORDER BY MakeTime
""").fetchdf()

# 4. インデックステーブルでデータ量を確認
df_index = pd.read_parquet('jvdb/parquet/_index/jv_partition_index.parquet')
print("\nレコード種別ごとの件数:")
print(df_index.groupby('record_spec')['record_count'].sum().sort_values(ascending=False))

# 5. パーティション単位でのデータアクセス
# 2024年12月のRAデータのみ読み込み
ra_202412 = pd.read_parquet('jvdb/parquet/RA/head_MakeDate_Year=2024/head_MakeDate_Month=12/')
```

### DuckDBでの高度なクエリ

```sql
-- 1. 馬ごとの成績集計
CREATE OR REPLACE VIEW horse_stats AS
SELECT 
    se.KettoNum,
    um.Bamei,
    COUNT(*) as race_count,
    SUM(CASE WHEN se.KakuteiJyuni = '01' THEN 1 ELSE 0 END) as win_count,
    SUM(CASE WHEN se.KakuteiJyuni <= '03' THEN 1 ELSE 0 END) as place_count,
    AVG(CAST(se.KakuteiJyuni AS INTEGER)) as avg_position
FROM read_parquet('jvdb/parquet/SE/**/*.parquet') se
JOIN read_parquet('jvdb/parquet/UM/**/*.parquet') um ON se.KettoNum = um.KettoNum
WHERE se.KakuteiJyuni != '00'
GROUP BY se.KettoNum, um.Bamei;

-- 2. 騎手別の重賞成績
SELECT 
    ks.KisyuRyakusyo,
    COUNT(*) as g_race_count,
    SUM(CASE WHEN se.KakuteiJyuni = '01' THEN 1 ELSE 0 END) as g_win_count
FROM read_parquet('jvdb/parquet/SE/**/*.parquet') se
JOIN read_parquet('jvdb/parquet/RA/**/*.parquet') ra 
    ON se.id_Year = ra.id_Year 
    AND se.id_MonthDay = ra.id_MonthDay
    AND se.id_JyoCD = ra.id_JyoCD
    AND se.id_RaceNum = ra.id_RaceNum
JOIN read_parquet('jvdb/parquet/KS/**/*.parquet') ks ON se.KisyuCode = ks.KisyuCode
WHERE ra.GradeCD IN ('1', '2', '3')  -- G1, G2, G3
GROUP BY ks.KisyuRyakusyo
ORDER BY g_win_count DESC;
```

### 差分更新の仕組み

JVDatabaseは以下の情報を使用して効率的な差分更新を実現：

1. **LastFileTimestamp**: 各データ種別の最終更新タイムスタンプ
2. **ProcessedFiles**: 処理済みファイルのリスト
3. **DataSpecStatuses**: データ種別ごとの更新状態

```json
// metadata.jsonの例
{
  "DataSpecStatuses": {
    "RACE": {
      "LastFileTimestamp": "20241215120000",
      "LastFileName": "JV-RACE-20241215-1200.txt",
      "LastUpdatedAt": "2024-12-15T12:05:00Z"
    },
    "DIFN": {
      "LastFileTimestamp": "20241214180000",
      "LastFileName": "JV-DIFN-20241214-1800.txt",
      "LastUpdatedAt": "2024-12-14T18:10:00Z"
    }
  }
}
```

## トラブルシューティング

### よくある問題と解決方法

#### 1. JVDownloader.exeが見つからない

```bash
# エラー例：
# JVDownloader.exe が見つかりません
# 期待される場所: D:\JVData\JVDatabase\bin\Release\net8.0\JVDownloader.exe

# 解決方法：
# 1. ソリューション全体をビルド
dotnet build JVConsole.sln -c Release

# 2. または個別にビルド
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Release
```

#### 2. JRA-VAN Data Labの接続エラー

```bash
# エラー例：
# JV-Link初期化エラー: -111 (サーバーメンテナンス中)

# 解決方法：
# 1. JRA-VAN Data Labが起動しているか確認
# 2. ユーザーID・パスワードが正しいか確認
# 3. サーバーメンテナンス時間（毎週月曜 4:00-7:00）を避ける
```

#### 3. メモリ不足エラー

```bash
# エラー例：
# System.OutOfMemoryException

# 解決方法：
# 1. 環境変数でメモリ制限を設定
$env:DUCKDB_MEMORY_LIMIT = "4GB"

# 2. バッチサイズを調整（ConvertOptions.csで設定可能）
# デフォルト: 10000 → 5000に減らす
```

#### 4. 文字化け・エンコーディングエラー

```bash
# エラー例：
# 馬名や騎手名が文字化けする

# 解決方法：
# JVデータはShift_JISエンコーディング
# Program.csで自動的に処理されているが、
# 出力時はUTF-8に変換されることを確認
```

#### 5. パーティションが見つからない

```bash
# エラー例：
# 指定した日付のデータが見つからない

# 解決方法：
# 1. パーティション構造を確認
dir jvdb\parquet\RA\head_MakeDate_Year=2024\

# 2. インデックステーブルで確認
import pandas as pd
df_index = pd.read_parquet('jvdb/parquet/_index/jv_partition_index.parquet')
print(df_index[df_index['record_spec'] == 'RA'])
```

#### 6. 重複データの問題

```python
# 重複排除の仕組み
# RecordIndexMapping.csで各レコードタイプのプライマリキーを定義
# 例：RAレコードのプライマリキー
# - id_Year + id_MonthDay + id_JyoCD + id_Kaiji + id_Nichiji + id_RaceNum

# 重複を確認する方法
import duckdb
con = duckdb.connect()
duplicates = con.execute("""
    SELECT id_Year, id_MonthDay, id_JyoCD, id_RaceNum, COUNT(*) as cnt
    FROM read_parquet('jvdb/parquet/RA/**/*.parquet')
    GROUP BY id_Year, id_MonthDay, id_JyoCD, id_RaceNum
    HAVING COUNT(*) > 1
""").fetchdf()
```

### デバッグモード

```bash
# 詳細なログを出力
$env:JVDATABASE_DEBUG = "true"
dotnet run --project JVDatabase/JVDatabase.csproj update

# JVDuckDBの詳細出力
./JVDuckDB.exe -i input.txt -o output -v
```

## ライセンス

このプロジェクトはJRA-VANデータラボの利用規約に従います。
