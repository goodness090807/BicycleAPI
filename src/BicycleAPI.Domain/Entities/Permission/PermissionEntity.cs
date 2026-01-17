using Shared.ResultPatterns;

namespace BicycleAPI.Domain.Entities.Permission;

public class PermissionEntity : BaseAuditableEntity
{
    private PermissionEntity(string code, string name, string? group)
    {
        Id = Guid.CreateVersion7();
        Code = code;
        Name = name;
        Group = group;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Group { get; private set; }

    // 關聯屬性
    public ICollection<RolePermission.RolePermissionEntity> RolePermissions { get; private set; } = new List<RolePermission.RolePermissionEntity>();

    public static Result<PermissionEntity> Create(string code, string name, string? group = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<PermissionEntity>.Failure(
                new DomainError("Permission.EmptyCode", "Permission", "權限代碼不能為空。")
            );
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<PermissionEntity>.Failure(
                new DomainError("Permission.EmptyName", "Permission", "權限名稱不能為空。")
            );
        }

        var permission = new PermissionEntity(code, name, group);

        return Result<PermissionEntity>.Success(permission);
    }
}
