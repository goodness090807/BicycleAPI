namespace BicycleAPI.Application.Features.Auth.Commands.Register;

public record RegisterResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
