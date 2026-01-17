using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BicycleAPI.Api.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(IHostEnvironment env)
    {
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 定義 ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server.Error",
            Detail = _env.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred. Please try again later.",
            Type = "https://tools.ietf.org/html/rfc7807",
            Instance = httpContext.Request.Path
        };

        // 在開發環境下，加入更多除錯資訊
        if (_env.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().FullName;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    message = exception.InnerException.Message,
                    type = exception.InnerException.GetType().FullName
                };
            }
        }

        // 設定回應狀態碼
        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // 寫入回應
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // 回傳 true 表示錯誤已被處理，管線不需要繼續拋出例外
        return true;
    }
}
