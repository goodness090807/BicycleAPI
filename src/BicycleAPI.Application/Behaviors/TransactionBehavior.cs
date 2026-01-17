using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionalCommand)
        {
            return await next();
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var response = await next();

            if (response is IResult result && result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();

                // 直接回傳失敗的 Result 給上層
                return response;
            }

            await _unitOfWork.CommitTransactionAsync();
            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
