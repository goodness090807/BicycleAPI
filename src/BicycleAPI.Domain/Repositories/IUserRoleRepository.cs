using BicycleAPI.Domain.Entities.UserRole;

namespace BicycleAPI.Domain.Repositories;

public interface IUserRoleRepository : IGenericRepository<UserRoleEntity>
{
    /// <summary>
    /// 透過使用者 Id取得使用者角色關聯
    /// </summary>
    /// <param name="userId">使用者 Id</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>使用者角色關聯集合</returns>
    Task<IEnumerable<UserRoleEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
