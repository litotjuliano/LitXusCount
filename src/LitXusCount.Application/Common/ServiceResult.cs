namespace LitXusCount.Application.Common;

public record ServiceResult<T>
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public T? Value { get; init; }

    public static ServiceResult<T> Success(T value) => new() { Succeeded = true, Value = value };
    public static ServiceResult<T> Failure(string error) => new() { Succeeded = false, Error = error };
}
