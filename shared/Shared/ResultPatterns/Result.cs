namespace Shared.ResultPatterns;

public class Result : IResult
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("成功結果不能包含錯誤。");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("失敗結果必須包含錯誤。");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(bool isSuccess, Error error, TValue? value) : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("無法取得失敗結果的值。");

    public static Result<TValue> Success(TValue value) => new(true, Error.None, value);
    public static new Result<TValue> Failure(Error error) => new(false, error, default);
}
