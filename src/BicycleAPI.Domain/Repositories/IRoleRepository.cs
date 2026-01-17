using BicycleAPI.Domain.Entities.Role;

namespace BicycleAPI.Domain.Repositories;

public interface IRoleRepository : IGenericRepository<RoleEntity>
{
    /// <summary>
    /// 透過角色代碼取得角色實體
    /// </summary>
    /// <param name="code">代碼名稱</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>角色實體</returns>
    Task<RoleEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
