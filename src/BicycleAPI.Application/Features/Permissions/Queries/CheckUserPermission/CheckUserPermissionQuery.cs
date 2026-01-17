using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Permissions.Queries.CheckUserPermission;

public record CheckUserPermissionQuery(
    Guid UserId,
    string PermissionCode
) : IRequest<Result<bool>>;
