using BicycleAPI.Domain.Entities.RefreshToken;
using BicycleAPI.Domain.Entities.User;
using BicycleAPI.Domain.Entities.UserRole;
using BicycleAPI.Domain.Repositories;
using BicycleAPI.Domain.Variables;
using MediatR;
using Shared.ResultPatterns;
using Shared.Security.JWTHandler;
using Shared.Security.PasswordHasher;

namespace BicycleAPI.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHandler _tokenHandler;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        ITokenHandler tokenHandler)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHasher = passwordHasher;
        _tokenHandler = tokenHandler;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByEmailAsync(request.Email, cancellationToken) != null)
        {
            return Result<RegisterResponse>.Failure(
                new ConflictError("Register.EmailAlreadyExists", "User", "此 Email 已被註冊。")
            );
        }

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var userResult = UserEntity.Create(request.Email, request.DisplayName, hashedPassword);
        if (userResult.IsFailure)
        {
            return Result<RegisterResponse>.Failure(userResult.Error);
        }

        var user = userResult.Value;
        await _userRepository.AddAsync(user);

        // 取得預設角色 (Member)
        var memberRole = await _roleRepository.GetByCodeAsync(Roles.MemberCode, cancellationToken);
        if (memberRole == null)
        {
            return Result<RegisterResponse>.Failure(
                new NotFoundError("Register.DefaultRoleNotFound", "Role", "找不到預設角色。")
            );
        }

        // 建立 User 與 Role 的關聯
        var userRole = new UserRoleEntity(user.Id, memberRole.Id);
        await _userRoleRepository.AddAsync(userRole);

        // 生成 JWT Token
        var accessToken = _tokenHandler.GenerateJwtToken(user.Id.ToString(), [memberRole.Code]);

        // 生成 Refresh Token
        var refreshTokenValue = _tokenHandler.GenerateRefreshToken();
        var refreshTokenHash = _passwordHasher.Hash(refreshTokenValue);
        var refreshTokenExpirationDays = _tokenHandler.GetRefreshTokenExpirationDays();
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        var refreshTokenResult = RefreshTokenEntity.Create(user.Id, refreshTokenHash, refreshTokenExpiresAt);
        if (refreshTokenResult.IsFailure)
        {
            return Result<RegisterResponse>.Failure(refreshTokenResult.Error);
        }

        await _refreshTokenRepository.AddAsync(refreshTokenResult.Value);

        var response = new RegisterResponse(
            UserId: user.Id,
            Email: user.Email,
            DisplayName: user.DisplayName,
            AccessToken: accessToken,
            RefreshToken: refreshTokenValue,
            RefreshTokenExpiresAt: refreshTokenExpiresAt
        );

        return Result<RegisterResponse>.Success(response);
    }
}
