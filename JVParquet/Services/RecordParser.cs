using System.Reflection;
using System.Text;
using JVParquet.Core;
using JVParquet.Exceptions;
using JVParquet.Interfaces;
using static JVData_Struct;

namespace JVParquet.Services
{
    public class RecordParser : IRecordParser
    {
        private static readonly Dictionary<string, Type> RecordClassMapping = new()
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

        public Result<ParsedRecord> ParseRecord(string line)
        {
            try
            {
                if (string.IsNullOrEmpty(line))
                    return Result<ParsedRecord>.Failure("Line is empty");

                if (line.Length < Constants.RecordSpecLength)
                    return Result<ParsedRecord>.Failure("Line too short to contain record spec");

                var recordSpec = line.Substring(0, Constants.RecordSpecLength);
                
                if (!RecordClassMapping.TryGetValue(recordSpec, out var structType))
                    return Result<ParsedRecord>.Failure($"Unknown record spec: {recordSpec}");

                var structInstance = CreateAndPopulateStruct(line, structType);
                if (structInstance == null)
                    return Result<ParsedRecord>.Failure("Failed to create struct instance");

                var flattenedData = ReflectionFlattener.FlattenStruct(structInstance);
                var makeDate = ExtractMakeDate(flattenedData);

                return Result<ParsedRecord>.Success(new ParsedRecord
                {
                    RecordSpec = recordSpec,
                    Data = flattenedData,
                    MakeDate = makeDate
                });
            }
            catch (Exception ex)
            {
                return Result<ParsedRecord>.Failure(ex);
            }
        }

        private object? CreateAndPopulateStruct(string line, Type structType)
        {
            var structInstance = Activator.CreateInstance(structType);
            if (structInstance == null) return null;

            var setDataBMethod = structType.GetMethod("SetDataB", BindingFlags.Public | BindingFlags.Instance);
            if (setDataBMethod == null) return structInstance;

            var parameters = setDataBMethod.GetParameters();
            if (parameters.Length == 1)
            {
                if (parameters[0].ParameterType == typeof(string).MakeByRefType())
                {
                    object[] args = new object[] { line };
                    setDataBMethod.Invoke(structInstance, args);
                }
                else if (parameters[0].ParameterType == typeof(byte[]).MakeByRefType())
                {
                    byte[] bBuff = ConvertToShiftJisBytes(line);
                    object[] args = new object[] { bBuff };
                    setDataBMethod.Invoke(structInstance, args);
                }
            }

            return structInstance;
        }

        private DateTime? ExtractMakeDate(Dictionary<string, object?> data)
        {
            if (data.TryGetValue(Constants.FieldNames.HeadMakeDateYear, out var year) &&
                data.TryGetValue(Constants.FieldNames.HeadMakeDateMonth, out var month) &&
                data.TryGetValue(Constants.FieldNames.HeadMakeDateDay, out var day))
            {
                try
                {
                    var yearInt = Convert.ToInt32(year);
                    var monthInt = Convert.ToInt32(month);
                    var dayInt = Convert.ToInt32(day);
                    
                    if (yearInt > 0 && monthInt > 0 && monthInt <= 12 && dayInt > 0 && dayInt <= 31)
                        return new DateTime(yearInt, monthInt, dayInt);
                }
                catch
                {
                    // 日付変換に失敗した場合はnullを返す
                }
            }

            return null;
        }

        private static byte[] ConvertToShiftJisBytes(string str)
        {
            return Encoding.GetEncoding("Shift_JIS").GetBytes(str);
        }
    }
}