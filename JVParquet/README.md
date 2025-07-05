# JVParquet

JVParquetは、JRA-VANのレースデータをParquet形式に変換する.NETアプリケーションです。

## 概要

JVDownloaderで取得したJRA-VANのテキストデータを、効率的なParquet形式に変換します。Parquetファイルには、Python（Pandas）での利用を考慮したメタデータが含まれています。

## 使用方法

### 基本的な使い方

```bash
# JVDownloaderの出力ファイルをParquet形式に変換
dotnet run -- convert -i JV-RACE-20241201.txt -o output

# 特定のレコードタイプをスキップ
dotnet run -- convert -i JV-RACE-20241201.txt -o output -s H6,O6

# Parquetファイルの内容を確認
dotnet run -- read -d output -r RA
```

### コマンドラインオプション

#### convertコマンド
- `-i, --inputPath`: 入力テキストファイルのパス（必須）
- `-o, --outputDir`: 出力ディレクトリ（必須）
- `-s, --skipRecordSpec`: スキップするレコード種別（カンマ区切り）

#### readコマンド
- `-d, --directory`: Parquetファイルが格納されているディレクトリ
- `-f, --file`: 特定のParquetファイルのパス
- `-r, --recordSpec`: 分析するレコード種別（デフォルト: CK）
- `-m, --maxRows`: 表示する最大行数（デフォルト: 10）

## 出力形式

### ディレクトリ構造

Parquetファイルは以下の構造で出力されます：

```
output/
├── RA/
│   └── year=2024/
│       └── month=12/
│           └── day=01/
│               └── data.parquet
├── SE/
│   └── year=2024/
│       └── month=12/
│           └── day=01/
│               └── data.parquet
└── ...
```

### メタデータ

各Parquetファイルには、以下のメタデータが含まれています：

- `pandas_index_columns`: MultiIndexとして使用するカラムのリスト（JSON形式）
- `record_spec`: レコード種別（RA、SE等）
- `created_by`: 作成アプリケーション名（JVParquet.NET）

## Python（Pandas）での利用方法

### 基本的な読み込み

```python
import pandas as pd
import pyarrow.parquet as pq
import json

# Parquetファイルを読み込む
df = pd.read_parquet('output/RA/year=2024/month=12/day=01/data.parquet')

# メタデータからインデックスカラムを取得して設定
parquet_file = pq.ParquetFile('output/RA/year=2024/month=12/day=01/data.parquet')
metadata = parquet_file.metadata.metadata

if b'pandas_index_columns' in metadata:
    index_cols = json.loads(metadata[b'pandas_index_columns'])
    df = df.set_index(index_cols)
```

### ヘルパー関数の使用例

```python
def load_jv_parquet(filepath):
    """JVParquetで作成したファイルを読み込み、自動的にMultiIndexを設定"""
    import pandas as pd
    import pyarrow.parquet as pq
    import json
    
    df = pd.read_parquet(filepath)
    
    # メタデータからインデックスカラムを取得
    parquet_file = pq.ParquetFile(filepath)
    metadata = parquet_file.metadata.metadata
    
    if b'pandas_index_columns' in metadata:
        index_cols = json.loads(metadata[b'pandas_index_columns'])
        df = df.set_index(index_cols)
    
    return df

# 使用例
df_race = load_jv_parquet('output/RA/year=2024/month=12/day=01/data.parquet')
```

## レコードタイプとインデックスカラム

### レース関連レコード

| レコード | 説明 | インデックスカラム |
|---------|------|------------------|
| RA | レース詳細 | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum |
| SE | 馬毎レース情報 | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum, Umaban, KettoNum |
| HR | 払戻 | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum |
| O1-O6 | オッズ | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum |
| H1,H6 | 票数 | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum |

### マスター系レコード (RECORDSPECS_MASTER)

| レコード | 説明 | インデックスカラム |
|---------|------|------------------|
| UM | 競走馬マスタ | KettoNum |
| KS | 騎手マスタ | KisyuCode |
| CH | 調教師マスタ | ChokyosiCode |
| BR | 生産者マスタ | BreederCode |
| BN | 馬主マスタ | BanusiCode |
| RC | レコードマスタ | id_Year, id_MonthDay, id_JyoCD, id_Kaiji, id_Nichiji, id_RaceNum, TokuNum, SyubetuCD, Kyori, TrackCD |
| HN | 繁殖馬マスタ | HansyokuNum |
| SK | 産駒マスタ | KettoNum |
| BT | 系統情報 | HansyokuNum |

## ビルド方法

```bash
# .NET 9.0 SDKが必要です
dotnet build

# リリースビルド
dotnet build -c Release
```

## 注意事項

- 出力されるParquetファイルのカラム名は、元のJVData構造体のフィールド名に基づいています
- ドット（`.`）はアンダースコア（`_`）に変換されます（例：`id.Year` → `id_Year`）
- 配列要素は`_数値__`形式で表現されます（例：`CornerInfo[0].Corner` → `CornerInfo_0__Corner`）
- 現在、すべてのデータは文字列型として保存されます（Phase 1実装）