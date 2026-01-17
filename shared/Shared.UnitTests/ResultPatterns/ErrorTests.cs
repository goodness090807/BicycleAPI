using Shared.ResultPatterns;

namespace Shared.UnitTests.ResultPatterns;

public class ErrorTests
{
    [Fact(DisplayName = "Error.None 應為空錯誤")]
    public void Error_None_Should_BeEmptyError()
    {
        // Arrange & Act
        var error = Error.None;

        // Assert
        Assert.Equal(string.Empty, error.Code);
        Assert.Equal(string.Empty, error.Title);
        Assert.Equal(string.Empty, error.Detail);
    }

    [Fact(DisplayName = "Error.NullValue 應有正確的值")]
    public void Error_NullValue_Should_HaveCorrectValues()
    {
        // Arrange & Act
        var error = Error.NullValue;

        // Assert
        Assert.Equal("Error.NullValue", error.Code);
        Assert.Equal("值為 Null。", error.Title);
        Assert.Equal("值為 Null。", error.Detail);
    }

    [Fact(DisplayName = "相同 Error 應相等")]
    public void Error_WithSameValues_Should_BeEqual()
    {
        // Arrange
        var error1 = new Error("Test.Error", "錯誤標題", "錯誤詳情");
        var error2 = new Error("Test.Error", "錯誤標題", "錯誤詳情");

        // Act & Assert
        Assert.Equal(error1, error2);
    }

    [Fact(DisplayName = "不同 Error 應不相等")]
    public void Error_WithDifferentValues_Should_NotBeEqual()
    {
        // Arrange
        var error1 = new Error("Test.Error1", "錯誤標題1", "錯誤詳情1");
        var error2 = new Error("Test.Error2", "錯誤標題2", "錯誤詳情2");

        // Act & Assert
        Assert.NotEqual(error1, error2);
    }

    [Fact(DisplayName = "DomainError 應繼承自 Error")]
    public void DomainError_Should_InheritFromError()
    {
        // Arrange
        var domainError = new DomainError("Domain.Error", "領域錯誤", "領域錯誤詳情");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(domainError);
        Assert.Equal("Domain.Error", domainError.Code);
        Assert.Equal("領域錯誤", domainError.Title);
        Assert.Equal("領域錯誤詳情", domainError.Detail);
    }

    [Fact(DisplayName = "ValidationError 應包含驗證失敗資訊")]
    public void ValidationError_Should_ContainFailures()
    {
        // Arrange
        var failures = new Dictionary<string, ValidationErrorDetail[]>
        {
            {
                "Field1", new[]
                {
                    new ValidationErrorDetail("Error.Code1", "錯誤訊息 1"),
                    new ValidationErrorDetail("Error.Code2", "錯誤訊息 2")
                }
            },
            {
                "Field2", new[]
                {
                    new ValidationErrorDetail("Error.Code3", "錯誤訊息 3")
                }
            }
        };

        // Act
        var validationError = new ValidationError("Validation.Failed", failures);

        // Assert
        Assert.IsAssignableFrom<Error>(validationError);
        Assert.Equal("Validation.Failed", validationError.Code);
        Assert.Equal(2, validationError.Failures.Count);
        Assert.True(validationError.Failures.ContainsKey("Field1"));
        Assert.True(validationError.Failures.ContainsKey("Field2"));
    }

    [Fact(DisplayName = "UnauthorizedError 應繼承自 Error")]
    public void UnauthorizedError_Should_InheritFromError()
    {
        // Arrange
        var error = new UnauthorizedError("Auth.Unauthorized", "未授權", "使用者未授權存取此資源");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(error);
        Assert.Equal("Auth.Unauthorized", error.Code);
    }

    [Fact(DisplayName = "ForbiddenError 應繼承自 Error")]
    public void ForbiddenError_Should_InheritFromError()
    {
        // Arrange
        var error = new ForbiddenError("Auth.Forbidden", "禁止存取", "使用者無權存取此資源");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(error);
        Assert.Equal("Auth.Forbidden", error.Code);
    }

    [Fact(DisplayName = "NotFoundError 應繼承自 Error")]
    public void NotFoundError_Should_InheritFromError()
    {
        // Arrange
        var error = new NotFoundError("Resource.NotFound", "找不到資源", "請求的資源不存在");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(error);
        Assert.Equal("Resource.NotFound", error.Code);
    }

    [Fact(DisplayName = "ConflictError 應繼承自 Error")]
    public void ConflictError_Should_InheritFromError()
    {
        // Arrange
        var error = new ConflictError("Resource.Conflict", "資源衝突", "資源已存在或發生衝突");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(error);
        Assert.Equal("Resource.Conflict", error.Code);
    }

    [Fact(DisplayName = "InternalServerError 應繼承自 Error")]
    public void InternalServerError_Should_InheritFromError()
    {
        // Arrange
        var error = new InternalServerError("Server.Error", "伺服器錯誤", "發生內部伺服器錯誤");

        // Act & Assert
        Assert.IsAssignableFrom<Error>(error);
        Assert.Equal("Server.Error", error.Code);
    }
}
