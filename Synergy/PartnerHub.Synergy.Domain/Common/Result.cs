namespace PartnersHub.Synergy.Domain.Common;

/// <summary>
/// Represents the result of a domain operation with success/failure state and error information
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private set; }
    public string? Error { get; private set; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value, string errorMessage = null) => new(true, value, errorMessage);
    public static Result<T> Failure(string error) => new(false, default, error);
}

/// <summary>
/// Represents the result of a domain operation without a return value
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}
