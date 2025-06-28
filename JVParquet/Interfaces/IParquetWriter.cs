using JVParquet.Core;

namespace JVParquet.Interfaces
{
    public interface IParquetWriter : IDisposable
    {
        Task<Result> WriteRecordsAsync(string recordSpec, IEnumerable<Dictionary<string, object?>> records);
        Task<Result> CloseAsync();
    }
}