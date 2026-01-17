using Shared.ResultPatterns;

namespace BicycleAPI.Domain.Entities.User;

public class UserEntity : BaseAuditableEntity
{
    private UserEntity(string email, string displayName, string passwordHash)
    {
        Id = Guid.CreateVersion7();
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public string PasswordHash { get; private set; }

    // 關聯屬性
    public ICollection<UserRole.UserRoleEntity> UserRoles { get; private set; } = new List<UserRole.UserRoleEntity>();

    public static Result<UserEntity> Create(string email, string displayName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<UserEntity>.Failure(
                new DomainError("User.EmptyEmail", "User", "Email 不能為空。")
            );
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result<UserEntity>.Failure(
                new DomainError("User.EmptyDisplayName", "User", "使用者名稱不能為空。")
            );
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result<UserEntity>.Failure(
                new DomainError("User.EmptyPasswordHash", "User", "密碼雜湊不能為空。")
            );
        }

        var user = new UserEntity(email, displayName, passwordHash);

        return Result<UserEntity>.Success(user);
    }
}
