namespace Shared.ResultPatterns;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
}
