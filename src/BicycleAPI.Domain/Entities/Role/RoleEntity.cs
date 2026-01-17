using Shared.ResultPatterns;

namespace BicycleAPI.Domain.Entities.Role;

public class RoleEntity : BaseAuditableEntity
{
    private RoleEntity(string code, string? name, string? description)
    {
        Id = Guid.CreateVersion7();
        Code = code;
        Name = name;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    // 關聯屬性
    public ICollection<UserRole.UserRoleEntity> UserRoles { get; private set; } = new List<UserRole.UserRoleEntity>();
    public ICollection<RolePermission.RolePermissionEntity> RolePermissions { get; private set; } = new List<RolePermission.RolePermissionEntity>();

    public static Result<RoleEntity> Create(string code, string? name = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<RoleEntity>.Failure(
                new DomainError("Role.EmptyCode", "Role", "角色代碼不能為空。")
            );
        }

        var role = new RoleEntity(code, name, description);

        return Result<RoleEntity>.Success(role);
    }
}
