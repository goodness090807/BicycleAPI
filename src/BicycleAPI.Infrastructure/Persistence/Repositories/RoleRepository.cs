using BicycleAPI.Domain.Entities.Role;
using BicycleAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence.Repositories;

public class RoleRepository : GenericRepository<RoleEntity>, IRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<RoleEntity?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
    }
}
