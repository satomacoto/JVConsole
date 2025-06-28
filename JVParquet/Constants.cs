namespace JVParquet
{
    public static class Constants
    {
        public const int DefaultBatchSize = 1000;
        public const int RecordSpecLength = 2;
        public const int ProgressReportInterval = 1000;
        
        public static class FieldNames
        {
            public const string HeadRecordSpec = "head_RecordSpec";
            public const string HeadDataKubun = "head_DataKubun";
            public const string HeadMakeDateYear = "head_MakeDate_Year";
            public const string HeadMakeDateMonth = "head_MakeDate_Month";
            public const string HeadMakeDateDay = "head_MakeDate_Day";
        }
        
        public static class PartitionKeys
        {
            public const string Year = "year";
            public const string Month = "month";
            public const string Day = "day";
            public const string Unknown = "unknown";
        }
        
        public static class FileExtensions
        {
            public const string Parquet = ".parquet";
            public const string Json = ".json";
            public const string Text = ".txt";
        }
    }
}