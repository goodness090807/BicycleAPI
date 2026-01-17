namespace Shared.ResultPatterns;

/// <summary>
/// 表示應用程式中的錯誤資訊。這是一個不可變的記錄類型，包含錯誤代碼和描述。
/// </summary>
/// <param name="Code">錯誤代碼，用於識別錯誤類型。</param>
/// <param name="Title">錯誤的簡短標題或摘要。</param>
/// <param name="Detail">錯誤的詳細描述。</param>
public record Error(string Code, string Title, string Detail)
{
    public static readonly Error None = new(string.Empty, string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "值為 Null。", "值為 Null。");
}

public sealed record DomainError(string Code, string Title, string Detail) : Error(Code, Title, Detail);

public sealed record ValidationError(string Code, Dictionary<string, ValidationErrorDetail[]> Failures) : Error(Code, "一項或多項驗證失敗。", "請檢查提供的資料。");

public sealed record ValidationErrorDetail(string Code, string Message);

public sealed record UnauthorizedError(string Code, string Title, string Detail) : Error(Code, Title, Detail);

public sealed record ForbiddenError(string Code, string Title, string Detail) : Error(Code, Title, Detail);

public sealed record NotFoundError(string Code, string Title, string Detail) : Error(Code, Title, Detail);

public sealed record ConflictError(string Code, string Title, string Detail) : Error(Code, Title, Detail);

public sealed record InternalServerError(string Code, string Title, string Detail) : Error(Code, Title, Detail);