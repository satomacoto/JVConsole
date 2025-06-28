namespace JVParquet.Core
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }
        public Exception? Exception { get; }

        private Result(bool isSuccess, T? value, string? error, Exception? exception)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Exception = exception;
        }

        public static Result<T> Success(T value) => new(true, value, null, null);
        
        public static Result<T> Failure(string error) => new(false, default, error, null);
        
        public static Result<T> Failure(Exception exception) => 
            new(false, default, exception.Message, exception);

        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (!IsSuccess || Value is null)
                return Result<TNew>.Failure(Error ?? "Value is null");
            
            try
            {
                return Result<TNew>.Success(mapper(Value));
            }
            catch (Exception ex)
            {
                return Result<TNew>.Failure(ex);
            }
        }

        public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
        {
            if (!IsSuccess || Value is null)
                return Result<TNew>.Failure(Error ?? "Value is null");
            
            try
            {
                var result = await mapper(Value);
                return Result<TNew>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<TNew>.Failure(ex);
            }
        }
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public Exception? Exception { get; }

        private Result(bool isSuccess, string? error, Exception? exception)
        {
            IsSuccess = isSuccess;
            Error = error;
            Exception = exception;
        }

        public static Result Success() => new(true, null, null);
        public static Result Failure(string error) => new(false, error, null);
        public static Result Failure(Exception exception) => new(false, exception.Message, exception);
    }
}