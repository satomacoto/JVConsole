using System.Collections.Concurrent;
using JVParquet.Configuration;
using JVParquet.Core;
using JVParquet.Interfaces;

namespace JVParquet.Services
{
    public class BufferManager<T> : IBufferManager<T>
    {
        private readonly ConcurrentDictionary<string, List<T>> _buffers = new();
        private readonly ParquetSettings _settings;
        private readonly Func<string, IEnumerable<T>, Task<Result>> _flushAction;
        private readonly SemaphoreSlim _semaphore;

        public BufferManager(
            ParquetSettings settings, 
            Func<string, IEnumerable<T>, Task<Result>> flushAction)
        {
            _settings = settings;
            _flushAction = flushAction;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<Result> AddAsync(string key, T item)
        {
            try
            {
                await _semaphore.WaitAsync();
                
                var buffer = _buffers.GetOrAdd(key, _ => new List<T>());
                buffer.Add(item);

                if (ShouldFlush(key))
                {
                    return await FlushInternalAsync(key, buffer);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> FlushAsync(string key)
        {
            try
            {
                await _semaphore.WaitAsync();
                
                if (_buffers.TryGetValue(key, out var buffer) && buffer.Count > 0)
                {
                    return await FlushInternalAsync(key, buffer);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> FlushAllAsync()
        {
            var tasks = new List<Task<Result>>();
            
            foreach (var key in _buffers.Keys.ToList())
            {
                tasks.Add(FlushAsync(key));
            }

            var results = await Task.WhenAll(tasks);
            
            var failedResults = results.Where(r => !r.IsSuccess).ToList();
            if (failedResults.Any())
            {
                var errors = string.Join("; ", failedResults.Select(r => r.Error));
                return Result.Failure($"Failed to flush some buffers: {errors}");
            }

            return Result.Success();
        }

        public int GetBufferSize(string key)
        {
            return _buffers.TryGetValue(key, out var buffer) ? buffer.Count : 0;
        }

        public bool ShouldFlush(string key)
        {
            return GetBufferSize(key) >= _settings.BatchSize;
        }

        private async Task<Result> FlushInternalAsync(string key, List<T> buffer)
        {
            if (buffer.Count == 0)
                return Result.Success();

            var itemsToFlush = new List<T>(buffer);
            buffer.Clear();

            var result = await _flushAction(key, itemsToFlush);
            
            if (!result.IsSuccess)
            {
                // フラッシュに失敗した場合、アイテムをバッファに戻す
                buffer.AddRange(itemsToFlush);
            }

            return result;
        }

        public void Dispose()
        {
            FlushAllAsync().GetAwaiter().GetResult();
            _semaphore?.Dispose();
        }
    }
}