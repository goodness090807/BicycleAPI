using System.Security.Claims;

namespace Shared.Security.JWTHandler;

public interface ITokenHandler
{
    string GenerateJwtToken(string userId, IEnumerable<string>? roles = null, IEnumerable<Claim>? claims = null, int tokenExpirationMinutes = 10);
    string GenerateRefreshToken();
    int GetRefreshTokenExpirationDays();
}
