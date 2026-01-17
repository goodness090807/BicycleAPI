namespace BicycleAPI.Domain.Entities.UserRole;

public class UserRoleEntity
{
    public UserRoleEntity(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    // 關聯屬性
    public User.UserEntity User { get; private set; } = null!;
    public Role.RoleEntity Role { get; private set; } = null!;
}
