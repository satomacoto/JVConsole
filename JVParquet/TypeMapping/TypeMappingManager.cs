using System;
using System.Collections.Generic;
using System.Linq;
using JVParquet.TypeMapping.RecordTypes;

namespace JVParquet.TypeMapping
{
    /// <summary>
    /// レコード種別ごとの型マッピングを管理するクラス
    /// </summary>
    public class TypeMappingManager
    {
        private readonly Dictionary<string, IRecordTypeMapping> _mappings;
        private static readonly Lazy<TypeMappingManager> _instance = new Lazy<TypeMappingManager>(() => new TypeMappingManager());

        public static TypeMappingManager Instance => _instance.Value;

        public TypeMappingManager()
        {
            _mappings = new Dictionary<string, IRecordTypeMapping>();
            InitializeMappings();
        }

        /// <summary>
        /// 各レコード種別の型マッピングを初期化
        /// </summary>
        private void InitializeMappings()
        {
            // A
            RegisterMapping(new AVRecordTypeMapping()); // 出走取消・競走除外

            // B
            RegisterMapping(new BNRecordTypeMapping()); // 馬主マスタ
            RegisterMapping(new BRRecordTypeMapping()); // 生産者マスタ
            RegisterMapping(new BTRecordTypeMapping()); // 系統情報

            // C
            RegisterMapping(new CCRecordTypeMapping()); // コース変更
            RegisterMapping(new CHRecordTypeMapping()); // 調教師マスタ
            RegisterMapping(new CKRecordTypeMapping()); // 出走別着度数
            RegisterMapping(new CSRecordTypeMapping()); // コース情報

            // D
            RegisterMapping(new DMRecordTypeMapping()); // タイム型データマイニング予想

            // H
            RegisterMapping(new H1RecordTypeMapping()); // 票数（全掛け）
            RegisterMapping(new H6RecordTypeMapping()); // 票数（3連単）
            RegisterMapping(new HCRecordTypeMapping()); // 坂路調教
            RegisterMapping(new HNRecordTypeMapping()); // 繁殖馬マスタ
            RegisterMapping(new HRRecordTypeMapping()); // 払戻
            RegisterMapping(new HSRecordTypeMapping()); // 馬主データ
            RegisterMapping(new HYRecordTypeMapping()); // 馬名意味由来

            // J
            RegisterMapping(new JCRecordTypeMapping()); // 競走馬市場取引価格
            RegisterMapping(new JGRecordTypeMapping()); // 除外馬

            // K
            RegisterMapping(new KSRecordTypeMapping()); // 騎手マスタ

            // O
            RegisterMapping(new O1RecordTypeMapping()); // オッズ（単複枠）
            RegisterMapping(new O2RecordTypeMapping()); // オッズ（馬連）
            RegisterMapping(new O3RecordTypeMapping()); // オッズ（ワイド）
            RegisterMapping(new O4RecordTypeMapping()); // オッズ（馬単）
            RegisterMapping(new O5RecordTypeMapping()); // オッズ（3連複）
            RegisterMapping(new O6RecordTypeMapping()); // オッズ（3連単）

            // R
            RegisterMapping(new RARecordTypeMapping()); // レース詳細
            RegisterMapping(new RCRecordTypeMapping()); // レースコメント

            // S
            RegisterMapping(new SERecordTypeMapping()); // レース成績
            RegisterMapping(new SKRecordTypeMapping()); // 産駒マスタ

            // T
            RegisterMapping(new TCRecordTypeMapping()); // 特別登録馬
            RegisterMapping(new TKRecordTypeMapping()); // 特別登録（抽選番号）
            RegisterMapping(new TMRecordTypeMapping()); // 対戦型データマイニング予想

            // U
            RegisterMapping(new UMRecordTypeMapping()); // 馬マスタ

            // W
            RegisterMapping(new WCRecordTypeMapping()); // ウッドチップ調教
            RegisterMapping(new WERecordTypeMapping()); // 天候・馬場状態
            RegisterMapping(new WFRecordTypeMapping()); // WIN5
            RegisterMapping(new WHRecordTypeMapping()); // WIN5払戻

            // Y
            RegisterMapping(new YSRecordTypeMapping()); // 予想スケジュール
        }

        /// <summary>
        /// 型マッピングを登録
        /// </summary>
        public void RegisterMapping(IRecordTypeMapping mapping)
        {
            _mappings[mapping.RecordSpec] = mapping;
        }

        /// <summary>
        /// レコード種別に対応する型マッピングを取得
        /// </summary>
        public IRecordTypeMapping GetMapping(string recordSpec)
        {
            if (_mappings.TryGetValue(recordSpec, out var mapping))
            {
                return mapping;
            }
            return null;
        }

        /// <summary>
        /// フィールド名から.NET型を取得
        /// </summary>
        public Type GetFieldType(string recordSpec, string fieldName)
        {
            var mapping = GetMapping(recordSpec);
            if (mapping != null && mapping.FieldTypeMappings.TryGetValue(fieldName, out var type))
            {
                return type;
            }
            
            // マッピングが見つからない場合、基底クラスの型推論を使用
            return RecordTypeMappingBase.InferTypeFromFieldName(fieldName);
        }

        /// <summary>
        /// レコード種別のインデックスカラムを取得
        /// </summary>
        public List<string> GetIndexColumns(string recordSpec)
        {
            var mapping = GetMapping(recordSpec);
            return mapping?.IndexColumns ?? new List<string>();
        }
    }
}