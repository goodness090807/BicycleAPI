using BicycleAPI.Api.Extensions;
using BicycleAPI.Application.Features.Auth.Commands.Login;
using BicycleAPI.Application.Features.Auth.Commands.Logout;
using BicycleAPI.Application.Features.Auth.Commands.RefreshToken;
using BicycleAPI.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BicycleAPI.Api.Controllers;

public class AuthController : ApiController
{
    private readonly IMediator _mediator;
    private readonly CookieSettings _cookieSettings;
    private const string RefreshTokenCookieName = "RefreshToken";

    public AuthController(IMediator mediator, IOptions<CookieSettings> cookieSettings)
    {
        _mediator = mediator;
        _cookieSettings = cookieSettings.Value;
    }

    #region JSON Response 端點 (適用於 Mobile App / API)

    /// <summary>
    /// 使用者註冊 - Refresh Token 透過 JSON Response 回傳
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// 使用者登入 - Refresh Token 透過 JSON Response 回傳
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// 刷新 Token - Refresh Token 透過 JSON Body 傳入
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// 登出 - 撤銷 Refresh Token
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : HandleResult(result);
    }

    #endregion

    #region HttpOnly Cookie 端點 (適用於 Web 前端 SPA 跨域請求)

    /// <summary>
    /// 使用者註冊 - Refresh Token 透過 HttpOnly Cookie 回傳
    /// </summary>
    [HttpPost("cookie/register")]
    public async Task<IActionResult> RegisterWithCookie([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        // 設定 Refresh Token 到 HttpOnly Cookie
        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAt);

        // 回傳 Access Token（不包含 Refresh Token）
        return Ok(new
        {
            result.Value.UserId,
            result.Value.Email,
            result.Value.DisplayName,
            result.Value.AccessToken
        });
    }

    /// <summary>
    /// 使用者登入 - Refresh Token 透過 HttpOnly Cookie 回傳
    /// </summary>
    [HttpPost("cookie/login")]
    public async Task<IActionResult> LoginWithCookie([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        // 設定 Refresh Token 到 HttpOnly Cookie
        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAt);

        // 回傳 Access Token（不包含 Refresh Token）
        return Ok(new
        {
            result.Value.UserId,
            result.Value.Email,
            result.Value.DisplayName,
            result.Value.AccessToken
        });
    }

    /// <summary>
    /// 刷新 Token - 從 HttpOnly Cookie 讀取 Refresh Token
    /// </summary>
    [HttpPost("cookie/refresh")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshWithCookie()
    {
        // 從 Cookie 讀取 Refresh Token
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh Token 不存在" });
        }

        var command = new RefreshTokenCommand(refreshToken);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            // 清除無效的 Cookie
            DeleteRefreshTokenCookie();
            return HandleResult(result);
        }

        // 設定新的 Refresh Token 到 Cookie
        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAt);

        // 只回傳 Access Token
        return Ok(new
        {
            result.Value.AccessToken
        });
    }

    /// <summary>
    /// 登出 - 撤銷資料庫中的 Token 並清除 Cookie
    /// </summary>
    [HttpPost("cookie/logout")]
    public async Task<IActionResult> LogoutWithCookie()
    {
        // 從 Cookie 讀取 Refresh Token
        var refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            // 撤銷資料庫中的 Token
            var command = new LogoutCommand(refreshToken);
            await _mediator.Send(command);
        }

        // 清除 Cookie
        DeleteRefreshTokenCookie();

        return NoContent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 設定 Refresh Token 到 HttpOnly Cookie
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                           // 防止 JavaScript 存取
            Secure = _cookieSettings.Secure,           // 只允許 HTTPS（生產環境）
            SameSite = SameSiteMode.None,              // 跨域必須設為 None
            Expires = expiresAt,                       // 過期時間
            Path = "/api/auth",                        // 限制 Cookie 只在此路徑下傳送
            Domain = _cookieSettings.Domain            // 可選：設定 Cookie Domain
        };

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    /// <summary>
    /// 刪除 Refresh Token Cookie
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = _cookieSettings.Secure,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(-1),     // 設定過期時間為過去
            Path = "/api/auth",
            Domain = _cookieSettings.Domain
        };

        Response.Cookies.Delete(RefreshTokenCookieName, cookieOptions);
    }

    #endregion
}
