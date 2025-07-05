using System;
using System.Collections.Generic;

namespace JVParquet.TypeMapping
{
    /// <summary>
    /// レコード種別ごとの型マッピングを定義するインターフェース
    /// </summary>
    public interface IRecordTypeMapping
    {
        /// <summary>
        /// レコード種別コード（例：RA, SE, UMなど）
        /// </summary>
        string RecordSpec { get; }

        /// <summary>
        /// フィールド名と.NET型のマッピング
        /// </summary>
        Dictionary<string, Type> FieldTypeMappings { get; }

        /// <summary>
        /// インデックスとして使用するカラム名のリスト
        /// </summary>
        List<string> IndexColumns { get; }
    }
}