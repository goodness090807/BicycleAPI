using BicycleAPI.Domain.Entities.User;
using BicycleAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    // <inheritdoc/>
    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
