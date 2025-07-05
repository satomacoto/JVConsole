using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace JVParquet
{
    public static class ReflectionFlattener
    {
        /// <summary>
        /// 構造体を直接リフレクションでフラット化（カスタム命名規則）
        /// 配列: name_0__property
        /// 通常のネスト: name_property
        /// </summary>
        public static Dictionary<string, object?> FlattenStruct(object obj)
        {
            var result = new Dictionary<string, object?>();
            FlattenObject(obj, "", result, false);
            return result;
        }

        private static void FlattenObject(object? obj, string prefix, Dictionary<string, object?> result, bool isArrayElement)
        {
            if (obj == null)
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    result[prefix] = null;
                }
                return;
            }

            var type = obj.GetType();

            // プリミティブ型または文字列の場合
            if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    result[prefix] = obj;
                }
                return;
            }

            // 配列の場合
            if (type.IsArray)
            {
                var array = (Array)obj;
                for (int i = 0; i < array.Length; i++)
                {
                    var itemPrefix = string.IsNullOrEmpty(prefix) ? $"{i}" : $"{prefix}_{i}";
                    FlattenObject(array.GetValue(i), itemPrefix, result, true);
                }
                return;
            }

            // 構造体またはクラスの場合
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var fieldName = field.Name;
                var fieldValue = field.GetValue(obj);
                
                // プレフィックスの組み立て
                string fieldPrefix;
                if (string.IsNullOrEmpty(prefix))
                {
                    fieldPrefix = fieldName;
                }
                else if (isArrayElement)
                {
                    // 配列要素内のプロパティは__で区切る
                    fieldPrefix = $"{prefix}__{fieldName}";
                }
                else
                {
                    // 通常のネストは_で区切る
                    fieldPrefix = $"{prefix}_{fieldName}";
                }

                // フィールドの型をチェック
                if (field.FieldType.IsValueType && !field.FieldType.IsPrimitive && field.FieldType != typeof(decimal))
                {
                    // ネストした構造体
                    FlattenObject(fieldValue, fieldPrefix, result, false);
                }
                else if (field.FieldType.IsArray)
                {
                    // 配列
                    FlattenObject(fieldValue, fieldPrefix, result, false);
                }
                else
                {
                    // プリミティブ型または文字列
                    result[fieldPrefix] = fieldValue;
                }
            }
        }
    }
}