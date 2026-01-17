using BicycleAPI.Domain.Entities.RefreshToken;
using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;
using Shared.Security.JWTHandler;
using Shared.Security.PasswordHasher;

namespace BicycleAPI.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHandler _tokenHandler;
    private readonly IPasswordHasher _passwordHasher;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHandler tokenHandler,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHandler = tokenHandler;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _passwordHasher.Hash(request.RefreshToken);

        var existingToken = await _refreshTokenRepository.GetByHashTokenAsync(tokenHash, cancellationToken);

        if (existingToken == null)
        {
            return Result<RefreshTokenResponse>.Failure(
                new NotFoundError("RefreshToken.NotFound", "RefreshToken", "Refresh Token 不存在。")
            );
        }

        if (!existingToken.IsActive)
        {
            return Result<RefreshTokenResponse>.Failure(
                new UnauthorizedError("RefreshToken.Invalid", "Unauthorized", "Refresh Token 已失效或已過期。")
            );
        }

        // 撤銷舊的 Refresh Token
        existingToken.Revoke();

        // 生成新的 Access Token
        var newAccessToken = _tokenHandler.GenerateJwtToken(existingToken.UserId.ToString());

        // 生成新的 Refresh Token
        var newRefreshTokenValue = _tokenHandler.GenerateRefreshToken();
        var newRefreshTokenHash = _passwordHasher.Hash(newRefreshTokenValue);
        var refreshTokenExpirationDays = _tokenHandler.GetRefreshTokenExpirationDays();
        var newRefreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        var newRefreshTokenResult = RefreshTokenEntity.Create(existingToken.UserId, newRefreshTokenHash, newRefreshTokenExpiresAt);
        if (newRefreshTokenResult.IsFailure)
        {
            return Result<RefreshTokenResponse>.Failure(newRefreshTokenResult.Error);
        }

        await _refreshTokenRepository.AddAsync(newRefreshTokenResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RefreshTokenResponse(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshTokenValue,
            RefreshTokenExpiresAt: newRefreshTokenExpiresAt
        );

        return Result<RefreshTokenResponse>.Success(response);
    }
}
