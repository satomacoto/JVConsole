using System.Reflection;
using System.Text;
using static JVData_Struct;

namespace JVParquet
{
    public class JVDataParquetConverter : IDisposable
    {
        private readonly string _outputDir;
        private readonly ParquetWriterManager _writerManager;
        private readonly Dictionary<string, List<Dictionary<string, object?>>> _recordBuffers;
        private readonly int _batchSize = 1000;

        public JVDataParquetConverter(string outputDir, string filePrefix = "data")
        {
            _outputDir = outputDir;
            _writerManager = new ParquetWriterManager(outputDir, filePrefix);
            _recordBuffers = new Dictionary<string, List<Dictionary<string, object?>>>();
        }

        public async Task ProcessRecordAsync(string line)
        {
            var recordSpec = line.Substring(0, 2);
            var record = JVReadToParquet(line, recordSpec);

            if (record != null)
            {
                // バッファに追加
                if (!_recordBuffers.ContainsKey(recordSpec))
                {
                    _recordBuffers[recordSpec] = new List<Dictionary<string, object?>>();
                }

                _recordBuffers[recordSpec].Add(record);

                // バッファがバッチサイズに達したら書き込み
                if (_recordBuffers[recordSpec].Count >= _batchSize)
                {
                    await FlushBufferAsync(recordSpec);
                }
            }
        }

        // レコード種別と構造体のマッピング（JVParserと同じ）
        private static readonly Dictionary<string, Type> RecordClassMapping = new Dictionary<string, Type>
        {
            { "AV", typeof(JV_AV_INFO) },
            { "BN", typeof(JV_BN_BANUSI) },
            { "BR", typeof(JV_BR_BREEDER) },
            { "BT", typeof(JV_BT_KEITO) },
            { "CC", typeof(JV_CC_INFO) },
            { "CH", typeof(JV_CH_CHOKYOSI) },
            { "CK", typeof(JV_CK_CHAKU) },
            { "CS", typeof(JV_CS_COURSE) },
            { "DM", typeof(JV_DM_INFO) },
            { "H1", typeof(JV_H1_HYOSU_ZENKAKE) },
            { "H6", typeof(JV_H6_HYOSU_SANRENTAN) },
            { "HC", typeof(JV_HC_HANRO) },
            { "HN", typeof(JV_HN_HANSYOKU) },
            { "HR", typeof(JV_HR_PAY) },
            { "HS", typeof(JV_HS_SALE) },
            { "HY", typeof(JV_HY_BAMEIORIGIN) },
            { "JC", typeof(JV_JC_INFO) },
            { "JG", typeof(JV_JG_JOGAIBA) },
            { "KS", typeof(JV_KS_KISYU) },
            { "O1", typeof(JV_O1_ODDS_TANFUKUWAKU) },
            { "O2", typeof(JV_O2_ODDS_UMAREN) },
            { "O3", typeof(JV_O3_ODDS_WIDE) },
            { "O4", typeof(JV_O4_ODDS_UMATAN) },
            { "O5", typeof(JV_O5_ODDS_SANREN) },
            { "O6", typeof(JV_O6_ODDS_SANRENTAN) },
            { "RA", typeof(JV_RA_RACE) },
            { "RC", typeof(JV_RC_RECORD) },
            { "SE", typeof(JV_SE_RACE_UMA) },
            { "SK", typeof(JV_SK_SANKU) },
            { "TC", typeof(JV_TC_INFO) },
            { "TK", typeof(JV_TK_TOKUUMA) },
            { "TM", typeof(JV_TM_INFO) },
            { "UM", typeof(JV_UM_UMA) },
            { "WC", typeof(JV_WC_WOOD) },
            { "WE", typeof(JV_WE_WEATHER) },
            { "WF", typeof(JV_WF_INFO) },
            { "WH", typeof(JV_WH_BATAIJYU) },
            { "YS", typeof(JV_YS_SCHEDULE) }
        };

        private Dictionary<string, object?>? JVReadToParquet(string line, string recordSpec)
        {
            try
            {
                // レコード種別に対応する構造体を取得
                if (!RecordClassMapping.TryGetValue(recordSpec, out Type? structType))
                {
                    Console.WriteLine($"Warning: Structure not found for record spec: {recordSpec}");
                    return null;
                }


                // インスタンスを作成
                var structInstance = Activator.CreateInstance(structType);
                if (structInstance == null)
                {
                    return null;
                }

                // SetDataBメソッドを取得して実行
                var setDataBMethod = structType.GetMethod("SetDataB", BindingFlags.Public | BindingFlags.Instance);
                if (setDataBMethod != null)
                {
                    // ref stringパラメータに対応
                    var parameters = setDataBMethod.GetParameters();
                    
                    
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string).MakeByRefType())
                    {
                        // string版のSetDataB
                        object[] args = new object[] { line };
                        setDataBMethod.Invoke(structInstance, args);
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(byte[]).MakeByRefType())
                    {
                        // byte[]版のSetDataB
                        byte[] bBuff = Str2Byte(line);
                        object[] args = new object[] { bBuff };
                        setDataBMethod.Invoke(structInstance, args);
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: SetDataB method not found for {structType.Name}");
                }

                // リフレクションで直接フラット化
                var flattenedDict = ReflectionFlattener.FlattenStruct(structInstance);


                return flattenedDict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing record spec {recordSpec}: {ex.Message}");
                return null;
            }
        }


        private async Task FlushBufferAsync(string recordSpec)
        {
            if (_recordBuffers.ContainsKey(recordSpec) && _recordBuffers[recordSpec].Count > 0)
            {
                await _writerManager.WriteRecordsAsync(recordSpec, _recordBuffers[recordSpec]);
                _recordBuffers[recordSpec].Clear();
            }
        }

        public async Task DisposeAsync()
        {
            // すべてのバッファをフラッシュ
            foreach (var recordSpec in _recordBuffers.Keys.ToList())
            {
                await FlushBufferAsync(recordSpec);
            }

            // WriterManagerを閉じる
            _writerManager.Dispose();
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 文字列をShift-JISバイト配列に変換
        /// </summary>
        private static byte[] Str2Byte(string str)
        {
            return Encoding.GetEncoding("Shift_JIS").GetBytes(str);
        }
    }
}