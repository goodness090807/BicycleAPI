using BicycleAPI.Domain.Entities.Permission;
using BicycleAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence.Repositories;

public class PermissionRepository : GenericRepository<PermissionEntity>, IPermissionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PermissionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RolePermissions
            .Where(rp => _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .Contains(rp.RoleId))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
