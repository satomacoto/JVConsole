using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping
{
    /// <summary>
    /// レコード種別の型マッピングの基底クラス
    /// </summary>
    public abstract class RecordTypeMappingBase : IRecordTypeMapping
    {
        public abstract string RecordSpec { get; }
        public abstract Dictionary<string, Type> FieldTypeMappings { get; }
        public abstract List<string> IndexColumns { get; }

        /// <summary>
        /// 共通の型推論ヘルパーメソッド
        /// </summary>
        protected static Type InferTypeFromFieldName(string fieldName)
        {
            // 数値系のサフィックス
            if (fieldName.EndsWith("Tosu") || fieldName.EndsWith("Num") || 
                fieldName.EndsWith("Count") || fieldName.EndsWith("Kaiji") || 
                fieldName.EndsWith("Nichiji"))
                return typeof(int);

            // 金額系
            if (fieldName.Contains("Pay") || fieldName.Contains("Honsyokin") || 
                fieldName.Contains("Fukasyokin") || fieldName.Contains("Syokin"))
                return typeof(decimal);

            // 距離・時間系
            if (fieldName.EndsWith("Kyori") || fieldName.EndsWith("Time") || 
                fieldName.EndsWith("Jikan"))
                return typeof(int);

            // 年月日
            if (fieldName.EndsWith(".Year") || fieldName.EndsWith(".Month") || 
                fieldName.EndsWith(".Day"))
                return typeof(int);

            // フラグ系
            if (fieldName.Contains("Flag") || fieldName.Contains("Kubun"))
                return typeof(string); // 将来的にはbool or enumにする可能性あり

            // コード系
            if (fieldName.EndsWith("CD") || fieldName.EndsWith("Code"))
                return typeof(string);

            // デフォルトは文字列
            return typeof(string);
        }
    }
}