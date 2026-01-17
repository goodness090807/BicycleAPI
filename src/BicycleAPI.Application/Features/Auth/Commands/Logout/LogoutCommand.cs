
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(
    string RefreshToken
) : IRequest<Result>;
