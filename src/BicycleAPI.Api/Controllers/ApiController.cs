using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.ResultPatterns;

namespace BicycleAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    /// <summary>
    /// 將 Result 轉換為適當的 IActionResult
    /// </summary>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error);
    }

    /// <summary>
    /// 將 Result<TValue> 轉換為適當的 IActionResult
    /// </summary>
    protected IActionResult HandleResult<TValue>(Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error);
    }

    /// <summary>
    /// 根據 ErrorType 回傳對應的 HTTP 狀態碼
    /// </summary>
    protected virtual IActionResult Problem(Error error)
    {
        var statusCode = error switch
        {
            DomainError => StatusCodes.Status400BadRequest,
            ValidationError => StatusCodes.Status400BadRequest,
            UnauthorizedError => StatusCodes.Status401Unauthorized,
            ForbiddenError => StatusCodes.Status403Forbidden,
            NotFoundError => StatusCodes.Status404NotFound,
            ConflictError => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: statusCode,
            title: error.Title,
            detail: error.Detail
        );

        if (error is ValidationError validationError)
        {
            problemDetails.Extensions["errors"] = validationError.Failures;
        }

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
