using BicycleAPI.Domain.Entities.RefreshToken;
using BicycleAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshTokenEntity>, IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<RefreshTokenEntity?> GetByHashTokenAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
    }
}
