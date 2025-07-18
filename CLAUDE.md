# CLAUDE.md

このファイルは、Claude Code (claude.ai/code) がこのリポジトリで作業する際のガイドラインを提供します。

## プロジェクト概要

JVConsoleは、JRA-VAN競馬データを処理するための複数のコンポーネントで構成されるソリューションです：

| プロジェクト | フレームワーク | 目的 |
|---------|-----------|---------|
| JVDownloader | .NET Framework 4.8 | COM相互運用を使用してJRA-VANデータラボからデータをダウンロード |
| JVParser | .NET 6.0 | JVデータファイルを解析してJSONL形式に変換 |
| JVDatabase | .NET 8.0 | JVデータのデータベース操作 |
| JVDuckDB | .NET 8.0 | データ分析のためのDuckDB統合 |
| JVParquet | .NET 8.0 | Parquetファイル形式のサポート |

## ビルドコマンド

### .NET Frameworkプロジェクト
```bash
# JVDownloader
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Debug
msbuild JVDownloader/JVDownloader.csproj /p:Configuration=Release
```

### .NET Core/5+プロジェクト
```bash
# 全プロジェクトのビルド
dotnet build

# 個別プロジェクトのビルド
dotnet build JVParser/JVParser.csproj
dotnet build JVDatabase/JVDatabase.csproj
dotnet build JVDuckDB/JVDuckDB.csproj
dotnet build JVParquet/JVParquet.csproj

# 引数付きで実行
dotnet run --project JVParser/JVParser.csproj [input_file] [output_dir]
dotnet run --project JVDatabase/JVDatabase.csproj [args]
dotnet run --project JVDuckDB/JVDuckDB.csproj [args]
```

## アーキテクチャ詳細

### データフロー
1. **JVDownloader** → バイナリ競馬データをテキストファイルとしてダウンロード (`JV-{spec}-{timestamp}.txt`)
2. **JVParser** → バイナリ構造を解析し、レコードタイプごとのJSONLファイルを出力
3. **JVDatabase/JVDuckDB/JVParquet** → さらなる処理と分析

### 主要コンポーネント

#### JVDownloader
- `Program.cs`: エントリーポイント（コマンド: `setup`, `jv`, `jvrt`）
- `Logger.cs`: JV-Link APIエラーコードマッピング
- WindowsおよびJRA-VANデータラボソフトウェアが必要

#### JVParser
- `JVData_Struct.cs`: 30以上のレコードタイプ定義
- `RecordSpecStreamWriterManager.cs`: 出力ストリーム管理
- 出力: `RA.jsonl`, `SE.jsonl` など

#### JVDatabase/JVDuckDB/JVParquet
- 最新のデータ処理機能
- 様々なデータベースとファイル形式のサポート

## 開発ガイドライン

### 言語設定
- 常に日本語で応答する
- 技術的なコミュニケーションは明確かつ簡潔に

### Python開発
- Pythonのコードは jv-python 以下でuvをつかって動かして

### テストデータ管理

テストデータは以下のルールに従って管理：

1. **命名規則**: `test_`で始まるディレクトリ名を使用
2. **推奨構造**:
   ```
   {Project}/
   └── test_output/     # 各プロジェクト用のテスト出力
   ```
3. **自動除外**: `.gitignore`で以下が除外されます
   - `test_*/`, `**/test_*/`
   - `jvdb*/`, `**/jvdb*/`
   - `nul`, `**/nul`

### 重要な制約事項

- **テストフレームワークなし**: テストは手動で実装する必要があります
- **COM依存**: JVDownloaderはWindows環境が必要
- **マルチフレームワーク**: .NET Frameworkと.NET Core/5+の混在
- **複雑なデータ形式**: `JVData_Struct.cs`で定義されたバイナリ構造

## テストと検証

機能をテストする際：
1. 指定されたテストディレクトリ（`test_*`）を使用
2. 変更後にlint/typecheckコマンドを実行
3. 適切なツールでデータの整合性を検証
4. 完了後にテストデータをクリーンアップ