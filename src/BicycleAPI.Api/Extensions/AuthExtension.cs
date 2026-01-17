using System.Security.Claims;

namespace BicycleAPI.Api.Extensions;

public static class AuthExtension
{
    public static Guid GetUserIdFromClaims(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("錯誤的UserId");
        }

        return userId;
    }
}
