
using Shared.Security.PasswordHasher;

namespace Shared.UnitTests.Security.PasswordHasher;

public class BcryptPasswordHasherTests
{
    private readonly IPasswordHasher _sut;

    public BcryptPasswordHasherTests()
    {
        _sut = new BcryptPasswordHasher();
    }

    [Fact(DisplayName = "當傳入密碼時，應回傳有效的Hash字串")]
    public void Hash_ShouldReturnValidHashString_WhenPasswordIsNotNull()
    {
        // Arrange
        string password = "TestPassword123!";

        // Act
        string result = _sut.Hash(password);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact(DisplayName = "當傳入正確的密碼與Hash字串時，應回傳驗證成功")]
    public void Verify_ShouldReturnTrue_WhenPasswordAndHashAreCorrect()
    {
        // Arrange
        string password = "TestPassword123!";
        string hashedPassword = _sut.Hash(password);

        // Act
        bool isValid = _sut.Verify(password, hashedPassword);

        // Assert
        Assert.True(isValid);
    }

    [Fact(DisplayName = "當傳入錯誤的密碼與Hash字串時，應回傳驗證失敗")]
    public void Verify_ShouldReturnFalse_WhenPasswordAndHashAreIncorrect()
    {
        // Arrange
        string password = "TestPassword123!";
        string wrongPassword = "WrongPassword456!";
        string hashedPassword = _sut.Hash(password);

        // Act
        bool isValid = _sut.Verify(wrongPassword, hashedPassword);

        // Assert
        Assert.False(isValid);
    }

    [Fact(DisplayName = "當同密碼多次Hash時，應回傳不同的Hash字串")]
    public void Hash_ShouldReturnDifferentHashStrings_ForSamePassword()
    {
        // Arrange
        string password = "TestPassword123!";

        // Act
        string hash1 = _sut.Hash(password);
        string hash2 = _sut.Hash(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
