namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// レコード種別の型マッピングを管理するクラス
    /// </summary>
    public class TypeMappingManager
    {
        private readonly Dictionary<string, IRecordTypeMapping> _mappings;
        
        public TypeMappingManager()
        {
            _mappings = new Dictionary<string, IRecordTypeMapping>();
            
            // 各レコード種別のマッピングを登録
            RegisterMapping(new RaRecordTypeMapping());
            RegisterMapping(new SeRecordTypeMapping());
            RegisterMapping(new HnRecordTypeMapping());
            RegisterMapping(new UmRecordTypeMapping());
            RegisterMapping(new JgRecordTypeMapping());
            RegisterMapping(new HrRecordTypeMapping());
            RegisterMapping(new O1RecordTypeMapping());
            RegisterMapping(new O2RecordTypeMapping());
            RegisterMapping(new H1RecordTypeMapping());
            RegisterMapping(new WfRecordTypeMapping());
            RegisterMapping(new AvRecordTypeMapping());
            RegisterMapping(new BnRecordTypeMapping());
            RegisterMapping(new BrRecordTypeMapping());
            RegisterMapping(new BtRecordTypeMapping());
            RegisterMapping(new CcRecordTypeMapping());
            RegisterMapping(new ChRecordTypeMapping());
            RegisterMapping(new CkRecordTypeMapping());
            RegisterMapping(new CsRecordTypeMapping());
            RegisterMapping(new DmRecordTypeMapping());
            RegisterMapping(new H6RecordTypeMapping());
            RegisterMapping(new HcRecordTypeMapping());
            RegisterMapping(new HsRecordTypeMapping());
            RegisterMapping(new HyRecordTypeMapping());
            RegisterMapping(new JcRecordTypeMapping());
            RegisterMapping(new KsRecordTypeMapping());
            RegisterMapping(new O3RecordTypeMapping());
            RegisterMapping(new O4RecordTypeMapping());
            RegisterMapping(new O5RecordTypeMapping());
            RegisterMapping(new O6RecordTypeMapping());
            RegisterMapping(new RcRecordTypeMapping());
            RegisterMapping(new SkRecordTypeMapping());
            RegisterMapping(new TcRecordTypeMapping());
            RegisterMapping(new TkRecordTypeMapping());
            RegisterMapping(new TmRecordTypeMapping());
            RegisterMapping(new WcRecordTypeMapping());
            RegisterMapping(new WeRecordTypeMapping());
            RegisterMapping(new WhRecordTypeMapping());
            RegisterMapping(new YsRecordTypeMapping());
        }
        
        private void RegisterMapping(IRecordTypeMapping mapping)
        {
            _mappings[mapping.RecordSpec] = mapping;
        }
        
        /// <summary>
        /// レコード種別とフィールド名から型を取得
        /// </summary>
        public Type GetFieldType(string recordSpec, string fieldName)
        {
            if (_mappings.TryGetValue(recordSpec, out var mapping))
            {
                if (mapping.FieldTypeMappings.TryGetValue(fieldName, out var type))
                {
                    return type;
                }
                
                // 基底クラスの型推論を使用
                if (mapping is RecordTypeMappingBase baseMapping)
                {
                    return baseMapping.InferTypeFromFieldName(fieldName);
                }
            }
            
            // デフォルトは文字列
            return typeof(string);
        }
        
        /// <summary>
        /// レコード種別のインデックスカラムを取得
        /// </summary>
        public List<string> GetIndexColumns(string recordSpec)
        {
            if (_mappings.TryGetValue(recordSpec, out var mapping))
            {
                return mapping.IndexColumns;
            }
            
            return new List<string>();
        }
        
        /// <summary>
        /// レコード種別の型マッピングが存在するか
        /// </summary>
        public bool HasMapping(string recordSpec)
        {
            return _mappings.ContainsKey(recordSpec);
        }
    }
}