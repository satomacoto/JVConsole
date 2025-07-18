using System.Text;
using System.Text.Json;
using JVDuckDB.TypeMapping;
using static JVData_Struct;

namespace JVDuckDB
{
    /// <summary>
    /// UTF-8形式のJVファイルを処理するためのプロセッサー
    /// </summary>
    public class JVDuckDBProcessorBase_UTF8
    {
        // レコード種別と処理メソッドのマッピング
        private readonly Dictionary<string, Action<string, Dictionary<string, object?>>> _recordParsers;

        public JVDuckDBProcessorBase_UTF8()
        {
            _recordParsers = new Dictionary<string, Action<string, Dictionary<string, object?>>>
            {
                { "AV", ParseAVRecord },
                { "BN", ParseBNRecord },
                { "BR", ParseBRRecord },
                { "BT", ParseBTRecord },
                { "CC", ParseCCRecord },
                { "CH", ParseCHRecord },
                { "CK", ParseCKRecord },
                { "CS", ParseCSRecord },
                { "DM", ParseDMRecord },
                { "H1", ParseH1Record },
                { "H6", ParseH6Record },
                { "HC", ParseHCRecord },
                { "HN", ParseHNRecord },
                { "HR", ParseHRRecord },
                { "HS", ParseHSRecord },
                { "HY", ParseHYRecord },
                { "JC", ParseJCRecord },
                { "JG", ParseJGRecord },
                { "KS", ParseKSRecord },
                { "O1", ParseO1Record },
                { "O2", ParseO2Record },
                { "O3", ParseO3Record },
                { "O4", ParseO4Record },
                { "O5", ParseO5Record },
                { "O6", ParseO6Record },
                { "RA", ParseRARecord },
                { "RC", ParseRCRecord },
                { "SE", ParseSERecord },
                { "SK", ParseSKRecord },
                { "TC", ParseTCRecord },
                { "TK", ParseTKRecord },
                { "TM", ParseTMRecord },
                { "UM", ParseUMRecord },
                { "WC", ParseWCRecord },
                { "WE", ParseWERecord },
                { "WF", ParseWFRecord },
                { "WH", ParseWHRecord },
                { "YS", ParseYSRecord }
            };
        }

        public Dictionary<string, object?>? ParseRecord(string line)
        {
            if (string.IsNullOrEmpty(line) || line.Length < 2)
                return null;

            var recordSpec = line.Substring(0, 2);

            // 共通ヘッダーの解析（文字位置ベース）
            var record = new Dictionary<string, object?>
            {
                ["record_spec"] = recordSpec,
                ["data_kubun"] = line.Substring(2, 1),
                ["head_MakeDate_Year"] = line.Substring(3, 4),
                ["head_MakeDate_Month"] = line.Substring(7, 2),
                ["head_MakeDate_Day"] = line.Substring(9, 2),
                ["raw_data"] = line // 元データも保存（デバッグ用）
            };

            // レコード種別ごとの解析
            if (_recordParsers.TryGetValue(recordSpec, out var parser))
            {
                try
                {
                    parser(line, record);
                }
                catch (Exception ex)
                {
                    // パースエラーの場合、基本情報のみ返す
                    record["parse_error"] = ex.Message;
                    record["parse_error_detail"] = ex.ToString();
                }
                return record;
            }

            return null;
        }

        // UTF-8文字列を処理するメソッド
        // JVParquetのアプローチと同様に、UTF-8文字列をShift-JISバイト配列に変換してから処理
        private Dictionary<string, object?> ProcessStructData<T>(string line) where T : struct
        {
            var result = new Dictionary<string, object?>();
            var type = typeof(T);
            
            // まずstring版のSetDataBを試す、次にbyte[]版を試す
            var setDataBStringMethod = type.GetMethod("SetDataB", new[] { typeof(string).MakeByRefType() });
            var setDataBByteMethod = type.GetMethod("SetDataB", new[] { typeof(byte[]).MakeByRefType() });
            
            if (setDataBStringMethod != null)
            {
                // 構造体のインスタンスを作成
                object boxedInstance = new T();
                
                // SetDataBメソッドはref stringを受け取る
                string mutableLine = line;
                var parameters = new object[] { mutableLine };
                setDataBStringMethod.Invoke(boxedInstance, parameters);
                
                // フィールドとプロパティを辞書に変換
                ConvertStructToDict(boxedInstance, type, result);
            }
            else if (setDataBByteMethod != null)
            {
                // 構造体のインスタンスを作成
                object boxedInstance = new T();
                
                // UTF-8文字列をShift-JISバイト配列に変換
                byte[] byteData = Str2Byte(line);
                var parameters = new object[] { byteData };
                setDataBByteMethod.Invoke(boxedInstance, parameters);
                
                // フィールドとプロパティを辞書に変換
                ConvertStructToDict(boxedInstance, type, result);
            }
            
            return result;
        }
        
