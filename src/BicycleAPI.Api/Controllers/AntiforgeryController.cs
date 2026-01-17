using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace BicycleAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AntiforgeryController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    /// <summary>
    /// 取得 CSRF Token
    /// 前端在進行任何 POST/PUT/DELETE 請求前應先呼叫此端點取得 Token
    /// </summary>
    [HttpGet("token")]
    public IActionResult GetAntiforgeryToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        // 將 Request Token 透過回應標頭傳給前端
        Response.Headers.Append("X-CSRF-TOKEN", tokens.RequestToken!);

        return Ok(new { message = "CSRF token generated" });
    }
}
