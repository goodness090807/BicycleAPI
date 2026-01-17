using BicycleAPI.Application.Behaviors;
using MediatR;
using Shared.ResultPatterns;


namespace BicycleAPI.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string DisplayName,
    string Password,
    string ConfirmPassword
) : IRequest<Result<RegisterResponse>>, ITransactionalCommand;