using Microsoft.AspNetCore.Authorization;

namespace BicycleAPI.Api.Authorizations.PermissionAuthorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionRequirement(string permissionName)
    {
        PermissionName = permissionName;
    }
}
