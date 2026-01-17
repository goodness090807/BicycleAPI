namespace BicycleAPI.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
