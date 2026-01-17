using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // 如果有錯誤，中斷 Pipeline，回傳失敗 Result
        if (failures.Any())
        {
            return CreateFailureResult(failures);
        }

        // 驗證通過，進入下一層
        return await next();
    }

    private static TResponse CreateFailureResult(List<ValidationFailure> validationFailures)
    {
        var failures = validationFailures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => new ValidationErrorDetail(
                    Code: f.ErrorCode,
                    Message: f.ErrorMessage
                )).ToArray()
            );

        var errorDetails = new ValidationError("Validation.General", failures);

        if (typeof(TResponse) == typeof(Result))
        {
            // 直接回非泛型 Result
            var failure = Result.Failure(errorDetails);
            return (TResponse)(object)failure;
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArg = typeof(TResponse).GetGenericArguments()[0];
            var genericResultType = typeof(Result<>).MakeGenericType(genericArg);

            var failureMethod = genericResultType.GetMethod(nameof(Result<object>.Failure), BindingFlags.Public | BindingFlags.Static)!;

            var failure = failureMethod.Invoke(null, new object[] { errorDetails })!;
            return (TResponse)failure;
        }

        throw new InvalidOperationException($"ValidationBehavior 只支援 Result or Result<T>, 但使用到 {typeof(TResponse).Name} 類別");
    }
}
