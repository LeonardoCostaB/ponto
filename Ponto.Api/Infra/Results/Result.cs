namespace Ponto.Api.Infra.Results;

public record ResultError(string Code, string Message);

public class Result
{
    public bool IsSuccess { get; init; }
    public IReadOnlyList<ResultError> Errors { get; init; } = [];

    public static Result Ok() => new() { IsSuccess = true };

    public static Result Fail(params ResultError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    public static new Result<T> Fail(params ResultError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}
