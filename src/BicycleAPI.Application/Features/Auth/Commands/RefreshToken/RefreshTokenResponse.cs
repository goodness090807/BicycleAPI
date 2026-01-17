namespace BicycleAPI.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
