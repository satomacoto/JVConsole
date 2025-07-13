# JVDuckDB

JRA-VANデータをDuckDBを使用してParquet形式に変換するツールです。パーティション分割、重複排除、インデックステーブル作成機能を提供します。

## 特徴

- **完全なデータパース**: JVData_Structを使用した全フィールドのパース
- **パーティション分割**: MakeDateによるHive形式のパーティション（year=YYYY/month=MM/day=DD）
- **重複排除**: 各レコードタイプのPKに基づく重複排除（最新のMakeDateを保持）
- **高速処理**: DuckDBによる効率的なデータ処理
- **Python互換**: pandas.read_parquet()で直接読み込み可能
- **S3/Athena対応**: AWS S3やAthenaでのクエリに対応
- **Feast対応**: 特徴量ストアとして使用可能

## インストール

```bash
dotnet build
```

## 使用方法

### 基本的な使用方法

```bash
# JVデータファイルをParquet形式に変換
dotnet run -- -i /path/to/jv-data -o /path/to/output

# 単一ファイルの変換
dotnet run -- -i /path/to/JV-RACE-20241201.txt -o /path/to/output
```

### オプション

- `-i, --input`: 入力ディレクトリまたはファイルパス（必須）
- `-o, --output`: 出力ディレクトリパス（必須）
- `-s, --skip`: スキップするレコード種別（例: H6,O6）
- `-b, --batch-size`: バッチサイズ（デフォルト: 10000）
- `-m, --memory`: DuckDBデータベースパス（デフォルト: :memory:）
- `-f, --filter`: 処理するレコード種別（指定しない場合は全て）
- `-d, --dedupe`: 重複排除を行うか（デフォルト: true）
- `-v, --verbose`: 詳細ログを出力

### 使用例

```bash
# RAレコードのみを処理
dotnet run -- -i ./jv-data -o ./output -f RA

# H6とO6レコードをスキップ
dotnet run -- -i ./jv-data -o ./output -s H6,O6

# 重複排除を無効化
dotnet run -- -i ./jv-data -o ./output -d false

# 詳細ログを有効化
dotnet run -- -i ./jv-data -o ./output -v
```

## 出力形式

### ディレクトリ構造

```
output/
├── RA/
│   ├── year=2024/
│   │   ├── month=01/
│   │   │   ├── day=01/
│   │   │   │   └── *.parquet
│   │   │   └── day=02/
│   │   │       └── *.parquet
│   │   └── month=02/
│   │       └── ...
├── SE/
│   └── ...
├── UM/
│   └── ...
└── _index/
    └── jv_partition_index.parquet
```

### パーティション方式

すべてのレコードはMakeDate（データ作成日）でパーティション分割されます：
- year=YYYY
- month=MM
- day=DD

これにより、特定期間のデータを効率的にクエリできます。

### インデックステーブル

`_index/jv_partition_index.parquet`には以下の情報が含まれます：

| カラム名 | 説明 |
|---------|------|
| record_spec | レコード種別（RA, SE等） |
| id_Year | レース年（レース系レコードのみ） |
| id_MonthDay | レース月日（レース系レコードのみ） |
| id_JyoCD | 場コード（レース系レコードのみ） |
| KettoNum | 血統登録番号（馬系レコードのみ） |
| head_MakeDate_Year | パーティション年 |
| head_MakeDate_Month | パーティション月 |
| head_MakeDate_Day | パーティション日 |
| partition_path | パーティションパス |
| record_count | レコード数 |
| last_updated | 最終更新日時 |

## Pythonでの使用例

### pandasでの読み込み

```python
import pandas as pd

# 特定のパーティションを読み込み
df = pd.read_parquet('output/RA/year=2024/month=01/day=01/')

# 全データを読み込み（パーティションを意識せずに）
df = pd.read_parquet('output/RA/')

# インデックステーブルを使用した効率的なクエリ
index_df = pd.read_parquet('output/_index/jv_partition_index.parquet')
target_partitions = index_df[
    (index_df['record_spec'] == 'RA') & 
    (index_df['id_Year'] == '2024')
]['partition_path'].unique()

# 対象パーティションのみ読み込み
dfs = []
for partition in target_partitions:
    dfs.append(pd.read_parquet(f'output/RA/{partition}'))
df = pd.concat(dfs)
```

### DuckDBでの直接クエリ

```python
import duckdb

# DuckDBで直接クエリ
con = duckdb.connect()
df = con.query("""
    SELECT * FROM read_parquet('output/RA/**/*.parquet')
    WHERE id_Year = '2024' AND id_MonthDay >= '0101'
""").df()
```

## AWS S3での使用

```python
# S3にアップロード後
df = pd.read_parquet('s3://your-bucket/jv-data/RA/')

# Athenaでのクエリ例
CREATE EXTERNAL TABLE jv_race (
    id_Year string,
    id_MonthDay string,
    id_JyoCD string,
    -- 他のカラム
)
PARTITIONED BY (
    year string,
    month string,
    day string
)
STORED AS PARQUET
LOCATION 's3://your-bucket/jv-data/RA/'
```

## Feastでの使用

```python
from feast import FeatureStore, Entity, FeatureView, FileSource

# エンティティ定義
race_entity = Entity(name="race", join_keys=["race_id"])

# データソース定義
race_source = FileSource(
    path="output/RA/",
    timestamp_field="head_MakeDate",
    created_timestamp_column="head_MakeDate"
)

# フィーチャービュー定義
race_features = FeatureView(
    name="race_features",
    entities=[race_entity],
    source=race_source,
    features=[
        Feature(name="GradeCD", dtype=ValueType.STRING),
        Feature(name="Kyori", dtype=ValueType.STRING),
        # 他のフィーチャー
    ]
)
```

## 技術仕様

### 重複排除の仕組み

各レコードタイプのPK（Primary Key）に基づいて重複を検出し、最新のMakeDateを持つレコードのみを保持します。

例：
- RAレコード: id_Year + id_MonthDay + id_JyoCD + id_Kaiji + id_Nichiji + id_RaceNum
- UMレコード: KettoNum
- KSレコード: KisyuCode

### パフォーマンス

- バッチ処理により大量データを効率的に処理
- DuckDBのカラムナストレージによる高速な集計処理
- メモリ使用量を抑えたストリーミング処理

## 制限事項

- 現在実装済みのレコードタイプ: RA, SE, UM, KS, CH
- その他のレコードタイプは基本情報（record_spec, data_kubun, MakeDate）のみパース
- 完全な実装にはすべてのレコードタイプのパース処理追加が必要

## ライセンス

このプロジェクトはJVConsoleの一部として提供されています。