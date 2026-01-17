using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Security.JWTHandler;

public class TokenHandler : ITokenHandler
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _tokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public TokenHandler(IOptions<TokenHandlerSettings> settings)
    {
        _secretKey = settings.Value.SecretKey;
        _issuer = settings.Value.Issuer;
        _audience = settings.Value.Audience;
        _tokenExpirationMinutes = settings.Value.ExpirationMinutes;
        _refreshTokenExpirationDays = settings.Value.RefreshTokenExpirationDays;
    }

    public string GenerateJwtToken(string userId, IEnumerable<string>? roles = null, IEnumerable<Claim>? claims = null, int tokenExpirationMinutes = 10)
    {
        var validExpirationMinutes = tokenExpirationMinutes > 10 ? tokenExpirationMinutes : _tokenExpirationMinutes;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var defultClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (roles != null)
        {
            foreach (var role in roles)
            {
                defultClaims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (claims != null)
        {
            defultClaims.AddRange(claims);
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: defultClaims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(validExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public int GetRefreshTokenExpirationDays()
    {
        return _refreshTokenExpirationDays > 0 ? _refreshTokenExpirationDays : 7;
    }
}
