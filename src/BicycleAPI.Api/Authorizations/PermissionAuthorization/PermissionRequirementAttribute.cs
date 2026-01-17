using Microsoft.AspNetCore.Authorization;

namespace BicycleAPI.Api.Authorizations.PermissionAuthorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class PermissionRequirementAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public PermissionRequirementAttribute(string permissionCode)
    {
        // 自動組合成 Policy 字串，例如 "Permission:Product.Delete"
        Policy = $"{PolicyPrefix}{permissionCode}";
    }
}
