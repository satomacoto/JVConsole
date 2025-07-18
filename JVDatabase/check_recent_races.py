#!/usr/bin/env python3
"""
JVDatabase RAデータから最近のレース開催情報を確認するスクリプト
"""

import pandas as pd
import pyarrow.parquet as pq
from pathlib import Path
from datetime import datetime

def check_recent_races():
    # パスの設定
    base_path = Path("/mnt/c/Users/stmct/ghq/github.com/satomacoto/JVConsole/JVDatabase/jvdb/parquet/RA")
    
    # 最新のファイルを探す（2025年のデータ）
    parquet_files = []
    
    # 2025年のファイルを収集
    year_2025 = base_path / "year=2025"
    if year_2025.exists():
        for month_dir in sorted(year_2025.iterdir(), reverse=True):
            for day_dir in month_dir.iterdir():
                for file in day_dir.glob("*.parquet"):
                    parquet_files.append(file)
    
    print(f"見つかったファイル数: {len(parquet_files)}")
    
    # 最新のRACEファイルから読み込み
    race_files = [f for f in parquet_files if "RACE" in f.name]
    
    if not race_files:
        print("RACEファイルが見つかりませんでした。")
        return
    
    # 最新のRACEファイルを選択
    latest_race_file = sorted(race_files, reverse=True)[0]
    print(f"\n読み込むファイル: {latest_race_file}")
    
    # Parquetファイルを読み込み
    df = pd.read_parquet(latest_race_file)
    
    print(f"\nデータ件数: {len(df)}")
    print(f"カラム: {list(df.columns)}")
    
    # race_dateカラムがあるか確認
    if 'race_date' in df.columns:
        # race_dateでソート（降順）
        df_sorted = df.sort_values('race_date', ascending=False)
        
        # 最近のレース情報を10件表示
        print("\n最近のレース開催情報（10件）:")
        print("-" * 80)
        
        for idx, row in df_sorted.head(10).iterrows():
            print(f"開催日: {row['race_date']}")
            
            # idカラムが辞書形式の場合
            if 'id' in df.columns and isinstance(row['id'], dict):
                print(f"  場コード: {row['id'].get('JyoCD', 'N/A')}")
                print(f"  回次: {row['id'].get('Kaiji', 'N/A')}")
                print(f"  日次: {row['id'].get('Nichiji', 'N/A')}")
                print(f"  レース番号: {row['id'].get('RaceNum', 'N/A')}")
            # idカラムが別々の場合
            elif 'id.JyoCD' in df.columns:
                print(f"  場コード: {row.get('id.JyoCD', 'N/A')}")
                print(f"  回次: {row.get('id.Kaiji', 'N/A')}")
                print(f"  日次: {row.get('id.Nichiji', 'N/A')}")
                print(f"  レース番号: {row.get('id.RaceNum', 'N/A')}")
            
            # レース名があれば表示
            if 'RaceName' in df.columns:
                print(f"  レース名: {row.get('RaceName', 'N/A')}")
            
            print("-" * 40)
    else:
        print("\nrace_dateカラムが見つかりません。カラム構造を確認します:")
        print(df.head(1).to_dict(orient='records'))

if __name__ == "__main__":
    check_recent_races()