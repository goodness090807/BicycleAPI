using BicycleAPI.Domain.Entities.RefreshToken;

namespace BicycleAPI.Domain.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshTokenEntity>
{
    /// <summary>
    /// 透過雜湊後的 Token 取得 Refresh Token
    /// </summary>
    /// <param name="tokenHash">雜湊後的 Token 字串</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>對應的 Refresh Token 實體，若不存在則為 null</returns>
    Task<RefreshTokenEntity?> GetByHashTokenAsync(string tokenHash, CancellationToken cancellationToken);
}
