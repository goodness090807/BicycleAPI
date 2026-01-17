using BicycleAPI.Domain.Entities.User;

namespace BicycleAPI.Domain.Repositories;

public interface IUserRepository : IGenericRepository<UserEntity>
{
    /// <summary>
    /// 透過 Email取得使用者
    /// </summary>
    /// <param name="email">使用者 Email</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>使用者實體</returns>
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
