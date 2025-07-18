namespace JVDuckDB.TypeMapping
{
    /// <summary>
    /// レコード種別マッピングの基底クラス
    /// </summary>
    public abstract class RecordTypeMappingBase : IRecordTypeMapping
    {
        public abstract string RecordSpec { get; }
        public abstract Dictionary<string, Type> FieldTypeMappings { get; }
        public virtual List<string> IndexColumns => new List<string>();
        
        /// <summary>
        /// フィールド名から型を推論する
        /// </summary>
        public Type InferTypeFromFieldName(string fieldName)
        {
            // 着回数フィールド
            if (fieldName.Contains("ChakuKaisu"))
                return typeof(int);
                
            // 金額フィールド
            if (fieldName.Contains("Pay") || fieldName.Contains("Honsyokin") || 
                fieldName.Contains("syokin") || fieldName.Contains("Odds"))
                return typeof(int);
                
            // 年月日フィールド
            if (fieldName == "Year" || fieldName == "Month" || fieldName == "Day")
                return typeof(int);
                
            // 時間フィールド（ミリ秒）
            if (fieldName.Contains("Time") && !fieldName.Contains("Hasso"))
                return typeof(int);
                
            // 距離フィールド
            if (fieldName.Contains("Kyori"))
                return typeof(int);
                
            // 頭数フィールド
            if (fieldName.Contains("Tosu"))
                return typeof(int);
                
            // 順位・着順フィールド
            if (fieldName.Contains("Jyuni") || fieldName.Contains("KakuteiJyuni"))
                return typeof(int);
                
            // 体重関連フィールド
            if (fieldName.Contains("Taijyu") || fieldName.Contains("Zogen"))
                return typeof(int);
                
            // 人気フィールド
            if (fieldName.Contains("Ninki"))
                return typeof(int);
                
            // フラグ・区分・記号フィールドは文字列
            if (fieldName.Contains("Flag") || fieldName.Contains("Kubun") || 
                fieldName.Contains("Mark"))
                return typeof(string);
                
            // コード系フィールドは文字列
            if (fieldName.EndsWith("CD") || fieldName.EndsWith("Code"))
                return typeof(string);
                
            // デフォルトは文字列
            return typeof(string);
        }
    }
}