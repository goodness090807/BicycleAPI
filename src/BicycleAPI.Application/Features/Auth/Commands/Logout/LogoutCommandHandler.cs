using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(
        IUnitOfWork unitOfWork,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByHashTokenAsync(request.RefreshToken, cancellationToken);

        if (existingToken == null)
        {
            // Token 不存在，但仍視為登出成功（冪等性）
            return Result.Success();
        }

        if (existingToken.IsActive)
        {
            existingToken.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
