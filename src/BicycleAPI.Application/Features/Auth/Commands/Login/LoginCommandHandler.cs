using BicycleAPI.Domain.Entities.RefreshToken;
using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;
using Shared.Security.JWTHandler;
using Shared.Security.PasswordHasher;

namespace BicycleAPI.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHandler _tokenHandler;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        ITokenHandler tokenHandler)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _tokenHandler = tokenHandler;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 根據 Email 查找使用者
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            return Result<LoginResponse>.Failure(
                new UnauthorizedError("Auth.InvalidCredentials", "Unauthorized", "Email 或密碼錯誤。")
            );
        }

        // 驗證密碼
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(
                new UnauthorizedError("Auth.InvalidCredentials", "Unauthorized", "Email 或密碼錯誤。")
            );
        }

        // 取得Roles
        var roles = await _userRoleRepository.GetByUserIdAsync(user.Id, cancellationToken);

        // 生成 JWT Token
        var accessToken = _tokenHandler.GenerateJwtToken(user.Id.ToString(), roles.Select(r => r.Role.Code).ToArray());

        // 生成 Refresh Token
        var refreshTokenValue = _tokenHandler.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.Hash(refreshTokenValue);
        var refreshTokenExpirationDays = _tokenHandler.GetRefreshTokenExpirationDays();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        var refreshTokenResult = RefreshTokenEntity.Create(user.Id, refreshTokenHash, refreshTokenExpiresAt);
        if (refreshTokenResult.IsFailure)
        {
            return Result<LoginResponse>.Failure(refreshTokenResult.Error);
        }

        await _refreshTokenRepository.AddAsync(refreshTokenResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(
            UserId: user.Id,
            Email: user.Email,
            DisplayName: user.DisplayName,
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue,
            RefreshTokenExpiresAt: refreshTokenExpiresAt
        );

        return Result<LoginResponse>.Success(response);
    }
}
