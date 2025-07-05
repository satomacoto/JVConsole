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
            // AVレコード（出走取消・競走除外）
            RegisterMapping(new AVRecordTypeMapping());
            
            // BNレコード（馬主マスタ）
            RegisterMapping(new BNRecordTypeMapping());
            
            // BRレコード（生産者マスタ）
            RegisterMapping(new BRRecordTypeMapping());
            
            // BTレコード（系統情報）
            RegisterMapping(new BTRecordTypeMapping());
            
            // 今後、各レコード種別のマッピングクラスを追加していく
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
            
            // マッピングが見つからない場合はデフォルトで文字列型を返す
            return typeof(string);
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