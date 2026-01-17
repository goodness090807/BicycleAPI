using BicycleAPI.Domain.Entities.User;

namespace BicycleAPI.Domain.UnitTests.Entities.User;

public class UserEntityTests
{
    [Fact(DisplayName = "建立使用者_當資料有效時_應回傳成功")]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var email = "test@example.com";
        var displayName = "Test User";
        var passwordHash = "hashed_password";

        // Act
        var result = UserEntity.Create(email, displayName, passwordHash);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(displayName, result.Value.DisplayName);
        Assert.Equal(passwordHash, result.Value.PasswordHash);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(7, result.Value.Id.Version);
    }

    [Theory(DisplayName = "建立使用者_當Email無效時_應回傳失敗")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenUsernameIsInvalid(string? invalidEmail)
    {
        // Arrange
        var name = "Test User";
        var passwordHash = "hashed_password";

        // Act
        var result = UserEntity.Create(invalidEmail!, name, passwordHash);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.EmptyEmail", result.Error.Code);
    }

    [Theory(DisplayName = "建立使用者_當DisplayName無效時_應回傳失敗")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenDisplayNameIsInvalid(string? invalidDisplayName)
    {
        // Arrange
        var email = "test@example.com";
        var passwordHash = "hashed_password";

        // Act
        var result = UserEntity.Create(email, invalidDisplayName!, passwordHash);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.EmptyDisplayName", result.Error.Code);
    }

    [Theory(DisplayName = "建立使用者_當PasswordHash無效時_應回傳失敗")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnFailure_WhenPasswordHashIsInvalid(string? invalidPasswordHash)
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";

        // Act
        var result = UserEntity.Create(email, name, invalidPasswordHash!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.EmptyPasswordHash", result.Error.Code);
    }
}