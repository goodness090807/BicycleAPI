using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;
