using BicycleAPI.Domain.Entities.UserRole;
using BicycleAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence.Repositories;

public class UserRoleRepository : GenericRepository<UserRoleEntity>, IUserRoleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserRoleEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync(cancellationToken);
    }
}
