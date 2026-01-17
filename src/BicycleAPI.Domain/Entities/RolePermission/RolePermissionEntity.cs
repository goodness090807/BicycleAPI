namespace BicycleAPI.Domain.Entities.RolePermission;

public class RolePermissionEntity
{
    public RolePermissionEntity(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    // 關聯屬性
    public Role.RoleEntity Role { get; private set; } = null!;
    public Permission.PermissionEntity Permission { get; private set; } = null!;
}