        // UTF-8文字列をShift-JISバイト配列に変換（JVParquetと同じ）
        private static byte[] Str2Byte(string str)
        {
            return Encoding.GetEncoding("Shift_JIS").GetBytes(str);
        }
        
        // 構造体の内容を辞書に変換（JVParquetのReflectionFlattenerと同様）
        private void ConvertStructToDict(object structInstance, Type type, Dictionary<string, object?> result)
        {
            FlattenObject(structInstance, "", result, false);
        }

        private void FlattenObject(object? obj, string prefix, Dictionary<string, object?> result, bool isArrayElement)
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
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
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

        // 各レコードタイプの解析メソッド
        private void ParseAVRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_AV_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseBNRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_BN_BANUSI>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseBRRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_BR_BREEDER>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseBTRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_BT_KEITO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseCCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_CC_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseCHRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_CH_CHOKYOSI>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseCKRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_CK_CHAKU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseCSRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_CS_COURSE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseDMRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_DM_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseH1Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_H1_HYOSU_ZENKAKE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseH6Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_H6_HYOSU_SANRENTAN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseHCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_HC_HANRO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseHNRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_HN_HANSYOKU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
        }

        private void ParseHRRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_HR_PAY>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseHSRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_HS_SALE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseHYRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_HY_BAMEIORIGIN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseJCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_JC_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseJGRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_JG_JOGAIBA>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseKSRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_KS_KISYU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseO1Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O1_ODDS_TANFUKUWAKU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseO2Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O2_ODDS_UMAREN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseO3Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O3_ODDS_WIDE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseO4Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O4_ODDS_UMATAN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseO5Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O5_ODDS_SANREN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseO6Record(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_O6_ODDS_SANRENTAN>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseRARecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_RA_RACE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // JVParquetと同様のdatetime列とrace_idを追加
            AddRAEnhancedFields(data, record);
        }

