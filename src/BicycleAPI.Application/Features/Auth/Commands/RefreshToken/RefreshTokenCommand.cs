using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<RefreshTokenResponse>>;
