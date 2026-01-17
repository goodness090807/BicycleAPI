using BicycleAPI.Domain.Entities.Permission;

namespace BicycleAPI.Domain.Repositories;

public interface IPermissionRepository : IGenericRepository<PermissionEntity>
{
    /// <summary>
    /// 取得使用者的所有權限代碼（透過角色關聯）
    /// </summary>
    /// <param name="userId">使用者 Id</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>權限代碼集合</returns>
    Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