        private void ParseRCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_RC_RECORD>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseSERecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_SE_RACE_UMA>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseSKRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_SK_SANKU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseTCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_TC_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseTKRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_TK_TOKUUMA>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseTMRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_TM_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
        }

        private void ParseUMRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_UM_UMA>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
        }

        private void ParseWCRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_WC_WOOD>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        private void ParseWERecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_WE_WEATHER>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseWFRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_WF_INFO>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
        }

        private void ParseWHRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_WH_BATAIJYU>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
            
            // race_idの生成
            AddRaceId(data, record);
            // happyo_datetimeの生成
            AddHappyoDateTime(data, record);
        }

        private void ParseYSRecord(string line, Dictionary<string, object?> record)
        {
            var data = ProcessStructData<JV_YS_SCHEDULE>(line);
            foreach (var kvp in data)
                record[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// race_idを生成（レースIDを持つ全レコード用）
        /// </summary>
        private void AddRaceId(Dictionary<string, object?> data, Dictionary<string, object?> record)
        {
            try
            {
                var year = data.ContainsKey("id_Year") ? data["id_Year"]?.ToString() : "";
                var monthDay = data.ContainsKey("id_MonthDay") ? data["id_MonthDay"]?.ToString() : "";
                var jyoCD = data.ContainsKey("id_JyoCD") ? data["id_JyoCD"]?.ToString() : "";
                var kaiji = data.ContainsKey("id_Kaiji") ? data["id_Kaiji"]?.ToString() : "";
                var nichiji = data.ContainsKey("id_Nichiji") ? data["id_Nichiji"]?.ToString() : "";
                var raceNum = data.ContainsKey("id_RaceNum") ? data["id_RaceNum"]?.ToString() : "";

                if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(monthDay) && !string.IsNullOrEmpty(jyoCD) &&
                    !string.IsNullOrEmpty(kaiji) && !string.IsNullOrEmpty(nichiji) && !string.IsNullOrEmpty(raceNum))
                {
                    record["race_id"] = $"{year}{monthDay.PadLeft(4, '0')}{jyoCD.PadLeft(2, '0')}{kaiji.PadLeft(2, '0')}{nichiji.PadLeft(2, '0')}{raceNum.PadLeft(2, '0')}";
                }
                else
                {
                    record["race_id"] = null;
                }
            }
            catch
            {
                record["race_id"] = null;
            }
        }

        /// <summary>
        /// happyo_datetimeを生成（HappyoTimeを持つ全レコード用）
        /// </summary>
        private void AddHappyoDateTime(Dictionary<string, object?> data, Dictionary<string, object?> record)
        {
            try
            {
                var year = data.ContainsKey("id_Year") ? data["id_Year"]?.ToString() : "";
                var month = data.ContainsKey("HappyoTime_Month") ? data["HappyoTime_Month"]?.ToString() : "";
                var day = data.ContainsKey("HappyoTime_Day") ? data["HappyoTime_Day"]?.ToString() : "";
                var hour = data.ContainsKey("HappyoTime_Hour") ? data["HappyoTime_Hour"]?.ToString() : "";
                var minute = data.ContainsKey("HappyoTime_Minute") ? data["HappyoTime_Minute"]?.ToString() : "";

                if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(day) &&
                    !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(minute) &&
                    month != "00" && day != "00")
                {
                    var happyoDateTime = new DateTime(
                        int.Parse(year),
                        int.Parse(month),
                        int.Parse(day),
                        int.Parse(hour),
                        int.Parse(minute),
                        0
                    );
                    record["happyo_datetime"] = happyoDateTime;
                }
                else
                {
                    record["happyo_datetime"] = null;
                }
            }
            catch
            {
                record["happyo_datetime"] = null;
            }
        }

        /// <summary>
        /// RAレコードに拡張フィールドを追加（JVParquetの実装を参考）
        /// </summary>
        private void AddRAEnhancedFields(Dictionary<string, object?> data, Dictionary<string, object?> record)
        {
            try
            {
                // race_datetimeの計算
                var raceDate = GetRaceDate(data);
                var hassoTime = data.ContainsKey("HassoTime") ? data["HassoTime"]?.ToString() : "";
                
                if (raceDate.HasValue && !string.IsNullOrEmpty(hassoTime) && hassoTime.Length >= 4)
                {
                    // HassoTimeは"HHMM"形式
                    var hour = int.Parse(hassoTime.Substring(0, 2));
                    var minute = int.Parse(hassoTime.Substring(2, 2));
                    var raceDateTime = raceDate.Value.AddHours(hour).AddMinutes(minute);
                    record["race_datetime"] = raceDateTime;
                }
                else
                {
                    record["race_datetime"] = null;
                }

                // track_typeの計算（1:芝, 2:ダート, 3:サンド, 4:障害）
                var trackCD = data.ContainsKey("TrackCD") ? data["TrackCD"]?.ToString() : "";
                record["track_type"] = GetTrackType(trackCD);

                // jyoken_winの計算
                var jyokenCD4 = data.ContainsKey("JyokenInfo_JyokenCD_4") ? data["JyokenInfo_JyokenCD_4"]?.ToString() : "";
                record["jyoken_win"] = !string.IsNullOrEmpty(jyokenCD4) && !new[] { "701", "702", "703" }.Contains(jyokenCD4);

                // race_idの生成
                AddRaceId(data, record);
            }
            catch (Exception ex)
            {
                record["race_datetime"] = null;
                record["track_type"] = 99;
                record["jyoken_win"] = false;
                record["race_id"] = null;
            }
        }

        /// <summary>
        /// レース日付を取得
        /// </summary>
        private DateTime? GetRaceDate(Dictionary<string, object?> data)
        {
            try
            {
                var year = data.ContainsKey("id_Year") ? data["id_Year"]?.ToString() : "";
                var monthDay = data.ContainsKey("id_MonthDay") ? data["id_MonthDay"]?.ToString() : "";

                if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(monthDay) && monthDay.Length >= 4)
                {
                    var month = int.Parse(monthDay.Substring(0, 2));
                    var day = int.Parse(monthDay.Substring(2, 2));
                    return new DateTime(int.Parse(year), month, day);
                }
            }
            catch
            {
                // パースエラーの場合はnullを返す
            }
            return null;
        }

        /// <summary>
        /// トラックタイプを取得（JVParquetの実装と同じ）
        /// </summary>
        private int GetTrackType(string? trackCD)
        {
            if (string.IsNullOrEmpty(trackCD)) return 99;

            // 芝
            string[] trackCdTurf = { "00", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22" };
            // ダート
            string[] trackCdDart = { "23", "24", "25", "26", "29" };
            // サンド
            string[] trackCdSand = { "27", "28" };
            // 障害
            string[] trackCdJump = { "51", "52", "53", "54", "55", "56", "57", "58", "59" };

            if (trackCdTurf.Contains(trackCD)) return 1;
            if (trackCdDart.Contains(trackCD)) return 2;
            if (trackCdSand.Contains(trackCD)) return 3;
            if (trackCdJump.Contains(trackCD)) return 4;
            
            return 99; // その他
        }
    }
}