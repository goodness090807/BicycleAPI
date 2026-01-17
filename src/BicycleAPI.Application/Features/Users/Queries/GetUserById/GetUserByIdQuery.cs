using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Queries.GetUserById;

public record GetUserByIdQuery(
    Guid UserId
) : IRequest<Result<GetUserByIdResponse>>;
