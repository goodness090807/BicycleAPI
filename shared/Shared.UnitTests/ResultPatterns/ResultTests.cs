using Shared.ResultPatterns;

namespace Shared.UnitTests.ResultPatterns;

public class ResultTests
{
    [Fact(DisplayName = "Success 應回傳成功結果")]
    public void Success_Should_ReturnSuccessResult()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact(DisplayName = "Failure 應回傳失敗結果")]
    public void Failure_Should_ReturnFailureResult()
    {
        // Arrange
        var error = new Error("Test.Error", "錯誤標題", "發生錯誤");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal("Test.Error", result.Error.Code);
        Assert.Equal("錯誤標題", result.Error.Title);
        Assert.Equal("發生錯誤", result.Error.Detail);
    }

    [Fact(DisplayName = "泛型 Success 應回傳成功結果並包含值")]
    public void Generic_Success_Should_ReturnSuccessResultWithValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(value, result.Value);
    }

    [Fact(DisplayName = "泛型 Failure 應回傳失敗結果")]
    public void Generic_Failure_Should_ReturnFailureResult()
    {
        // Arrange
        var error = new Error("Test.Error", "錯誤標題", "發生錯誤");

        // Act
        var result = Result<int>.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal("Test.Error", result.Error.Code);

        // 嘗試存取 Value 屬性應拋出例外
        Assert.Throws<InvalidOperationException>(() => { var _ = result.Value; });
    }

    [Fact(DisplayName = "泛型 Success 應能存取不同型別的值")]
    public void Generic_Success_Should_AccessDifferentTypeValues()
    {
        // Arrange
        var stringValue = "測試字串";
        var objectValue = new { Id = 1, Name = "測試" };

        // Act
        var stringResult = Result<string>.Success(stringValue);
        var objectResult = Result<object>.Success(objectValue);

        // Assert
        Assert.Equal(stringValue, stringResult.Value);
        Assert.Equal(objectValue, objectResult.Value);
    }

    [Fact(DisplayName = "建立失敗結果時若不包含錯誤應拋出例外")]
    public void Failure_WithoutError_Should_ThrowException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => Result.Failure(Error.None));
    }

    [Fact(DisplayName = "建立成功結果時若包含錯誤應拋出例外")]
    public void Success_WithError_Should_ThrowException()
    {
        // Arrange
        var error = new Error("Test.Error", "錯誤標題", "發生錯誤");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new TestableResult(true, error));
    }

    // 用於測試 protected 建構函式的輔助類別
    private class TestableResult : Result
    {
        public TestableResult(bool isSuccess, Error error) : base(isSuccess, error) { }
    }
}