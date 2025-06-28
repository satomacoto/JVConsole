using JVParquet.Core;

namespace JVParquet.Interfaces
{
    public interface IBufferManager<T> : IDisposable
    {
        Task<Result> AddAsync(string key, T item);
        Task<Result> FlushAsync(string key);
        Task<Result> FlushAllAsync();
        int GetBufferSize(string key);
        bool ShouldFlush(string key);
    }
}