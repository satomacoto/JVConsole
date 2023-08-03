using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static JVData_Struct;

namespace JVParser
{
    internal class JVJson
    {
        public string recordSpec { get; set; }
        public string json { get; set; }

        public JVJson(string recordSpec, string json)
        {
            this.recordSpec = recordSpec;
            this.json = json;
        }
    }

    internal class Options
    {
        [Option("inputPath", Required = true, HelpText = @"Path to txt file.")]
        public string InputPath { get; set; }

        [Option("outputDir", Required = true, HelpText = @"Path to jsonl directory.")]
        public string OutputDir { get; set; }

        // recordSpec to skip
        [Option("skipRecordSpec", Required = false, Separator = ',', HelpText = @"RecordSpec to skip.")]
        public IEnumerable<string> SkipRecordSpec { get; set; }

    }

    internal class ErrorCode
    {
        public const int Success = 0;
        public const int ErrorParsingArguments = 401;
        public const int ErrorOutputFileAlreadyExists = 402;
    }


    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions).WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options opts)
        {

            // To use Shift-JIS encoding, use the following:
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Get the input file path
            string inputFilePath = opts.InputPath;

            // Get output directory from args
            string outputDir = opts.OutputDir;

            // Get skipRecordSpec from args
            IEnumerable<string> skipRecordSpec = opts.SkipRecordSpec;

            // Get input file name without extension
            string fileNamePrefix = Path.GetFileNameWithoutExtension(inputFilePath);

            // Initalize the recordspec stream writer manager
            RecordSpecStreamWriterManager recordSpecStreamWriterManager = new RecordSpecStreamWriterManager(outputDir, fileNamePrefix);

            string? line;
            JVJson? jvJson;


            // Read from the input file
            using (StreamReader sr = new StreamReader(inputFilePath))
            {
                // Measure the calculation time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int lineNumber = 0;
                // read line and convert to json
                while ((line = sr.ReadLine()) != null)
                {
                    if ((jvJson = JVReadToJson(line, skipRecordSpec)) != null)
                    {
                        try
                        {
                            // Write to the output file
                            recordSpecStreamWriterManager.WriteLineToStreamWriter(jvJson.recordSpec, jvJson.json);
                        }
                        catch (OutputFileAlreadyExistsException e)
                        {
                            Console.Error.WriteLine(e.Message);
                            Environment.Exit(ErrorCode.ErrorOutputFileAlreadyExists);
                        }
                    }
                    lineNumber++;

                    // Print progress
                    if (lineNumber % 1000 == 0)
                    {
                        Console.Error.Write("Processed " + lineNumber + " lines in " + stopwatch.ElapsedMilliseconds + " ms.\r");
                    }
                }
                Console.Error.WriteLine("Parsed " + lineNumber + " lines in " + stopwatch.ElapsedMilliseconds + " ms.");

                recordSpecStreamWriterManager.PrintOutputPaths();

                recordSpecStreamWriterManager.Close();
            }

            // Exit with success code
            Environment.Exit(ErrorCode.Success);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            // Handle errors
            Console.Error.WriteLine("Error parsing arguments.");
            Environment.Exit(ErrorCode.ErrorParsingArguments);
        }

        static JVJson? JVReadToJson(string line, IEnumerable<string> skipRecordSpec)
        {
            var recordClassMapping = new Dictionary<string, Type>
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
                { "WC", typeof(JV_WC_WOODCHIP) },
                { "WE", typeof(JV_WE_WEATHER) },
                { "WF", typeof(JV_WF_INFO) },
                { "WH", typeof(JV_WH_BATAIJYU) },
                { "YS", typeof(JV_YS_SCHEDULE) }
            };


            JObject? jsonObject = null;
            var recordSpec = line.Substring(0, 2);

            if (skipRecordSpec.Contains(recordSpec))
            {
                return null;
            }

            if (!recordClassMapping.ContainsKey(recordSpec))
            {
                return null;
            }

            if (recordClassMapping.TryGetValue(recordSpec, out Type recordType))
            {
                var recordInstance = Activator.CreateInstance(recordType);
                var setDataBMethod = recordType.GetMethod("SetDataB");

                if (setDataBMethod != null)
                {
                    setDataBMethod.Invoke(recordInstance, new object[] { line });
                    jsonObject = JObject.FromObject(recordInstance);
                }
            }

            if (jsonObject != null)
            {
                var flattened = jsonObject
                    .SelectTokens("$..*")
                    .Where(t => !t.HasValues)
                    .ToDictionary(t => t.Path, t => t.ToString());

                return new JVJson(recordSpec, JsonConvert.SerializeObject(flattened));
            }
            else
            {
                return null;
            }
        }
    }
}

