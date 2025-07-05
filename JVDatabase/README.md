# JVDatabase

JVDatabaseは、JRA-VAN Data Lab.の競馬データを効率的に管理するための統合型コマンドラインツールです。データのダウンロードからParquet形式への変換まで、一貫したワークフローを提供します。

## 主要コマンド

### 1. init - データベースの初期化

新しいJVDatabaseを作成し、過去の競馬データをダウンロードします。

#### 基本的な使い方

```bash
# 対話形式で初期化（推奨）
JVDatabase init -i

# パラメータを指定して初期化
JVDatabase init -p ./mydb -s 20230101 -e 20231231 -d RACE,DIFN
```

#### オプション

| オプション | 短縮形 | 説明 | デフォルト |
|-----------|--------|------|------------|
| `--path` | `-p` | データベースのパス | `./jvdb` |
| `--interactive` | `-i` | 対話モードで実行 | false |
| `--startdate` | `-s` | 開始日（YYYYMMDD） | 3年前の1月1日 |
| `--enddate` | `-e` | 終了日（YYYYMMDD） | 昨日 |
| `--dataspec` | `-d` | データ種別（カンマ区切り） | 全種別 |
| `--skip-large-odds` | | 大容量オッズデータをスキップ | false |

#### データ種別（DATASPEC）

- `RACE`: レース情報・成績
- `DIFN`: 馬・騎手・調教師マスタ
- `BLDN`: 血統マスタ
- `TOKU`: 特別登録馬
- `SLOP/WOOD`: 調教データ
- `YSCH`: 予想
- その他多数（詳細は`--help`参照）

#### 実行例

```bash
# 過去1年分のレースデータのみ初期化
JVDatabase init -s 20230101 -e 20231231 -d RACE

# 対話形式で全設定を確認しながら初期化
JVDatabase init -i
```

### 2. update - データベースの更新

既存のデータベースに最新データを追加します。

#### 基本的な使い方

```bash
# デフォルト設定で更新
JVDatabase update

# 特定の日付範囲で更新
JVDatabase update -s 20240101 -e 20240131
```

#### オプション

| オプション | 短縮形 | 説明 | デフォルト |
|-----------|--------|------|------------|
| `--path` | `-p` | データベースのパス | `./jvdb` |
| `--startdate` | `-s` | 開始日（YYYYMMDD） | 前回更新日の翌日 |
| `--enddate` | `-e` | 終了日（YYYYMMDD） | 昨日 |
| `--dataspec` | `-d` | データ種別（カンマ区切り） | initと同じ |

#### 実行例

```bash
# 前回更新以降の全データを取得
JVDatabase update

# 特定期間のレースデータのみ更新
JVDatabase update -s 20240101 -e 20240107 -d RACE
```

### 3. realtime - リアルタイムデータの取得

レース当日の速報データ（オッズ、成績、払戻等）を取得します。

#### 基本的な使い方

```bash
# 今日のリアルタイムデータを取得
JVDatabase realtime

# 特定日のデータを取得
JVDatabase realtime -t 20240113
```

#### オプション

| オプション | 短縮形 | 説明 | デフォルト |
|-----------|--------|------|------------|
| `--path` | `-p` | データベースのパス | `./jvdb` |
| `--target` | `-t` | 対象日（YYYYMMDD） | 今日 |
| `--dataspec` | `-d` | データ種別（カンマ区切り） | `0B15,0B30,0B11` |

#### リアルタイムデータ種別

- `0B15`: 速報レース情報（出走馬名表～）
- `0B30`: 速報オッズ（全賭式）
- `0B31`: 速報オッズ（単複枠）
- `0B32`: 速報オッズ（馬連）
- `0B33`: 速報オッズ（ワイド）
- `0B34`: 速報オッズ（馬単）
- `0B35`: 速報オッズ（3連複）
- `0B36`: 速報オッズ（3連単）
- `0B11`: 速報馬体重
- `0B20`: 速報票数（全賭式）

#### レース日の使用例

```bash
# 朝 - 初期オッズ確認
JVDatabase realtime -d 0B15,0B30

# レース中 - 全データ更新
JVDatabase realtime

# レース後 - 最終成績と払戻
JVDatabase realtime -d 0B11,0B30
```

## ディレクトリ構造

```
jvdb/
├── .jvdb/              # メタデータ
│   └── metadata.json   # データベース情報
├── raw/                # 通常データ（生テキスト）
│   └── JV-*.txt
├── parquet/            # 通常データ（Parquet形式）
│   ├── RA/            # レースデータ
│   ├── SE/            # 成績データ
│   └── ...
└── realtime/          # リアルタイムデータ
    ├── raw/           # 生テキスト
    │   └── JVRT-*.txt
    └── parquet/       # Parquet形式
        ├── RA/
        ├── SE/
        └── ...
```

## ワークフロー例

### 初回セットアップ

```bash
# 1. 設定
JVDatabase config

# 2. データベース初期化（対話形式）
JVDatabase init -i

# 3. 確認
ls jvdb/
```

### 定期更新（週次など）

```bash
# 前回更新以降のデータを自動取得
JVDatabase update
```

### レース日の運用

```bash
# 土曜朝
JVDatabase realtime

# 日曜朝
JVDatabase realtime

# レース後の最終確認
JVDatabase update
```

## 注意事項

- JRA-VAN Data Lab.の利用契約が必要です
- 初回実行時は利用規約への同意が必要です
- Windows環境でのみ動作します（JVLinkのCOM依存）
- 大量データのダウンロードには時間がかかります
- `realtime`データは速報値のため、後日`update`で確定データを取得してください

## トラブルシューティング

### エラー: JVDownloader.exeが見つからない

JVDownloader.exeが同じディレクトリまたはPATHに存在することを確認してください。

### エラー: 認証エラー（-301）

JRA-VAN Data Lab.の利用キーが正しく設定されているか確認してください。

### データが取得できない

- ネットワーク接続を確認
- JRA-VANのメンテナンス状況を確認
- 日付範囲が正しいか確認（未来の日付は取得不可）