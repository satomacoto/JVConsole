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
        public static Type InferTypeFromFieldName(string fieldName)
        {
            // 配列要素の場合の処理を改善
            // 例: HonRuikei_0__ChakuKaisuDirt_ChakuKaisu_0 -> ChakuKaisu部分を抽出
            var cleanFieldName = fieldName;
            
            // ダブルアンダースコアで区切られた最後の部分を取得
            var parts = fieldName.Split(new[] { "__" }, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                cleanFieldName = parts[parts.Length - 1];
            }
            
            // アンダースコアで区切られた最後の部分（インデックスを除く）
            var lastParts = cleanFieldName.Split('_');
            var lastPart = lastParts[lastParts.Length - 1];
            
            // 最後の部分が数字の場合、その前の部分を使用
            if (System.Text.RegularExpressions.Regex.IsMatch(lastPart, @"^\d+$") && lastParts.Length > 1)
            {
                lastPart = lastParts[lastParts.Length - 2];
            }
            
            // ChakuKaisuは常にint型（数値を文字列で保持）
            // JVデータでは文字列として保存されるが、意味的には数値
            if (lastPart == "ChakuKaisu" || cleanFieldName.Contains("ChakuKaisu"))
                return typeof(int);
            
            // 頭数・回数系
            if (lastPart == "Tosu" || lastPart == "Num" || 
                lastPart == "Count" || lastPart == "Kaiji" || 
                lastPart == "Nichiji" || lastPart == "RaceNum" ||
                cleanFieldName.EndsWith("Tosu"))
                return typeof(int);
            
            // 年月日時分秒
            if (lastPart == "Year" || lastPart == "Month" || 
                lastPart == "Day" || lastPart == "Hour" ||
                lastPart == "Minute" || lastPart == "Second")
                return typeof(int);
            
            // 着順
            if (lastPart == "KakuteiJyuni" || lastPart == "Jyuni" || 
                lastPart == "DochakuDosa" || lastPart == "DochakuTime")
                return typeof(int);

            // 金額系
            if (cleanFieldName.Contains("Pay") || cleanFieldName.Contains("Honsyokin") || 
                cleanFieldName.Contains("Fukasyokin") || cleanFieldName.Contains("Syokin") ||
                cleanFieldName.Contains("Kakukin") || cleanFieldName.Contains("Kingaku") ||
                cleanFieldName.Contains("Honsyo") || cleanFieldName.Contains("Fukasyo") ||
                cleanFieldName.Contains("Odds"))
                return typeof(int);

            // 距離・時間系
            if (lastPart == "Kyori" || lastPart == "Time" || 
                lastPart == "Jikan" || lastPart == "MileTime" ||
                lastPart == "FurlongTime" || lastPart == "HaronTime")
                return typeof(int);
            
            // 重量系
            if (lastPart == "Futan" || lastPart == "Taiju" || 
                lastPart == "Zogen" || lastPart == "BaTaiju")
                return typeof(int);
            
            // 順位・着差系
            if (lastPart == "Rank" || lastPart == "Sa")
                return typeof(int);
            
            // フラグ系
            if (cleanFieldName.Contains("Flag") || cleanFieldName.Contains("Kubun") ||
                cleanFieldName.Contains("Mark"))
                return typeof(string);

            // コード系
            if (lastPart == "CD" || lastPart == "Code" || 
                lastPart.EndsWith("CD") || lastPart.EndsWith("Code"))
                return typeof(string);
            
            // 設定年
            if (lastPart == "SetYear")
                return typeof(int);

            // デフォルトは文字列
            return typeof(string);
        }
    }
}