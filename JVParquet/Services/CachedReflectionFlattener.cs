using System.Collections.Concurrent;
using System.Reflection;

namespace JVParquet.Services
{
    public static class CachedReflectionFlattener
    {
        // 型ごとのフィールド情報をキャッシュ
        private static readonly ConcurrentDictionary<Type, FieldInfo[]> _fieldCache = new();
        
        // 型ごとのフラット化戦略をキャッシュ
        private static readonly ConcurrentDictionary<Type, FlattenStrategy> _strategyCache = new();

        public static Dictionary<string, object?> FlattenStruct(object obj)
        {
            var type = obj.GetType();
            var strategy = _strategyCache.GetOrAdd(type, t => BuildStrategy(t));
            
            var result = new Dictionary<string, object?>(strategy.EstimatedFieldCount);
            strategy.Execute(obj, "", result, false);
            return result;
        }

        private static FlattenStrategy BuildStrategy(Type type)
        {
            var fields = GetCachedFields(type);
            var estimatedCount = EstimateFieldCount(type, fields);
            
            return new FlattenStrategy
            {
                Fields = fields,
                EstimatedFieldCount = estimatedCount,
                Execute = (obj, prefix, result, isArrayElement) =>
                    ExecuteStrategy(obj, prefix, result, fields, isArrayElement)
            };
        }

        private static FieldInfo[] GetCachedFields(Type type)
        {
            return _fieldCache.GetOrAdd(type, 
                t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
        }

        private static int EstimateFieldCount(Type type, FieldInfo[] fields)
        {
            int count = 0;
            foreach (var field in fields)
            {
                if (field.FieldType.IsPrimitive || 
                    field.FieldType == typeof(string) || 
                    field.FieldType == typeof(decimal))
                {
                    count++;
                }
                else if (field.FieldType.IsArray)
                {
                    // 配列の場合は推定値を使用
                    count += 10;
                }
                else if (field.FieldType.IsValueType)
                {
                    // ネストした構造体の場合は再帰的に推定
                    var nestedFields = GetCachedFields(field.FieldType);
                    count += nestedFields.Length;
                }
            }
            return count;
        }

        private static void ExecuteStrategy(
            object? obj,
            string prefix,
            Dictionary<string, object?> result,
            FieldInfo[] fields,
            bool isArrayElement)
        {
            if (obj == null) return;

            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(obj);
                if (fieldValue == null) continue;

                var fieldType = field.FieldType;
                var fieldName = field.Name;
                
                // プレフィックスの構築（高速化）
                string fieldPrefix;
                if (string.IsNullOrEmpty(prefix))
                {
                    fieldPrefix = fieldName;
                }
                else if (isArrayElement)
                {
                    fieldPrefix = string.Concat(prefix, "__", fieldName);
                }
                else
                {
                    fieldPrefix = string.Concat(prefix, "_", fieldName);
                }

                // 型に応じた処理（分岐を最小化）
                if (fieldType.IsPrimitive || fieldType == typeof(string) || fieldType == typeof(decimal))
                {
                    result[fieldPrefix] = fieldValue;
                }
                else if (fieldType.IsArray)
                {
                    ProcessArray((Array)fieldValue, fieldPrefix, result);
                }
                else if (fieldType.IsValueType)
                {
                    ProcessNestedStruct(fieldValue, fieldPrefix, result, false);
                }
            }
        }

        private static void ProcessArray(Array array, string prefix, Dictionary<string, object?> result)
        {
            var elementType = array.GetType().GetElementType()!;
            var isPrimitive = elementType.IsPrimitive || 
                             elementType == typeof(string) || 
                             elementType == typeof(decimal);

            for (int i = 0; i < array.Length; i++)
            {
                var item = array.GetValue(i);
                if (item == null) continue;

                var itemPrefix = $"{prefix}_{i}";
                
                if (isPrimitive)
                {
                    result[itemPrefix] = item;
                }
                else
                {
                    ProcessNestedStruct(item, itemPrefix, result, true);
                }
            }
        }

        private static void ProcessNestedStruct(
            object obj,
            string prefix,
            Dictionary<string, object?> result,
            bool isArrayElement)
        {
            var type = obj.GetType();
            var strategy = _strategyCache.GetOrAdd(type, t => BuildStrategy(t));
            strategy.Execute(obj, prefix, result, isArrayElement);
        }

        private class FlattenStrategy
        {
            public required FieldInfo[] Fields { get; init; }
            public required int EstimatedFieldCount { get; init; }
            public required Action<object?, string, Dictionary<string, object?>, bool> Execute { get; init; }
        }
    }
}