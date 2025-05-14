namespace Demo.DAL.Abstractions;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string? Error { get; }
    public int? ErrorCode { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
        ErrorCode = null;
    }

    private Result(string? error, int errorCode)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(string? error, int errorCode) => new Result<T>(error, errorCode);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public int? ErrorCode { get; }

    private Result()
    {
        IsSuccess = true;
        Error = null;
        ErrorCode = null;
    }

    private Result(string? error, int errorCode)
    {
        IsSuccess = false;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new Result();
    public static Result Failure(string? error, int errorCode) => new Result(error, errorCode);
}