using Shared.ResultPatterns;

namespace BicycleAPI.Domain.Entities.RefreshToken;

public class RefreshTokenEntity : BaseAuditableEntity
{
    private RefreshTokenEntity(Guid userId, string tokenHash, DateTime expiresAt)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    // 關聯屬性
    public User.UserEntity User { get; private set; } = null!;

    public static Result<RefreshTokenEntity> Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
        {
            return Result<RefreshTokenEntity>.Failure(
                new DomainError("RefreshToken.InvalidUserId", "RefreshToken", "使用者 ID 不能為空。")
            );
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return Result<RefreshTokenEntity>.Failure(
                new DomainError("RefreshToken.EmptyTokenHash", "RefreshToken", "TokenHash 不能為空。")
            );
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            return Result<RefreshTokenEntity>.Failure(
                new DomainError("RefreshToken.InvalidExpiration", "RefreshToken", "過期時間必須大於目前時間。")
            );
        }

        var refreshToken = new RefreshTokenEntity(userId, tokenHash, expiresAt);
        return Result<RefreshTokenEntity>.Success(refreshToken);
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
