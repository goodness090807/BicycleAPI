using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Permissions.Queries.CheckUserPermission;

public class CheckUserPermissionQueryHandler : IRequestHandler<CheckUserPermissionQuery, Result<bool>>
{
    private readonly IPermissionRepository _permissionRepository;

    public CheckUserPermissionQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<bool>> Handle(CheckUserPermissionQuery request, CancellationToken cancellationToken)
    {
        var permissionCodes = await _permissionRepository.GetPermissionCodesByUserIdAsync(
            request.UserId,
            cancellationToken);

        var hasPermission = permissionCodes.Contains(request.PermissionCode);

        return Result<bool>.Success(hasPermission);
    }
}
