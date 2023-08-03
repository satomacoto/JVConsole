namespace JVParser
{
    class OutputFileAlreadyExistsException : Exception
    {
        public OutputFileAlreadyExistsException(string? message) : base("Output file already exsits. path: " + message)
        {
        }
    }
}

