namespace BicycleAPI.Api.Extensions;

public class CookieSettings
{
    public const string SectionName = "Cookie";

    /// <summary>
    /// 允許的前端來源網址（用於 CORS）
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Cookie 的 Domain（可選，跨子網域時使用）
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// 是否為安全 Cookie（生產環境應為 true）
    /// </summary>
    public bool Secure { get; set; } = true;
}
